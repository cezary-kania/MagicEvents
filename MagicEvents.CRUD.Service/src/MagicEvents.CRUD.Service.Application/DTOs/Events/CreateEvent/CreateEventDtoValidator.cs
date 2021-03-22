using System;
using FluentValidation;

namespace MagicEvents.CRUD.Service.Application.DTOs.Events.CreateEvent
{
    public class CreateEventDtoValidator : AbstractValidator<CreateEventDto>
    {
        public CreateEventDtoValidator()
        {
            RuleFor(x => x.OrganizerId)
                .NotEmpty()
                .WithMessage("Invalid OrganizerId");
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage($"Title can't be blank")
                .MaximumLength(100)
                .WithMessage($"Maximum length of event title exceeded");
            RuleFor(x => x.Description)
                .MaximumLength(4000)
                .WithMessage($"Maximum length of event description exceeded");
            RuleFor(x => x.StartsAt)
                .NotEmpty()
                .WithMessage($"Start date can't be blank")
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Invalid start date");
            RuleFor(x => x.EndsAt)
                .NotEmpty()
                .WithMessage($"End date can't be blank")
                .GreaterThan(DateTime.UtcNow)
                .WithMessage("Invalid end date")
                .GreaterThan(x => x.StartsAt)
                .WithMessage("Invalid dates setup");
        }
    }
}