using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;

namespace InsightHub.Application.Features.Datasets.Commands.DeleteDataset;

public record DeleteDatasetCommand(Guid Id) : IRequest;