using BarSystem.Core.Application.DTOs.Tables;
using FluentValidation;

namespace BarSystem.Core.Application.Validators.Tables;

public class CreateTableValidator : AbstractValidator<CreateTableDto>
{
    public CreateTableValidator()
    {
        RuleFor(x => x.Number)
            .GreaterThan(0).WithMessage("Número da mesa deve ser maior que zero");

        RuleFor(x => x.Capacity)
            .GreaterThan(0).WithMessage("Capacidade deve ser maior que zero")
            .LessThanOrEqualTo(100).WithMessage("Capacidade máxima é 100 pessoas");

        RuleFor(x => x.Location)
            .MaximumLength(100).WithMessage("Localização deve ter no máximo 100 caracteres");
    }
}
