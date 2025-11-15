using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using UMS.API.Middleware;
using UMS.API.Models;
using UMS.Core.Exceptions;

namespace UMS.Api.Tests.Middleware;

public class ExceptionHandlingMiddlewareTests
{
    private readonly Mock<ILogger<ExceptionHandlingMiddleware>> _loggerMock;
    private readonly Mock<RequestDelegate> _nextMock;

    public ExceptionHandlingMiddlewareTests()
    {
        _loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        _nextMock = new Mock<RequestDelegate>();
    }

    [Fact]
    public async Task InvokeAsync_WhenNoException_ShouldCallNext()
    {
        // Arrange
        var middleware = new ExceptionHandlingMiddleware(_nextMock.Object, _loggerMock.Object);
        var context = new DefaultHttpContext();

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        _nextMock.Verify(next => next(context), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WhenValidationException_ShouldReturn400WithErrors()
    {
        // Arrange
        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Email", "Email is required."),
            new ValidationFailure("Email", "Email must be a valid email address."),
            new ValidationFailure("Password", "Password must be at least 8 characters.")
        };
        var validationException = new ValidationException(validationFailures);

        _nextMock.Setup(next => next(It.IsAny<HttpContext>()))
            .ThrowsAsync(validationException);

        var middleware = new ExceptionHandlingMiddleware(_nextMock.Object, _loggerMock.Object);
        var context = new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() }
        };

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        context.Response.ContentType.Should().Be("application/json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        errorResponse.Should().NotBeNull();
        errorResponse!.StatusCode.Should().Be(400);
        errorResponse.Title.Should().Be("Validation failed");
        errorResponse.Detail.Should().Be("One or more validation errors occurred.");
        errorResponse.Errors.Should().NotBeNull();
        errorResponse.Errors.Should().ContainKey("Email");
        errorResponse.Errors!["Email"].Should().HaveCount(2);
        errorResponse.Errors.Should().ContainKey("Password");
        errorResponse.Errors["Password"].Should().HaveCount(1);
    }

    [Fact]
    public async Task InvokeAsync_WhenNotFoundException_ShouldReturn404()
    {
        // Arrange
        var notFoundException = new NotFoundException("Student with ID 123 not found");

        _nextMock.Setup(next => next(It.IsAny<HttpContext>()))
            .ThrowsAsync(notFoundException);

        var middleware = new ExceptionHandlingMiddleware(_nextMock.Object, _loggerMock.Object);
        var context = new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() }
        };

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        context.Response.ContentType.Should().Be("application/json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        errorResponse.Should().NotBeNull();
        errorResponse!.StatusCode.Should().Be(404);
        errorResponse.Title.Should().Be("Resource not found");
        errorResponse.Detail.Should().Be("Student with ID 123 not found");
        errorResponse.Errors.Should().BeNull();
    }

    [Fact]
    public async Task InvokeAsync_WhenUnauthorizedException_ShouldReturn401()
    {
        // Arrange
        var unauthorizedException = new UnauthorizedException("Invalid credentials");

        _nextMock.Setup(next => next(It.IsAny<HttpContext>()))
            .ThrowsAsync(unauthorizedException);

        var middleware = new ExceptionHandlingMiddleware(_nextMock.Object, _loggerMock.Object);
        var context = new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() }
        };

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        context.Response.ContentType.Should().Be("application/json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        errorResponse.Should().NotBeNull();
        errorResponse!.StatusCode.Should().Be(401);
        errorResponse.Title.Should().Be("Unauthorized");
        errorResponse.Detail.Should().Be("Invalid credentials");
        errorResponse.Errors.Should().BeNull();
    }

    [Fact]
    public async Task InvokeAsync_WhenBadRequestException_ShouldReturn400()
    {
        // Arrange
        var badRequestException = new BadRequestException("Email already exists");

        _nextMock.Setup(next => next(It.IsAny<HttpContext>()))
            .ThrowsAsync(badRequestException);

        var middleware = new ExceptionHandlingMiddleware(_nextMock.Object, _loggerMock.Object);
        var context = new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() }
        };

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        context.Response.ContentType.Should().Be("application/json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        errorResponse.Should().NotBeNull();
        errorResponse!.StatusCode.Should().Be(400);
        errorResponse.Title.Should().Be("Bad request");
        errorResponse.Detail.Should().Be("Email already exists");
        errorResponse.Errors.Should().BeNull();
    }

    [Fact]
    public async Task InvokeAsync_WhenUnhandledException_ShouldReturn500()
    {
        // Arrange
        var exception = new Exception("Unexpected error");

        _nextMock.Setup(next => next(It.IsAny<HttpContext>()))
            .ThrowsAsync(exception);

        var middleware = new ExceptionHandlingMiddleware(_nextMock.Object, _loggerMock.Object);
        var context = new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() }
        };

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        context.Response.ContentType.Should().Be("application/json");

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        errorResponse.Should().NotBeNull();
        errorResponse!.StatusCode.Should().Be(500);
        errorResponse.Title.Should().Be("Internal server error");
        errorResponse.Detail.Should().Be("An unexpected error occurred while processing your request.");
        errorResponse.Errors.Should().BeNull();
    }

    [Fact]
    public async Task InvokeAsync_WhenExceptionThrown_ShouldLogError()
    {
        // Arrange
        var exception = new Exception("Test exception");

        _nextMock.Setup(next => next(It.IsAny<HttpContext>()))
            .ThrowsAsync(exception);

        var middleware = new ExceptionHandlingMiddleware(_nextMock.Object, _loggerMock.Object);
        var context = new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() }
        };

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        _loggerMock.Verify(
            logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WhenException_ShouldIncludeTraceId()
    {
        // Arrange
        var exception = new NotFoundException("Test not found");

        _nextMock.Setup(next => next(It.IsAny<HttpContext>()))
            .ThrowsAsync(exception);

        var middleware = new ExceptionHandlingMiddleware(_nextMock.Object, _loggerMock.Object);
        var context = new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() },
            TraceIdentifier = "test-trace-id-12345"
        };

        // Act
        await middleware.InvokeAsync(context);

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        errorResponse.Should().NotBeNull();
        errorResponse!.TraceId.Should().Be("test-trace-id-12345");
    }

    [Fact]
    public async Task InvokeAsync_WhenException_ShouldIncludeTimestamp()
    {
        // Arrange
        var exception = new NotFoundException("Test not found");

        _nextMock.Setup(next => next(It.IsAny<HttpContext>()))
            .ThrowsAsync(exception);

        var middleware = new ExceptionHandlingMiddleware(_nextMock.Object, _loggerMock.Object);
        var context = new DefaultHttpContext
        {
            Response = { Body = new MemoryStream() }
        };

        var beforeTime = DateTime.UtcNow;

        // Act
        await middleware.InvokeAsync(context);

        var afterTime = DateTime.UtcNow;

        // Assert
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseBody = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        errorResponse.Should().NotBeNull();
        errorResponse!.TimestampUtc.Should().BeAfter(beforeTime.AddSeconds(-1));
        errorResponse.TimestampUtc.Should().BeBefore(afterTime.AddSeconds(1));
    }
}
