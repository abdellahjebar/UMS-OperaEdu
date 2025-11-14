using MediatR;
using System;

namespace UMS.Application.Features.Tenants.Commands.CreateTenant
{
    public class CreateTenantCommand : IRequest<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string Subdomain { get; set; } = string.Empty;
        public decimal AnnualFee { get; set; }
        public string? AdminEmail { get; set; }
        public string? AdminPhone { get; set; }
        public string? Address { get; set; }
        public int MaxStudents { get; set; } = 10000;
        public int MaxFaculty { get; set; } = 500;
        public int MaxCourses { get; set; } = 1000;
        public DateTime? SubscriptionEndDate { get; set; }
    }
}
