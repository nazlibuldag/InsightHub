using FluentValidation;
using InsightHub.Application.Features.Datasets.Queries.GetDatasetData;

namespace InsightHub.Application.Features.Datasets.Validators;

public class GetDatasetDataQueryValidator
    : AbstractValidator<GetDatasetDataQuery>
{
    public GetDatasetDataQueryValidator()
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
    }
}