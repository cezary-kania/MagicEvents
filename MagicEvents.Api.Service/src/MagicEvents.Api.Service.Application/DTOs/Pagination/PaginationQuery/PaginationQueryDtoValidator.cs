using FluentValidation;

namespace MagicEvents.Api.Service.Application.DTOs.Pagination.PaginationQuery
{
    public class PaginationQueryDtoValidator : AbstractValidator<PaginationQueryDto>
    {
        public PaginationQueryDtoValidator()
        {
            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .WithMessage("Invalid pagination parameter");
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Invalid pagination parameter");
        }
    }
}