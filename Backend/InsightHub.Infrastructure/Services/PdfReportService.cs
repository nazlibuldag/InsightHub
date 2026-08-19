using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InsightHub.Application.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace InsightHub.Infrastructure.Services;

public class PdfReportService : IPdfReportService
{
    private readonly IDatasetRepository _datasetRepository;
    private readonly IAiAnalysisService _aiAnalysisService;
    private readonly ISavedAnalysisRepository _savedAnalysisRepository;

    public PdfReportService(
        IDatasetRepository datasetRepository,
        IAiAnalysisService aiAnalysisService,
        ISavedAnalysisRepository savedAnalysisRepository)
    {
        _datasetRepository = datasetRepository;
        _aiAnalysisService = aiAnalysisService;
        _savedAnalysisRepository = savedAnalysisRepository;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> GenerateDatasetPdfReportAsync(
        Guid datasetId,
        CancellationToken cancellationToken = default)
    {
        var dataset = await _datasetRepository.GetByIdWithColumnsAsync(datasetId, cancellationToken);
        if (dataset == null)
        {
            throw new KeyNotFoundException("Dataset bulunamadı.");
        }

        var columnSummaryList = dataset.Columns.Select(c => new
        {
            c.ColumnName,
            DataType = c.DataType.ToString(),
            c.NullCount,
            c.UniqueCount,
            c.MinValue,
            c.MaxValue,
            c.AverageValue,
            c.MedianValue
        });

        var columnSummaryJson = System.Text.Json.JsonSerializer.Serialize(columnSummaryList);
        var statsSummary = $"TotalRows: {dataset.TotalRows}, TotalColumns: {dataset.TotalColumns}";

        var aiInsights = await _aiAnalysisService.GenerateDatasetInsightsAsync(
            dataset.Name,
            dataset.TotalRows,
            dataset.TotalColumns,
            columnSummaryJson,
            statsSummary,
            cancellationToken);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontColor(Colors.Grey.Darken3));

                // Header
                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("InsightHub").FontSize(20).Bold().FontColor("#635bff");
                        col.Item().Text("Enterprise Data Analytics & AI Executive Report").FontSize(9).FontColor(Colors.Grey.Medium);
                    });

                    row.ConstantItem(120).AlignRight().Text(DateTime.Now.ToString("dd.MM.yyyy HH:mm")).FontSize(9).FontColor(Colors.Grey.Medium);
                });

                // Content
                page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                {
                    col.Spacing(15);

                    // Title Card
                    col.Item().Background("#eeecff").Padding(12).Column(c =>
                    {
                        c.Item().Text($"Veri Seti Raporu: {dataset.Name}").FontSize(16).Bold().FontColor("#635bff");
                        c.Item().Text($"Açıklama: {dataset.Description ?? "Özel Veri Seti"}").FontSize(10).FontColor(Colors.Grey.Darken2);
                        c.Item().Text($"Yükleme Tarihi: {dataset.UploadedAt:dd MMMM yyyy HH:mm} | Toplam Satır: {dataset.TotalRows} | Kolon Sayısı: {dataset.TotalColumns}").FontSize(9).FontColor(Colors.Grey.Medium);
                    });

                    // AI Executive Summary
                    col.Item().Column(c =>
                    {
                        c.Item().Text("🤖 Yapay Zeka (AI) Yönetici Özeti").FontSize(14).Bold().FontColor("#635bff");
                        c.Item().LineHorizontal(1).LineColor("#635bff");
                        c.Item().PaddingTop(6).Text(aiInsights).FontSize(10).LineHeight(1.5f);
                    });

                    // Column Summaries Table
                    col.Item().Column(c =>
                    {
                        c.Item().Text("📊 Kolon İstatistikleri Özet Tablosu").FontSize(14).Bold().FontColor("#635bff");
                        c.Item().LineHorizontal(1).LineColor("#635bff");

                        c.Item().PaddingTop(6).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3); // Name
                                columns.RelativeColumn(2); // Type
                                columns.RelativeColumn(2); // Min
                                columns.RelativeColumn(2); // Max
                                columns.RelativeColumn(2); // Avg
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellStyle).Text("Kolon Adı").Bold();
                                header.Cell().Element(CellStyle).Text("Veri Tipi").Bold();
                                header.Cell().Element(CellStyle).Text("Min").Bold();
                                header.Cell().Element(CellStyle).Text("Max").Bold();
                                header.Cell().Element(CellStyle).Text("Ortalama").Bold();

                                static IContainer CellStyle(IContainer container) =>
                                    container.DefaultTextStyle(x => x.FontSize(9).FontColor(Colors.White))
                                             .Background("#635bff")
                                             .PaddingVertical(4)
                                             .PaddingHorizontal(6);
                            });

                            foreach (var col in dataset.Columns)
                            {
                                table.Cell().Element(RowStyle).Text(col.ColumnName);
                                table.Cell().Element(RowStyle).Text(col.DataType.ToString());
                                table.Cell().Element(RowStyle).Text(col.MinValue.HasValue ? col.MinValue.Value.ToString("0.##") : "-");
                                table.Cell().Element(RowStyle).Text(col.MaxValue.HasValue ? col.MaxValue.Value.ToString("0.##") : "-");
                                table.Cell().Element(RowStyle).Text(col.AverageValue.HasValue ? col.AverageValue.Value.ToString("0.##") : "-");

                                static IContainer RowStyle(IContainer container) =>
                                    container.BorderBottom(1)
                                             .BorderColor(Colors.Grey.Lighten2)
                                             .PaddingVertical(4)
                                             .PaddingHorizontal(6)
                                             .DefaultTextStyle(x => x.FontSize(9));
                            }
                        });
                    });
                });

                // Footer
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("InsightHub AI Analytics Platform - Sayfa ");
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
        });

        using var ms = new MemoryStream();
        document.GeneratePdf(ms);
        return ms.ToArray();
    }

    public async Task<byte[]> GenerateSavedAnalysisPdfReportAsync(
        Guid savedAnalysisId,
        CancellationToken cancellationToken = default)
    {
        var analysis = await _savedAnalysisRepository.GetByIdWithDetailsAsync(savedAnalysisId, cancellationToken);
        if (analysis == null)
        {
            throw new KeyNotFoundException("Kaydedilmiş analiz bulunamadı.");
        }

        var dataset = analysis.Dataset;
        var userName = analysis.User != null ? $"{analysis.User.FirstName} {analysis.User.LastName}" : "Kullanıcı";

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontColor(Colors.Grey.Darken3));

                // Header
                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("InsightHub").FontSize(22).Bold().FontColor("#ec4899");
                        col.Item().Text("Kurumsal Kaydedilmiş Analiz Raporu").FontSize(10).FontColor(Colors.Grey.Medium);
                    });

                    row.ConstantItem(140).AlignRight().Column(col =>
                    {
                        col.Item().Text(DateTime.Now.ToString("dd.MM.yyyy HH:mm")).FontSize(9).FontColor(Colors.Grey.Medium);
                        col.Item().Text($"Rapor ID: #{analysis.Id.ToString()[..8]}").FontSize(8).FontColor(Colors.Grey.Medium);
                    });
                });

                // Content
                page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                {
                    col.Spacing(14);

                    // Analysis Info Card
                    col.Item().Background("#fdf2f8").Border(1).BorderColor("#fbcfe8").Padding(14).Column(c =>
                    {
                        c.Spacing(4);
                        c.Item().Text($"Analiz Adı: {analysis.Title}").FontSize(16).Bold().FontColor("#be185d");
                        c.Item().Text($"Analiz Türü: {analysis.AnalysisType} | Veri Seti: {dataset?.Name ?? "Bilinmiyor"}").FontSize(11).Bold().FontColor(Colors.Grey.Darken2);
                        c.Item().Text($"Hazırlayan: {userName} | Kayıt Tarihi: {analysis.CreatedDate:dd MMMM yyyy HH:mm}").FontSize(10).FontColor(Colors.Grey.Medium);
                        if (!string.IsNullOrWhiteSpace(analysis.Notes))
                        {
                            c.Item().PaddingTop(4).Text($"Notlar: {analysis.Notes}").FontSize(10).Italic().FontColor(Colors.Grey.Darken1);
                        }
                    });

                    // Dataset Overview Box
                    if (dataset != null)
                    {
                        col.Item().Column(c =>
                        {
                            c.Item().Text("📁 İlişkili Veri Seti Özeti").FontSize(13).Bold().FontColor("#be185d");
                            c.Item().LineHorizontal(1).LineColor("#fbcfe8");

                            c.Item().PaddingTop(6).Row(row =>
                            {
                                row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten3).Padding(8).Column(b =>
                                {
                                    b.Item().Text("Toplam Satır").FontSize(9).FontColor(Colors.Grey.Medium);
                                    b.Item().Text($"{dataset.TotalRows:N0}").FontSize(14).Bold().FontColor("#371b2d");
                                });
                                row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten3).Padding(8).Column(b =>
                                {
                                    b.Item().Text("Toplam Kolon").FontSize(9).FontColor(Colors.Grey.Medium);
                                    b.Item().Text($"{dataset.TotalColumns}").FontSize(14).Bold().FontColor("#371b2d");
                                });
                                row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten3).Padding(8).Column(b =>
                                {
                                    b.Item().Text("Yükleme Tarihi").FontSize(9).FontColor(Colors.Grey.Medium);
                                    b.Item().Text($"{dataset.UploadedAt:dd.MM.yyyy}").FontSize(14).Bold().FontColor("#371b2d");
                                });
                            });
                        });

                        // Columns Table
                        if (dataset.Columns.Any())
                        {
                            col.Item().Column(c =>
                            {
                                c.Item().Text("📊 Kolon Detayları & İstatistiksel Özet").FontSize(13).Bold().FontColor("#be185d");
                                c.Item().LineHorizontal(1).LineColor("#fbcfe8");

                                c.Item().PaddingTop(6).Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(3); // Name
                                        columns.RelativeColumn(2); // Type
                                        columns.RelativeColumn(2); // Null
                                        columns.RelativeColumn(2); // Unique
                                        columns.RelativeColumn(2); // Min
                                        columns.RelativeColumn(2); // Max
                                        columns.RelativeColumn(2); // Avg
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Element(CellStyle).Text("Kolon Adı").Bold();
                                        header.Cell().Element(CellStyle).Text("Tip").Bold();
                                        header.Cell().Element(CellStyle).Text("Null").Bold();
                                        header.Cell().Element(CellStyle).Text("Tekil").Bold();
                                        header.Cell().Element(CellStyle).Text("Min").Bold();
                                        header.Cell().Element(CellStyle).Text("Max").Bold();
                                        header.Cell().Element(CellStyle).Text("Ortalama").Bold();

                                        static IContainer CellStyle(IContainer container) =>
                                            container.DefaultTextStyle(x => x.FontSize(8).FontColor(Colors.White))
                                                     .Background("#be185d")
                                                     .PaddingVertical(4)
                                                     .PaddingHorizontal(4);
                                    });

                                    foreach (var dCol in dataset.Columns)
                                    {
                                        table.Cell().Element(RowStyle).Text(dCol.ColumnName);
                                        table.Cell().Element(RowStyle).Text(dCol.DataType.ToString());
                                        table.Cell().Element(RowStyle).Text(dCol.NullCount.ToString());
                                        table.Cell().Element(RowStyle).Text(dCol.UniqueCount.ToString());
                                        table.Cell().Element(RowStyle).Text(dCol.MinValue.HasValue ? dCol.MinValue.Value.ToString("0.##") : "-");
                                        table.Cell().Element(RowStyle).Text(dCol.MaxValue.HasValue ? dCol.MaxValue.Value.ToString("0.##") : "-");
                                        table.Cell().Element(RowStyle).Text(dCol.AverageValue.HasValue ? dCol.AverageValue.Value.ToString("0.##") : "-");

                                        static IContainer RowStyle(IContainer container) =>
                                            container.BorderBottom(1)
                                                     .BorderColor(Colors.Grey.Lighten3)
                                                     .PaddingVertical(3)
                                                     .PaddingHorizontal(4)
                                                     .DefaultTextStyle(x => x.FontSize(8));
                                    }
                                });
                            });
                        }
                    }

                    // Analysis Configuration & Result Snapshot Parsing
                    if (!string.IsNullOrWhiteSpace(analysis.ResultJson) && analysis.ResultJson != "{}")
                    {
                        try
                        {
                            using var jsonDoc = System.Text.Json.JsonDocument.Parse(analysis.ResultJson);
                            var root = jsonDoc.RootElement;

                            // 1. AI Prediction Section in PDF
                            if (root.TryGetProperty("aiPredictionResult", out var aiElem))
                            {
                                col.Item().Column(c =>
                                {
                                    c.Spacing(4);
                                    c.Item().Text("🧠 Yapay Zeka (AI) Tahmin Sonuçları").FontSize(13).Bold().FontColor("#be185d");
                                    c.Item().LineHorizontal(1).LineColor("#fbcfe8");

                                    var targetCol = aiElem.TryGetProperty("targetColumn", out var tc) ? tc.GetString() : "-";
                                    var predVal = aiElem.TryGetProperty("predictedValue", out var pv) ? pv.GetDouble().ToString("N2") : "-";
                                    var r2 = aiElem.TryGetProperty("r2Score", out var r2Elem) ? (r2Elem.GetDouble() * 100).ToString("F1") : "-";
                                    var mae = aiElem.TryGetProperty("meanAbsoluteError", out var maeElem) ? maeElem.GetDouble().ToString("N2") : "-";
                                    var rmse = aiElem.TryGetProperty("rootMeanSquaredError", out var rmseElem) ? rmseElem.GetDouble().ToString("N2") : "-";
                                    var modelName = aiElem.TryGetProperty("modelName", out var mn) ? mn.GetString() : "ML Regresyon";

                                    c.Item().PaddingTop(4).Background("#fdf2f8").Border(1).BorderColor("#fbcfe8").Padding(10).Row(r =>
                                    {
                                        r.RelativeItem().Column(rc =>
                                        {
                                            rc.Item().Text("Tahmin Edilen Değer").FontSize(9).FontColor(Colors.Grey.Medium);
                                            rc.Item().Text(predVal).FontSize(16).Bold().FontColor("#be185d");
                                            rc.Item().Text($"Hedef: {targetCol}").FontSize(8).FontColor(Colors.Grey.Darken2);
                                        });
                                        r.RelativeItem().Column(rc =>
                                        {
                                            rc.Item().Text("Model Güven Skoru (R²)").FontSize(9).FontColor(Colors.Grey.Medium);
                                            rc.Item().Text($"%{r2}").FontSize(14).Bold().FontColor("#371b2d");
                                            rc.Item().Text(modelName).FontSize(8).FontColor(Colors.Grey.Darken2);
                                        });
                                        r.RelativeItem().Column(rc =>
                                        {
                                            rc.Item().Text("Hata Sapması (MAE / RMSE)").FontSize(9).FontColor(Colors.Grey.Medium);
                                            rc.Item().Text($"{mae} / {rmse}").FontSize(14).Bold().FontColor("#371b2d");
                                            rc.Item().Text("Doğruluk Sapması").FontSize(8).FontColor(Colors.Grey.Darken2);
                                        });
                                    });

                                    // Feature weights
                                    if (aiElem.TryGetProperty("featureWeights", out var fwArray) && fwArray.GetArrayLength() > 0)
                                    {
                                        c.Item().PaddingTop(4).Text("Özellik Ağırlıkları (Feature Importance):").FontSize(10).Bold().FontColor("#be185d");
                                        foreach (var fw in fwArray.EnumerateArray())
                                        {
                                            var fName = fw.TryGetProperty("featureName", out var fn) ? fn.GetString() : "";
                                            var imp = fw.TryGetProperty("importancePercent", out var ip) ? ip.GetDouble().ToString("F1") : "0";
                                            c.Item().Row(fr =>
                                            {
                                                fr.ConstantItem(120).Text(fName).FontSize(9).Bold();
                                                fr.RelativeItem().Text($"%{imp} Etki Oranı").FontSize(9).FontColor(Colors.Grey.Darken1);
                                            });
                                        }
                                    }
                                });
                            }

                            // 2. ML Forecast Section in PDF
                            if (root.TryGetProperty("forecastResult", out var fcElem))
                            {
                                col.Item().Column(c =>
                                {
                                    c.Spacing(4);
                                    c.Item().Text("🤖 ML Zaman Serisi & Gelecek Tahmin Sonuçları").FontSize(13).Bold().FontColor("#be185d");
                                    c.Item().LineHorizontal(1).LineColor("#fbcfe8");

                                    var targetCol = fcElem.TryGetProperty("targetColumn", out var tc) ? tc.GetString() : "-";
                                    var trend = fcElem.TryGetProperty("trendDirection", out var td) ? td.GetString() : "-";
                                    var r2 = fcElem.TryGetProperty("rSquared", out var r2Elem) ? (r2Elem.GetDouble() * 100).ToString("F1") : "-";
                                    var slope = fcElem.TryGetProperty("slope", out var slElem) ? slElem.GetDouble().ToString("F4") : "-";

                                    c.Item().PaddingTop(4).Background("#fdf2f8").Border(1).BorderColor("#fbcfe8").Padding(10).Row(r =>
                                    {
                                        r.RelativeItem().Column(rc =>
                                        {
                                            rc.Item().Text("İncelenen Kolon").FontSize(9).FontColor(Colors.Grey.Medium);
                                            rc.Item().Text(targetCol).FontSize(14).Bold().FontColor("#be185d");
                                            rc.Item().Text(trend).FontSize(8).FontColor(Colors.Grey.Darken2);
                                        });
                                        r.RelativeItem().Column(rc =>
                                        {
                                            rc.Item().Text("Model Uyumu (R²)").FontSize(9).FontColor(Colors.Grey.Medium);
                                            rc.Item().Text($"%{r2}").FontSize(14).Bold().FontColor("#371b2d");
                                        });
                                        r.RelativeItem().Column(rc =>
                                        {
                                            rc.Item().Text("Trend Eğimi (Slope)").FontSize(9).FontColor(Colors.Grey.Medium);
                                            rc.Item().Text(slope).FontSize(14).Bold().FontColor("#371b2d");
                                        });
                                    });

                                    // Forecast steps
                                    if (fcElem.TryGetProperty("forecastedValues", out var fVals) && fVals.GetArrayLength() > 0)
                                    {
                                        c.Item().PaddingTop(4).Text("Adım Adım Gelecek Kestirimleri:").FontSize(10).Bold().FontColor("#be185d");
                                        int step = 1;
                                        foreach (var fv in fVals.EnumerateArray())
                                        {
                                            c.Item().Row(fr =>
                                            {
                                                fr.ConstantItem(100).Text($"Adım +{step}").FontSize(9).Bold();
                                                fr.RelativeItem().Text($"{fv.GetDouble():N2}").FontSize(9).FontColor("#be185d").Bold();
                                            });
                                            step++;
                                        }
                                    }
                                });
                            }
                        }
                        catch { }
                    }
                });

                // Footer
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("InsightHub İş Zekası ve Analitik Platformu - Sayfa ");
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
        });

        using var ms = new MemoryStream();
        document.GeneratePdf(ms);
        return ms.ToArray();
    }
}
