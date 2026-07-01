using BarSystem.Core.Application.DTOs.Users;
using FluentValidation;

namespace BarSystem.Core.Application.Validators.Users;

public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(200);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Email inválido");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role é obrigatória")
            .Must(role => new[] { "GroupAdmin", "BarManager", "Waiter", "Cashier" }.Contains(role))
            .WithMessage("Role deve ser: GroupAdmin, BarManager, Waiter ou Cashier");
    }
}
