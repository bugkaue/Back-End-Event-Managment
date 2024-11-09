using FluentValidation;

namespace ProjectSolutisDevTrail.Data.Dtos.FluentValidation;

public class UpdateEventoDtoValidator : AbstractValidator<UpdateEventoDto>
{
    public UpdateEventoDtoValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("O título não pode estar vazio.")
            .MaximumLength(100).WithMessage("O título deve ter no máximo 100 caracteres.")
            .When(x => x.Titulo != null);

        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage("A descrição não pode estar vazia.")
            .MaximumLength(500).WithMessage("A descrição deve ter no máximo 500 caracteres.")
            .When(x => x.Descricao != null);

        RuleFor(x => x.DataHora)
            .GreaterThan(DateTime.Now).WithMessage("A data e hora devem ser futuras.")
            .When(x => x.DataHora.HasValue);

        RuleFor(x => x.Local)
            .NotEmpty().WithMessage("O local não pode estar vazio.")
            .MaximumLength(200).WithMessage("O local deve ter no máximo 200 caracteres.")
            .When(x => x.Local != null);

        RuleFor(x => x.CapacidadeMaxima)
            .GreaterThan(0).WithMessage("A capacidade máxima deve ser maior que zero.")
            .When(x => x.CapacidadeMaxima.HasValue);
    }
}