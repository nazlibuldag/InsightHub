using InsightHub.Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using InsightHub.Application.Interfaces;
using InsightHub.Infrastructure.Repositories;
using InsightHub.Infrastructure.Services;


var builder = WebApplication.CreateBuilder(args);



builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IDatasetRepository, DatasetRepository>();
builder.Services.AddScoped<IDatasetColumnRepository, DatasetColumnRepository>();
builder.Services.AddScoped<ICsvReaderService, CsvReaderService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<IColumnAnalysisService, ColumnAnalysisService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<IExcelReaderService, ExcelReaderService>();
builder.Services.AddScoped<IExcelColumnAnalysisService, ExcelColumnAnalysisService>();
builder.Services.AddScoped<IDatasetColumnValueRepository, DatasetColumnValueRepository>();

builder.Services.AddScoped<IDatasetRowRepository, DatasetRowRepository>();

builder.Services.AddScoped<IDatasetRowService, DatasetRowService>();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(InsightHub.Application.Features.Datasets.Commands.CreateDataset.CreateDatasetCommand).Assembly));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
