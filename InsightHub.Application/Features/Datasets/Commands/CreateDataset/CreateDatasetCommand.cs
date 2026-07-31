using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;

namespace InsightHub.Application.Features.Datasets.Commands.CreateDataset;

public class CreateDatasetCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}