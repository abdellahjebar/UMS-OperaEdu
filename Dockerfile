# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["UMS solution.sln", "./"]
COPY ["UMS.api/UMS.API.csproj", "UMS.api/"]
COPY ["UMS.Application/UMS.Application.csproj", "UMS.Application/"]
COPY ["UMS.core/UMS.Core.csproj", "UMS.core/"]
COPY ["UMS.Infrastructure/UMS.Infrastructure.csproj", "UMS.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "UMS solution.sln"

# Copy all source files
COPY . .

# Build and publish
WORKDIR "/src/UMS.api"
RUN dotnet build "UMS.API.csproj" -c Release -o /app/build
RUN dotnet publish "UMS.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy published files
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "UMS.API.dll"]
