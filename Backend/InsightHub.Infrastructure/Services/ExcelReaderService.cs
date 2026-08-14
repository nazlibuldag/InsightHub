using ClosedXML.Excel;
using InsightHub.Application.Interfaces;

namespace InsightHub.Infrastructure.Services;

public class ExcelReaderService : IExcelReaderService
{
    public async Task<(int TotalRows, int TotalColumns)> ReadExcelInfoAsync(
        string filePath,
        CancellationToken cancellationToken)
    {
        return await Task.Run(() =>
        {
            using var workbook = new XLWorkbook(filePath);

            var worksheet = workbook.Worksheet(1);

            var usedRange = worksheet.RangeUsed();

            if (usedRange == null)
                return (0, 0);

            int totalRows = usedRange.RowCount() - 1; // Header hariç
            int totalColumns = usedRange.ColumnCount();

            return (totalRows, totalColumns);

        }, cancellationToken);
    }
}