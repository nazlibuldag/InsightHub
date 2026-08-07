using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;

namespace InsightHub.Application.Features.Datasets.Commands.UpdateDataset;

public class UpdateDatasetCommand : IRequest
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}