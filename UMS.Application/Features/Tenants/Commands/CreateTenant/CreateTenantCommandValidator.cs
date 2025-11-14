using FluentValidation;

namespace UMS.Application.Features.Tenants.Commands.CreateTenant
{
    public class CreateTenantCommandValidator : AbstractValidator<CreateTenantCommand>
    {
        public CreateTenantCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("School name is required.")
                .MaximumLength(200).WithMessage("School name cannot exceed 200 characters.");

            RuleFor(x => x.Subdomain)
                .NotEmpty().WithMessage("Subdomain is required.")
                .MaximumLength(50).WithMessage("Subdomain cannot exceed 50 characters.")
                .Matches("^[a-z0-9-]+$").WithMessage("Subdomain can only contain lowercase letters, numbers, and hyphens.")
                .Must(subdomain => subdomain != "admin" && subdomain != "www" && subdomain != "api")
                .WithMessage("This subdomain is reserved.");

            RuleFor(x => x.AnnualFee)
                .GreaterThanOrEqualTo(0).WithMessage("Annual fee cannot be negative.");

            RuleFor(x => x.AdminEmail)
                .EmailAddress().When(x => !string.IsNullOrEmpty(x.AdminEmail))
                .WithMessage("Invalid email format.");

            RuleFor(x => x.MaxStudents)
                .GreaterThan(0).WithMessage("Max students must be greater than 0.");

            RuleFor(x => x.MaxFaculty)
                .GreaterThan(0).WithMessage("Max faculty must be greater than 0.");

            RuleFor(x => x.MaxCourses)
                .GreaterThan(0).WithMessage("Max courses must be greater than 0.");
        }
    }
}
