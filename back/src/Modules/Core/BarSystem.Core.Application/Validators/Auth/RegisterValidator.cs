using BarSystem.Core.Application.DTOs.Auth;
using FluentValidation;

namespace BarSystem.Core.Application.Validators.Auth;

public class RegisterValidator : AbstractValidator<RegisterDto>
{
    public RegisterValidator()
    {
        RuleFor(x => x.GroupName)
            .NotEmpty().WithMessage("Nome do grupo é obrigatório")
            .MaximumLength(200);

        RuleFor(x => x.FirstBarName)
            .NotEmpty().WithMessage("Nome do primeiro bar é obrigatório")
            .MaximumLength(200);

        RuleFor(x => x.AdminName)
            .NotEmpty().WithMessage("Nome do administrador é obrigatório")
            .MaximumLength(200);

        RuleFor(x => x.AdminEmail)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido");

        RuleFor(x => x.AdminPassword)
            .NotEmpty().WithMessage("Senha é obrigatória")
            .MinimumLength(6).WithMessage("Senha deve ter no mínimo 6 caracteres");
    }
}
