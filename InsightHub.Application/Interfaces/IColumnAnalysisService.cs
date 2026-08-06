using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InsightHub.Domain.Entities;

namespace InsightHub.Application.Interfaces;

public interface IColumnAnalysisService
{
    Task<List<DatasetColumn>> AnalyzeAsync(string filePath, Guid datasetId);
}
