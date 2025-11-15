using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using UMS.API.Middleware;
using UMS.API.Models;
using UMS.Core.Entities.Tenants;
using UMS.Core.Interfaces;
using UMS.Core.Interfaces.IRepositories;
using UMS.Infrastructure.Services;
using Xunit;

namespace UMS.Api.Tests.Middleware
{
    public class TenantResolutionMiddlewareTests
    {
        private readonly Mock<ITenantRepository> _tenantRepositoryMock;
        private readonly Mock<ITenantService> _tenantServiceMock;
        private readonly Mock<IDatabaseInitializationService> _databaseInitServiceMock;
        private readonly Mock<RequestDelegate> _nextMock;
        private readonly Mock<ILogger<TenantResolutionMiddleware>> _loggerMock;
        private readonly TenantResolutionMiddleware _middleware;

        public TenantResolutionMiddlewareTests()
        {
            _tenantRepositoryMock = new Mock<ITenantRepository>();
            _tenantServiceMock = new Mock<ITenantService>();
            _databaseInitServiceMock = new Mock<IDatabaseInitializationService>();
            _nextMock = new Mock<RequestDelegate>();
            _loggerMock = new Mock<ILogger<TenantResolutionMiddleware>>();
            _middleware = new TenantResolutionMiddleware(_nextMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task InvokeAsync_NoSubdomain_Returns400WithErrorResponse()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Host = new HostString("domain.com");
            var responseStream = new MemoryStream();
            context.Response.Body = responseStream;

            // Act
            await _middleware.InvokeAsync(
                context,
                _tenantRepositoryMock.Object,
                _tenantServiceMock.Object,
                _databaseInitServiceMock.Object);

            // Assert
            context.Response.StatusCode.Should().Be(400);
            responseStream.Position = 0;
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var responseBody = await JsonSerializer.DeserializeAsync<ErrorResponse>(responseStream, options);
            responseBody.Should().NotBeNull();
            responseBody!.StatusCode.Should().Be(400);
            responseBody.Title.Should().Be("Invalid request");
            responseBody.Detail.Should().Contain("No subdomain found");
        }

        [Fact]
        public async Task InvokeAsync_AdminSubdomain_SetsSuperAdminContext()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Host = new HostString("admin.domain.com");

            // Act
            await _middleware.InvokeAsync(
                context,
                _tenantRepositoryMock.Object,
                _tenantServiceMock.Object,
                _databaseInitServiceMock.Object);

            // Assert
            _tenantServiceMock.Verify(x => x.SetSuperAdminContext(), Times.Once);
            _nextMock.Verify(x => x(context), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_TenantNotFound_Returns404WithErrorResponse()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Host = new HostString("nonexistent.domain.com");
            var responseStream = new MemoryStream();
            context.Response.Body = responseStream;

            _tenantRepositoryMock
                .Setup(x => x.GetBySubdomainAsync("nonexistent"))
                .ReturnsAsync((Tenant?)null);

            // Act
            await _middleware.InvokeAsync(
                context,
                _tenantRepositoryMock.Object,
                _tenantServiceMock.Object,
                _databaseInitServiceMock.Object);

            // Assert
            context.Response.StatusCode.Should().Be(404);
            responseStream.Position = 0;
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var responseBody = await JsonSerializer.DeserializeAsync<ErrorResponse>(responseStream, options);
            responseBody.Should().NotBeNull();
            responseBody!.StatusCode.Should().Be(404);
            responseBody.Title.Should().Be("Tenant not found");
            responseBody.Detail.Should().Contain("nonexistent");
        }

        [Fact]
        public async Task InvokeAsync_InactiveTenant_Returns403WithErrorResponse()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Host = new HostString("inactive.domain.com");
            var responseStream = new MemoryStream();
            context.Response.Body = responseStream;

            var inactiveTenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = "Inactive School",
                Subdomain = "inactive",
                ConnectionString = "Server=localhost;Database=UMS_Tenant_inactive;",
                IsActive = false,
                SubscriptionStartDate = DateTime.UtcNow.AddMonths(-6),
                SubscriptionEndDate = DateTime.UtcNow.AddMonths(-1)
            };

