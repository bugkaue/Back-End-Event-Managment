using FluentValidation;

namespace ProjectSolutisDevTrail.Data.Dtos.FluentValidation;

public class CreateParticipanteDtoValidator : AbstractValidator<CreateParticipanteDto>
{
    public CreateParticipanteDtoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome do participante é obrigatório.")
            .MaximumLength(100).WithMessage("O nome do participante deve ter no máximo 100 caracteres.");
        
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O e-mail do participante é obrigatório.")
            .MaximumLength(100).WithMessage("O e-mail do participante deve ter no máximo 100 caracteres.")
            .EmailAddress().WithMessage("O e-mail do participante é inválido.");
    }
}