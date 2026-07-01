using BarSystem.Core.Application.DTOs.Tenants;
using FluentValidation;

namespace BarSystem.Core.Application.Validators.Tenants;

public class CreateTenantValidator : AbstractValidator<CreateTenantDto>
{
    public CreateTenantValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(200).WithMessage("Nome deve ter no máximo 200 caracteres");

        RuleFor(x => x.Cnpj)
            .MaximumLength(18).WithMessage("CNPJ deve ter no máximo 18 caracteres");
    }
}
