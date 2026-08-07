using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace InsightHub.Application.Features.Datasets.Queries.GetDatasetSummary;

public record GetDatasetSummaryQuery(Guid Id)
    : IRequest<GetDatasetSummaryResponse>;
