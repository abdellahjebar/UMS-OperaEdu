using FluentValidation;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace UMS.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            int statusCode;
            object response;

            switch (exception)
            {
                case ValidationException validationEx:
                    statusCode = StatusCodes.Status400BadRequest;
                    response = new
                    {
                        StatusCode = statusCode,
                        Message = "Validation failed",
                        Errors = validationEx.Errors.Select(e => new
                        {
                            Property = e.PropertyName,
                            Message = e.ErrorMessage
                        })
                    };
                    break;
                case InvalidOperationException invalidOpEx:
                    statusCode = StatusCodes.Status400BadRequest;
                    response = new
                    {
                        StatusCode = statusCode,
                        Message = invalidOpEx.Message
                    };
                    break;
                case KeyNotFoundException notFoundEx:
                    statusCode = StatusCodes.Status404NotFound;
                    response = new
                    {
                        StatusCode = statusCode,
                        Message = notFoundEx.Message
                    };
                    break;
                case UnauthorizedAccessException:
                    statusCode = StatusCodes.Status401Unauthorized;
                    response = new
                    {
                        StatusCode = statusCode,
                        Message = "Unauthorized access"
                    };
                    break;
                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    response = new
                    {
                        StatusCode = statusCode,
                        Message = "An error occurred while processing your request"
                    };
                    break;
            }

            context.Response.StatusCode = statusCode;

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return context.Response.WriteAsync(jsonResponse);
        }
    }
}
