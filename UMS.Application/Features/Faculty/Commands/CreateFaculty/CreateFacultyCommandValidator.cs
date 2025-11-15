using FluentValidation;

namespace UMS.Application.Features.Faculty.Commands.CreateFaculty
{
    public class CreateFacultyCommandValidator : AbstractValidator<CreateFacultyCommand>
    {
        public CreateFacultyCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email must be a valid email address.")
                .MaximumLength(100).WithMessage("Email must not exceed 100 characters.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters.")
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));

            RuleFor(x => x.EmployeeNumber)
                .MaximumLength(20).WithMessage("Employee number must not exceed 20 characters.")
                .When(x => !string.IsNullOrEmpty(x.EmployeeNumber));

            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("Department ID is required.");

            RuleFor(x => x.HireDate)
                .NotEmpty().WithMessage("Hire date is required.")
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Hire date cannot be in the future.");

            RuleFor(x => x.OfficeLocation)
                .MaximumLength(100).WithMessage("Office location must not exceed 100 characters.")
                .When(x => !string.IsNullOrEmpty(x.OfficeLocation));

            RuleFor(x => x.OfficeHours)
                .MaximumLength(200).WithMessage("Office hours must not exceed 200 characters.")
                .When(x => !string.IsNullOrEmpty(x.OfficeHours));
        }
    }
}