            _tenantRepositoryMock
                .Setup(x => x.GetBySubdomainAsync("inactive"))
                .ReturnsAsync(inactiveTenant);

            // Act
            await _middleware.InvokeAsync(
                context,
                _tenantRepositoryMock.Object,
                _tenantServiceMock.Object,
                _databaseInitServiceMock.Object);

            // Assert
            context.Response.StatusCode.Should().Be(403);
            responseStream.Position = 0;
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var responseBody = await JsonSerializer.DeserializeAsync<ErrorResponse>(responseStream, options);
            responseBody.Should().NotBeNull();
            responseBody!.StatusCode.Should().Be(403);
            responseBody.Title.Should().Be("Subscription inactive");
            responseBody.Detail.Should().Contain("not active");
        }

        [Fact]
        public async Task InvokeAsync_ValidTenant_SetsTenantContextAndCallsNext()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Host = new HostString("harvard.domain.com");

            var validTenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = "Harvard University",
                Subdomain = "harvard",
                ConnectionString = "Server=localhost;Database=UMS_Tenant_harvard;",
                IsActive = true,
                SubscriptionStartDate = DateTime.UtcNow.AddMonths(-6),
                SubscriptionEndDate = DateTime.UtcNow.AddMonths(6)
            };

            _tenantRepositoryMock
                .Setup(x => x.GetBySubdomainAsync("harvard"))
                .ReturnsAsync(validTenant);

            _databaseInitServiceMock
                .Setup(x => x.EnsureDatabaseCreatedAsync(validTenant))
                .Returns(Task.CompletedTask);

            // Act
            await _middleware.InvokeAsync(
                context,
                _tenantRepositoryMock.Object,
                _tenantServiceMock.Object,
                _databaseInitServiceMock.Object);

            // Assert
            _tenantServiceMock.Verify(
                x => x.SetTenantContext(validTenant.Id.ToString(), validTenant.ConnectionString),
                Times.Once);
            _databaseInitServiceMock.Verify(
                x => x.EnsureDatabaseCreatedAsync(validTenant),
                Times.Once);
            _nextMock.Verify(x => x(context), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_LocalhostWithSubdomain_ExtractsCorrectSubdomain()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Host = new HostString("testschool.localhost:5000");

            var validTenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = "Test School",
                Subdomain = "testschool",
                ConnectionString = "Server=localhost;Database=UMS_Tenant_testschool;",
                IsActive = true,
                SubscriptionStartDate = DateTime.UtcNow,
                SubscriptionEndDate = DateTime.UtcNow.AddYears(1)
            };

            _tenantRepositoryMock
                .Setup(x => x.GetBySubdomainAsync("testschool"))
                .ReturnsAsync(validTenant);

            _databaseInitServiceMock
                .Setup(x => x.EnsureDatabaseCreatedAsync(validTenant))
                .Returns(Task.CompletedTask);

            // Act
            await _middleware.InvokeAsync(
                context,
                _tenantRepositoryMock.Object,
                _tenantServiceMock.Object,
                _databaseInitServiceMock.Object);

            // Assert
            _tenantRepositoryMock.Verify(x => x.GetBySubdomainAsync("testschool"), Times.Once);
            _nextMock.Verify(x => x(context), Times.Once);
        }

        [Fact]
        public async Task InvokeAsync_PlainLocalhost_DefaultsToAdminSubdomain()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Host = new HostString("localhost:5000");

            // Act
            await _middleware.InvokeAsync(
                context,
                _tenantRepositoryMock.Object,
                _tenantServiceMock.Object,
                _databaseInitServiceMock.Object);

            // Assert
            _tenantServiceMock.Verify(x => x.SetSuperAdminContext(), Times.Once);
            _nextMock.Verify(x => x(context), Times.Once);
        }
    }
}
