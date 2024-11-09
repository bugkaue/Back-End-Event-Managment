using FluentValidation;

namespace ProjectSolutisDevTrail.Data.Dtos.FluentValidation;

public class UpdateParticipanteDtoValidator : AbstractValidator<UpdateParticipanteDto>
{
    public UpdateParticipanteDtoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome não pode estar vazio.")
            .MaximumLength(100).WithMessage("O nome deve ter no máximo 100 caracteres.")
            .When(x => x.Nome != null);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail não pode estar vazio.")
            .EmailAddress().WithMessage("O e-mail deve ser válido.")
            .MaximumLength(150).WithMessage("O e-mail deve ter no máximo 150 caracteres.")
            .When(x => x.Email != null);
    }
}