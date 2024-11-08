using ProjectSolutisDevTrail.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ProjectSolutisDevTrail.Services.Implementations;
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

// Adiciona os serviços do Identity e configuração de banco de dados
builder.Services.AddDbContext<EventoContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Configura o Identity com a classe de usuário personalizada (Usuario) e IdentityRole
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

// Configuração de autenticação JWT
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

// Adiciona os serviços ao contêiner.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API Project", Version = "v1" });

    // Configurar a definição de segurança para Bearer Token
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

// Registrando os serviços e repositórios para injeção de dependência
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
builder.Services.AddScoped<IGenerateJwtToken, GenerateJwtTokenService>();
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await SeedData.CreateAdminUser(services);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao criar o administrador: {ex.Message}");
    }
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins"); // Ensure this is before UseAuthentication and UseAuthorization

// Middleware de autenticação e autorização
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();