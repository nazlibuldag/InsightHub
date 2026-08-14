using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;
using Microsoft.AspNetCore.Http;

namespace InsightHub.Application.Features.Datasets.Commands.UploadDataset;

public class UploadDatasetCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public IFormFile File { get; set; } = null!;
}