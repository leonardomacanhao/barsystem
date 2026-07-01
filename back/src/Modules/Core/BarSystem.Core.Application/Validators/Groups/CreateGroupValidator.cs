using BarSystem.Core.Application.DTOs.Groups;
using FluentValidation;

namespace BarSystem.Core.Application.Validators.Groups;

public class CreateGroupValidator : AbstractValidator<CreateGroupDto>
{
    public CreateGroupValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome é obrigatório")
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.ContactEmail)
            .EmailAddress().WithMessage("Email inválido")
            .When(x => !string.IsNullOrEmpty(x.ContactEmail));

        RuleFor(x => x.ContactPhone)
            .MaximumLength(20);
    }
}
