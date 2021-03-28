using FluentValidation;

namespace MagicEvents.Api.Service.Application.DTOs.Users.UpdateProfile
{
    public class UpdateProfileDtoValidator : AbstractValidator<UpdateProfileDto>
    {
        public UpdateProfileDtoValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("Invalid firstname parameter")
                .MaximumLength(100)
                .WithMessage("Invalid firstname parameter");
            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Invalid firstname parameter")
                .MaximumLength(100)
                .WithMessage("Invalid lastname parameter");
            RuleFor(x => x.Informations)
                .NotEmpty()
                .WithMessage("Invalid firstname parameter")
                .MaximumLength(1000)
                .WithMessage("Invalid information parameter");
        }
    }
}