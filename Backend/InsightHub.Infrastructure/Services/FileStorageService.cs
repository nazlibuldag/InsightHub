using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using InsightHub.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace InsightHub.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    public async Task<string> SaveFileAsync(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        var uploadsFolder = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Uploads");

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var fileName = $"{Guid.NewGuid()}_{file.FileName}";

        var filePath = Path.Combine(uploadsFolder, fileName);

        using var stream = new FileStream(filePath, FileMode.Create);

        await file.CopyToAsync(stream, cancellationToken);

        return fileName;

    }

    public Task DeleteFileAsync(string fileName)
    {
        var filePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "Uploads",
            fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }
}
