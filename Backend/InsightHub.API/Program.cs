using FluentValidation;
using InsightHub.API.Middleware;
using InsightHub.Application.Features.Datasets.Queries.GetDatasetRows;
using InsightHub.Application.Interfaces;
using InsightHub.Infrastructure.Data.Contexts;
using InsightHub.Infrastructure.Repositories;
using InsightHub.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System.Reflection;


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

builder.Services.AddScoped<IExcelDatasetRowService, ExcelDatasetRowService>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(
        typeof(InsightHub.Application.Features.Datasets.Commands.CreateDataset.CreateDatasetCommand).Assembly);

    cfg.AddOpenBehavior(
        typeof(InsightHub.Application.Common.Behaviors.ValidationBehavior<,>));
});
builder.Services.AddScoped<
    IValidator<GetDatasetRowsQuery>,
    GetDatasetRowsQueryValidator>();


builder.Services.AddValidatorsFromAssembly(
    typeof(GetDatasetRowsQueryValidator).Assembly);


builder.Services.AddControllers();

// Configure CORS for Frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "http://localhost:5174",
                "https://localhost:5173",
                "https://localhost:5174")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("FrontendPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
