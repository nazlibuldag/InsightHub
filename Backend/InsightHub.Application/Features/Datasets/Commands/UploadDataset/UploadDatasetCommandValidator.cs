using System;
using System.IO;
using FluentValidation;

namespace InsightHub.Application.Features.Datasets.Commands.UploadDataset;

public class UploadDatasetCommandValidator : AbstractValidator<UploadDatasetCommand>
{
    private static readonly string[] AllowedExtensions = { ".csv", ".xlsx", ".xls" };
    private const long MaxFileSizeInBytes = 52_428_800; // 50 MB

    public UploadDatasetCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Dataset adı boş bırakılamaz.")
            .MaximumLength(200).WithMessage("Dataset adı en fazla 200 karakter olabilir.");

        RuleFor(x => x.File)
            .NotNull().WithMessage("Lütfen bir dosya seçin.")
            .Must(file => file != null && file.Length > 0).WithMessage("Seçilen dosya boş veya geçersiz.")
            .Must(file => file != null && file.Length <= MaxFileSizeInBytes).WithMessage("Dosya boyutu en fazla 50MB olabilir.")
            .Must(file =>
            {
                if (file == null) return false;
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                return Array.Exists(AllowedExtensions, e => e == ext);
            }).WithMessage("Yalnızca .csv, .xlsx veya .xls formatındaki dosyalar desteklenmektedir.");
    }
}
