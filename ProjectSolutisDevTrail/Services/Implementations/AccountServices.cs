using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using ProjectSolutisDevTrail.Data.Dtos;
using ProjectSolutisDevTrail.Data.Repositories.Interfaces;
using ProjectSolutisDevTrail.Models;
using ProjectSolutisDevTrail.Services.Interfaces;

namespace ProjectSolutisDevTrail.Services.Implementations
{
    public class AccountService : ControllerBase, IAccountService
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager; 
        private readonly IGenerateJwtToken _generateJwtTokenService;
        private readonly IEmailService _emailService;
        private readonly IAccountRepository _accountRepository;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TokenRevocationService _tokenRevocationService;

        public AccountService(
            UserManager<Usuario> userManager,
            RoleManager<IdentityRole> roleManager,
            IGenerateJwtToken generateJwtTokenService,
            IEmailService sendMailService,
            IAccountRepository accountRepository,
            IUrlHelperFactory urlHelperFactory,
            IHttpContextAccessor httpContextAccessor,
            TokenRevocationService tokenRevocationService)
        
        {
            _userManager = userManager;
            _roleManager = roleManager; 
            _generateJwtTokenService = generateJwtTokenService;
            _emailService = sendMailService;
            _accountRepository = accountRepository;
            _urlHelperFactory = urlHelperFactory;
            _httpContextAccessor = httpContextAccessor;
            _tokenRevocationService = tokenRevocationService;


        }
         public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return new BadRequestObjectResult(new { message = "O e-mail fornecido já está registrado." });
            }

            var user = new Usuario
            {
                UserName = model.Email,
                Email = model.Email,
                Nome = model.Nome,
                Sobrenome = model.Sobrenome,
                EmailConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
                return new BadRequestObjectResult(result.Errors);

            var roleExists = await _roleManager.RoleExistsAsync("User");
            if (!roleExists)
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole("User"));
                if (!roleResult.Succeeded)
                    return new BadRequestObjectResult("Erro ao criar o papel 'User'.");
            }

            var addToRoleResult = await _userManager.AddToRoleAsync(user, "User");
            if (!addToRoleResult.Succeeded)
                return new BadRequestObjectResult(addToRoleResult.Errors);

            var participante = new Participante
            {
                Nome = model.Nome,
                Sobrenome = model.Sobrenome,
                Email = model.Email,
                UsuarioId = user.Id
            };

            await _accountRepository.AddParticipanteAsync(participante);
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Generate the URL helper instance.
            var urlHelper = _urlHelperFactory.GetUrlHelper(new ActionContext(_httpContextAccessor.HttpContext, new RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()));
            var confirmationLink = urlHelper.Action(nameof(ConfirmEmail), "Account", new { token, email = user.Email }, _httpContextAccessor.HttpContext.Request.Scheme);

            if (confirmationLink == null)
            {
                return new BadRequestObjectResult("Erro ao gerar o link de confirmação de e-mail.");
            }
            await SendConfirmationEmail(user.Email, confirmationLink);

            return new OkObjectResult(new
            {
                message = "Registro bem-sucedido! Verifique seu e-mail para confirmar sua conta.",
                participanteId = participante.Id
            });
        }
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return new BadRequestObjectResult("Usuário não encontrado.");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
                return new OkObjectResult(new { message = "E-mail confirmado com sucesso!" });
            else
                return new BadRequestObjectResult("Falha na confirmação do e-mail.");
        }

        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                if (!user.EmailConfirmed)
                    return new UnauthorizedObjectResult(new { message = "E-mail não confirmado. Verifique sua caixa de entrada para confirmar seu e-mail." });

                bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

                var participante = await _accountRepository.GetParticipanteByUsuarioIdAsync(user.Id);

                var userRoles = await _userManager.GetRolesAsync(user);

                var token = _generateJwtTokenService.GenerateToken(user, isAdmin ? "Admin" : "User");

                return new OkObjectResult(new
                {
                    token,
                    participanteId = participante?.Id,
                    roles = userRoles,
                    isAdmin
                });
            }

            return new UnauthorizedObjectResult(new { message = "Credenciais inválidas." });
        }
        
        public void Logout(string token)
        {
            _tokenRevocationService.RevokeToken(token);
        } 

        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                return new BadRequestObjectResult("Usuário não encontrado ou e-mail não confirmado.");

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            await SendPasswordResetEmail(user.Email, code);

            return new OkObjectResult(new { message = "Instruções para redefinição de senha foram enviadas para seu e-mail." });
        }

        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return new NotFoundObjectResult("Usuário não encontrado.");

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                return new OkObjectResult(new { message = "Senha redefinida com sucesso!" });
            }

            return new BadRequestObjectResult("Falha ao redefinir a senha.");
        }

        public async Task<IActionResult> DeleteUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new NotFoundObjectResult(new { message = "Usuário não encontrado." });
            }

            var participante = await _accountRepository.GetParticipanteByUsuarioIdAsync(user.Id);
            if (participante != null)
            {
                await _accountRepository.RemoveParticipanteAsync(participante);
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return new BadRequestObjectResult(new { message = "Erro ao excluir o usuário." });
            }

            return new OkObjectResult(new { message = "Usuário e dados relacionados excluídos com sucesso." });
        }
        
        public class AuthService
        {
            private readonly TokenRevocationService _tokenRevocationService;

            public AuthService(TokenRevocationService tokenRevocationService)
            {
                _tokenRevocationService = tokenRevocationService;
            }

            public void Logout(string token)
            {
                _tokenRevocationService.RevokeToken(token);
            }
        }
        
        private async Task SendPasswordResetEmail(string email, string code)
        {
            await _emailService.SendPasswordResetEmail(email, code);
        }
        private string GenerateJwtToken(Usuario user, string role)
        {
            return _generateJwtTokenService.GenerateToken(user, role);
        }
        private async Task SendConfirmationEmail(string email, string confirmationLink)
        {
            await _emailService.SendConfirmationEmail(email, confirmationLink);
        }
    }  
}