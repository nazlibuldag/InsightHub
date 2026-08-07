using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace InsightHub.Application.Features.Datasets.Queries.GetAllDatasets;

public record GetAllDatasetsQuery
    : IRequest<List<GetAllDatasetsResponse>>;