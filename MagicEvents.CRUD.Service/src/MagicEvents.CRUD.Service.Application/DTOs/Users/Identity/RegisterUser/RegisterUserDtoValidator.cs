using FluentValidation;

namespace MagicEvents.CRUD.Service.Application.DTOs.Users.Identity.RegisterUser
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Invalid email address");
            int passwordLength = 8; // TODO: Add to appconfig.json as PasswordRules
            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(passwordLength).WithMessage($"Passwortd must have at least {passwordLength} characters")
                .Matches("[A-Z]").WithMessage("Password must contain at least 1 uppercase letter")
                .Matches("[a-z]").WithMessage("Password must contain at least 1 lowercase letter")
                .Matches("[0-9]").WithMessage("Password must contain at least 1 digit");
        }
    }
}