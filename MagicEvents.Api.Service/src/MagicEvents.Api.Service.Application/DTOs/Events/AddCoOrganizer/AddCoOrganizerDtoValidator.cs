using FluentValidation;

namespace MagicEvents.Api.Service.Application.DTOs.Events.AddCoOrganizer
{
    public class AddCoOrganizerDtoValidator : AbstractValidator<AddCoOrganizerDto>
    {
        public AddCoOrganizerDtoValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User id can't be empty.");
        }
    }
}