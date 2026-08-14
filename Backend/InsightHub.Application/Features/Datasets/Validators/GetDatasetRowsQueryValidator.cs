using FluentValidation;

namespace InsightHub.Application.Features.Datasets.Queries.GetDatasetRows;

public class GetDatasetRowsQueryValidator : AbstractValidator<GetDatasetRowsQuery>
{
    public GetDatasetRowsQueryValidator()
    {
        RuleFor(x => x.DatasetId)
            .NotEmpty()
            .WithMessage("DatasetId boş olamaz.");

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page en az 1 olmalıdır.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("PageSize 1 ile 100 arasında olmalıdır.");

        RuleFor(x => x.SortOrder)
            .Must(x =>
                x.Equals("asc", StringComparison.OrdinalIgnoreCase) ||
                x.Equals("desc", StringComparison.OrdinalIgnoreCase))
            .WithMessage("SortOrder sadece 'asc' veya 'desc' olabilir.");

        When(x => !string.IsNullOrWhiteSpace(x.FilterColumn), () =>
        {
            RuleFor(x => x.FilterOperator)
                .NotEmpty()
                .WithMessage("FilterOperator belirtilmelidir.");

            RuleFor(x => x.FilterValue)
                .NotEmpty()
                .WithMessage("FilterValue belirtilmelidir.");
        });
    }
}