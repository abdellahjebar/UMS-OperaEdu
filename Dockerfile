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

# Install SQL Server tools (for migrations if needed)
RUN apt-get update && apt-get install -y curl apt-transport-https && \
    curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - && \
    curl https://packages.microsoft.com/config/ubuntu/22.04/prod.list > /etc/apt/sources.list.d/mssql-release.list && \
    apt-get update && \
    ACCEPT_EULA=Y apt-get install -y msodbcsql18 mssql-tools18 && \
    echo 'export PATH="$PATH:/opt/mssql-tools18/bin"' >> ~/.bashrc && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

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
