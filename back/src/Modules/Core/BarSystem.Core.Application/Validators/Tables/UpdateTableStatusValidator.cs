using BarSystem.Core.Application.DTOs.Tables;
using FluentValidation;

namespace BarSystem.Core.Application.Validators.Tables;

public class UpdateTableStatusValidator : AbstractValidator<UpdateTableStatusDto>
{
    public UpdateTableStatusValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status é obrigatório")
            .Must(status => new[] { "Free", "Occupied", "Reserved" }.Contains(status))
            .WithMessage("Status deve ser: Free, Occupied ou Reserved");
    }
}
