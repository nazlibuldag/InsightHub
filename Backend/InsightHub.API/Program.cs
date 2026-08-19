using InsightHub.API.Middleware;
using InsightHub.Application.Common.Behaviors;
using InsightHub.Application.Interfaces;
using InsightHub.Infrastructure.Data.Contexts;
using InsightHub.Infrastructure.Repositories;
using InsightHub.Infrastructure.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("InsightHub.Infrastructure").EnableRetryOnFailure()));

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(InsightHub.Application.Features.Datasets.Queries.GetAllDatasets.GetAllDatasetsQuery).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
});

builder.Services.AddScoped<IDatasetRepository, DatasetRepository>();
builder.Services.AddScoped<IDatasetRowRepository, DatasetRowRepository>();
builder.Services.AddScoped<IDatasetColumnRepository, DatasetColumnRepository>();
builder.Services.AddScoped<IDatasetColumnValueRepository, DatasetColumnValueRepository>();
builder.Services.AddScoped<ICsvReaderService, CsvReaderService>();
builder.Services.AddScoped<IExcelReaderService, ExcelReaderService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<IColumnAnalysisService, ColumnAnalysisService>();
builder.Services.AddScoped<IExcelColumnAnalysisService, ExcelColumnAnalysisService>();
builder.Services.AddScoped<IDatasetRowService, DatasetRowService>();
builder.Services.AddScoped<IExcelDatasetRowService, ExcelDatasetRowService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<IMlForecastingService, MlForecastingService>();
builder.Services.AddScoped<IAnomalyDetectionService, AnomalyDetectionService>();
builder.Services.AddScoped<IDataCleaningService, DataCleaningService>();
builder.Services.AddScoped<ISavedAnalysisRepository, SavedAnalysisRepository>();
builder.Services.AddScoped<IWorkspaceRepository, WorkspaceRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddHttpClient<IAiAnalysisService, AiAnalysisService>();
builder.Services.AddScoped<IPdfReportService, PdfReportService>();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddSignalR();
builder.Services.AddSingleton<IHubNotificationService, HubNotificationService<InsightHub.API.Hubs.AnalysisHub>>();

var jwtSecret = builder.Configuration["JwtSettings:Secret"] ?? "InsightHub_Super_Secret_Key_For_Jwt_Token_Generation_2026!";
var jwtIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "InsightHubAPI";
var jwtAudience = builder.Configuration["JwtSettings:Audience"] ?? "InsightHubWeb";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSecret)),
        RoleClaimType = System.Security.Claims.ClaimTypes.Role
    };
});

builder.Services.AddControllers();

// Configure CORS for Frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",
                "http://localhost:5174",
                "http://127.0.0.1:5174",
                "https://localhost:5173",
                "https://localhost:5174")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();

        dbContext.Database.ExecuteSqlRaw(@"
            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SavedAnalyses')
            BEGIN
                CREATE TABLE SavedAnalyses (
                    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
                    UserId UNIQUEIDENTIFIER NOT NULL,
                    DatasetId UNIQUEIDENTIFIER NOT NULL,
                    Title NVARCHAR(MAX) NOT NULL,
                    Notes NVARCHAR(MAX) NOT NULL,
                    FilterJson NVARCHAR(MAX) NOT NULL,
                    CreatedDate DATETIME2 NOT NULL,
                    UpdatedDate DATETIME2 NULL
                );
            END;

            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Workspaces')
            BEGIN
                CREATE TABLE Workspaces (
                    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
                    Name NVARCHAR(MAX) NOT NULL,
                    Description NVARCHAR(MAX) NOT NULL,
                    OwnerId UNIQUEIDENTIFIER NOT NULL,
                    CreatedDate DATETIME2 NOT NULL,
                    UpdatedDate DATETIME2 NULL
                );
            END;

            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'WorkspaceMembers')
            BEGIN
                CREATE TABLE WorkspaceMembers (
                    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
                    WorkspaceId UNIQUEIDENTIFIER NOT NULL,
                    UserId UNIQUEIDENTIFIER NOT NULL,
                    Role NVARCHAR(MAX) NOT NULL,
                    CreatedDate DATETIME2 NOT NULL,
                    UpdatedDate DATETIME2 NULL
                );
            END;

            IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AuditLogs')
            BEGIN
                CREATE TABLE AuditLogs (
                    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
                    UserId UNIQUEIDENTIFIER NULL,
                    UserEmail NVARCHAR(MAX) NOT NULL,
                    Action NVARCHAR(MAX) NOT NULL,
                    EntityName NVARCHAR(MAX) NOT NULL,
                    EntityId NVARCHAR(MAX) NOT NULL,
                    IpAddress NVARCHAR(MAX) NOT NULL,
                    Details NVARCHAR(MAX) NOT NULL,
                    Timestamp DATETIME2 NOT NULL,
                    CreatedDate DATETIME2 NOT NULL,
                    UpdatedDate DATETIME2 NULL
                );
            END;
        ");
        // Seed Default Admin and Users if not exists
        var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        var adminUser = userRepo.GetByEmailAsync("admin@insighthub.com").GetAwaiter().GetResult();
        if (adminUser == null)
        {
            userRepo.AddAsync(new InsightHub.Domain.Entities.User
            {
                Id = Guid.NewGuid(),
                FirstName = "Sistem",
                LastName = "Yöneticisi",
                Email = "admin@insighthub.com",
                PasswordHash = hasher.HashPassword("Password123!"),
                Role = InsightHub.Domain.Enums.UserRole.Admin,
                IsActive = true
            }).GetAwaiter().GetResult();
        }
        else if (adminUser.Role != InsightHub.Domain.Enums.UserRole.Admin)
        {
            adminUser.Role = InsightHub.Domain.Enums.UserRole.Admin;
            userRepo.UpdateAsync(adminUser).GetAwaiter().GetResult();
        }

        var nazliUser = userRepo.GetByEmailAsync("nazli@insighthub.com").GetAwaiter().GetResult();
        if (nazliUser == null)
        {
            userRepo.AddAsync(new InsightHub.Domain.Entities.User
            {
                Id = Guid.NewGuid(),
                FirstName = "Nazlı",
                LastName = "Buldağ",
                Email = "nazli@insighthub.com",
                PasswordHash = hasher.HashPassword("Password123!"),
                Role = InsightHub.Domain.Enums.UserRole.Admin,
                IsActive = true
            }).GetAwaiter().GetResult();
        }
        else if (nazliUser.Role != InsightHub.Domain.Enums.UserRole.Admin)
        {
            nazliUser.Role = InsightHub.Domain.Enums.UserRole.Admin;
            userRepo.UpdateAsync(nazliUser).GetAwaiter().GetResult();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"DB Init Error: {ex.Message}");
    }
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("FrontendPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();
app.MapHub<InsightHub.API.Hubs.AnalysisHub>("/hubs/analysis");

app.Run();
