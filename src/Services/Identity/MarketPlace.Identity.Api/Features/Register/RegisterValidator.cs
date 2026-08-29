using FluentValidation;

namespace MarketPlace.Identity.Api.Features.Register;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");

        RuleFor(x => x.Role)
            .Must(role => string.IsNullOrEmpty(role) || role is "Buyer" or "Seller" or "Admin")
            .WithMessage("Role must be 'Buyer', 'Seller', or 'Admin'.");
    }
}
