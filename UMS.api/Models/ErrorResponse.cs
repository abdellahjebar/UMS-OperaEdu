using System;
using System.Collections.Generic;

namespace UMS.API.Models
{
    /// <summary>
    /// Standard error contract returned by the API.
    /// </summary>
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
        public string TraceId { get; set; } = string.Empty;
        public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
        public IDictionary<string, string[]>? Errors { get; set; }
    }
}
