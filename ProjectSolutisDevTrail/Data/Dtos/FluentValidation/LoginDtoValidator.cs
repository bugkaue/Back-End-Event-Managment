using FluentValidation;

namespace ProjectSolutisDevTrail.Data.Dtos.FluentValidation;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .MaximumLength(50).WithMessage("O e-mail deve ter no maximo 50 caracteres.")
            .EmailAddress().WithMessage("Digite um e-mail valido.");
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("A senha é obrigatória.")
            .MinimumLength(8).WithMessage("A senha deve ter no mínimo 8 caracteres.")
            .MaximumLength(16).WithMessage("A senha deve ter no máximo 16 caracteres.")
            .Matches(@"[A-Z]").WithMessage("A senha deve conter pelo menos uma letra maiúscula.")
            .Matches(@"[a-z]").WithMessage("A senha deve conter pelo menos uma letra minúscula.")
            .Matches(@"[0-9]").WithMessage("A senha deve conter pelo menos um número.")
            .Matches(@"[\W_]").WithMessage("A senha deve conter pelo menos um caractere especial.");
    }
}