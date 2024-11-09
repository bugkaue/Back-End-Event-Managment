using FluentValidation;
using ProjectSolutisDevTrail.Data.Dtos;

namespace ProjectSolutisDevTrail.Data.Dtos.FluentValidation;

public class CreateEventoDtoValidator : AbstractValidator<CreateEventoDto>
{
    public CreateEventoDtoValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("O título é obrigatório.")
            .MaximumLength(100).WithMessage("O título deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Descricao)
            .NotEmpty().WithMessage("A descrição é obrigatória.")
            .MaximumLength(500).WithMessage("A descrição deve ter no máximo 500 caracteres.");

        RuleFor(x => x.DataHora)
            .GreaterThan(DateTime.Now).WithMessage("A data e hora devem ser futuras.");

        RuleFor(x => x.Local)
            .NotEmpty().WithMessage("O local é obrigatório.")
            .MaximumLength(200).WithMessage("O local deve ter no máximo 200 caracteres.");

        RuleFor(x => x.CapacidadeMaxima)
            .GreaterThan(0).WithMessage("A capacidade máxima deve ser maior que zero.");
    }
}