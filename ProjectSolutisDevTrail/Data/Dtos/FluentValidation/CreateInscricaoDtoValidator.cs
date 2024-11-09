using FluentValidation;

namespace ProjectSolutisDevTrail.Data.Dtos.FluentValidation;

public class CreateInscricaoDtoValidator : AbstractValidator<CreateInscricaoDto>
{
    public CreateInscricaoDtoValidator()
    {
        RuleFor(x => x.EventoId)
            .NotEmpty().WithMessage("O Id do evento é obrigatório.");
        
        RuleFor(x => x.ParticipanteId)
            .NotEmpty().WithMessage("O Id do participante é obrigatório.");
    }
}