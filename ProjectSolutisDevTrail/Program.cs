using ProjectSolutisDevTrail.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ProjectSolutisDevTrail.Services;
using ProjectSolutisDevTrail.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ProjectSolutisDevTrail.Models;
using SendGrid;
using ProjectSolutisDevTrail.Services.Interfaces;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using ProjectSolutisDevTrail.Data.Repositories.Implementations;
using ProjectSolutisDevTrail.Data.Repositories;
using ProjectSolutisDevTrail.Data.Repositories.Interfaces.generic;
using ProjectSolutisDevTrail.Data.Repositories.Generic;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("EventoConnection");

// Adiciona os servi�os do Identity e configura��o de banco de dados
builder.Services.AddDbContext<EventoContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Configura o Identity com a classe de usu�rio personalizada (Usuario) e IdentityRole
builder.Services.AddIdentity<Usuario, IdentityRole>()
    .AddEntityFrameworkStores<EventoContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<ISendGridClient>(provider => 
{
    var apiKey = builder.Configuration["SendGrid:ApiKey"];
    return new SendGridClient(apiKey);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Configura��o de autentica��o JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        RoleClaimType = ClaimTypes.Role
    };
});

// Adiciona os servi�os ao cont�iner.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Project", Version = "v1" });

    // Configurar a defini��o de seguran�a para Bearer Token
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Por favor, insira 'Bearer' seguido pelo seu token JWT",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});
// Adicionando AutoMapper para os Dtos
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Registrando os servi�os e reposit�rios para inje��o de depend�ncia
builder.Services.AddScoped<IEventoService, EventoService>();
builder.Services.AddScoped<IEventoRepository, EventoRepository>();
builder.Services.AddScoped<IAtividadeService, AtividadeService>();
builder.Services.AddScoped<IAtividadeRepository, AtividadeRepository>();
builder.Services.AddScoped<IInscricaoService, InscricaoService>();
builder.Services.AddScoped<IInscricaoRepository, InscricaoRepository>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<IGenericRepository<Evento>, GenericRepository<Evento>>();
builder.Services.AddScoped<IParticipanteRepository, ParticipanteRepository>();
builder.Services.AddScoped<IParticipanteService, ParticipanteService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();



var app = builder.Build();

// Cria um escopo para executar a tarefa de criar o usu�rio administrador
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Chama o m�todo para criar o usu�rio administrador
        await SeedData.CreateAdminUser(services);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao criar o administrador: {ex.Message}");
    }
}

// Configura��o do pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Middleware de autentica��o e autoriza��o
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseCors("AllowAllOrigins");

app.Run();
