# Build stage for Backend
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-backend
WORKDIR /src
COPY ["Backend/InsightHub.API/InsightHub.API.csproj", "Backend/InsightHub.API/"]
COPY ["Backend/InsightHub.Application/InsightHub.Application.csproj", "Backend/InsightHub.Application/"]
COPY ["Backend/InsightHub.Domain/InsightHub.Domain.csproj", "Backend/InsightHub.Domain/"]
COPY ["Backend/InsightHub.Infrastructure/InsightHub.Infrastructure.csproj", "Backend/InsightHub.Infrastructure/"]
RUN dotnet restore "Backend/InsightHub.API/InsightHub.API.csproj"

COPY Backend/ Backend/
WORKDIR "/src/Backend/InsightHub.API"
RUN dotnet publish "InsightHub.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage for API
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build-backend /app/publish .
ENV ASPNETCORE_URLS=http://+:5099
EXPOSE 5099
ENTRYPOINT ["dotnet", "InsightHub.API.dll"]
