using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using UMS.API.Models;
using UMS.Core.Exceptions;

namespace UMS.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, errorResponse) = BuildErrorResponse(context, exception);
            context.Response.StatusCode = statusCode;

            var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            });

            return context.Response.WriteAsync(jsonResponse);
        }

        private static (int statusCode, ErrorResponse response) BuildErrorResponse(HttpContext context, Exception exception)
        {
            var traceId = context.TraceIdentifier;

            return exception switch
            {
                ValidationException validationEx =>
                    (StatusCodes.Status400BadRequest,
                        new ErrorResponse
                        {
                            StatusCode = StatusCodes.Status400BadRequest,
                            Title = "Validation failed",
                            Detail = "One or more validation errors occurred.",
                            TraceId = traceId,
                            Errors = validationEx.Errors
                                .GroupBy(e => e.PropertyName)
                                .ToDictionary(
                                    g => g.Key,
                                    g => g.Select(e => e.ErrorMessage).Distinct().ToArray())
                        }),
                BadRequestException badRequestEx => BuildSimpleResponse(StatusCodes.Status400BadRequest, "Bad request", badRequestEx.Message, traceId),
                InvalidOperationException invalidOpEx => BuildSimpleResponse(StatusCodes.Status400BadRequest, "Bad request", invalidOpEx.Message, traceId),
                UnauthorizedException unauthorizedEx => BuildSimpleResponse(StatusCodes.Status401Unauthorized, "Unauthorized", unauthorizedEx.Message, traceId),
                UnauthorizedAccessException => BuildSimpleResponse(StatusCodes.Status401Unauthorized, "Unauthorized", "Unauthorized access", traceId),
                NotFoundException notFoundEx => BuildSimpleResponse(StatusCodes.Status404NotFound, "Resource not found", notFoundEx.Message, traceId),
                KeyNotFoundException keyNotFoundEx => BuildSimpleResponse(StatusCodes.Status404NotFound, "Resource not found", keyNotFoundEx.Message, traceId),
                _ => BuildSimpleResponse(StatusCodes.Status500InternalServerError, "Internal server error", "An unexpected error occurred while processing your request.", traceId)
            };
        }

        private static (int statusCode, ErrorResponse response) BuildSimpleResponse(int statusCode, string title, string detail, string traceId)
        {
            return (statusCode, new ErrorResponse
            {
                StatusCode = statusCode,
                Title = title,
                Detail = detail,
                TraceId = traceId
            });
        }
    }
}
