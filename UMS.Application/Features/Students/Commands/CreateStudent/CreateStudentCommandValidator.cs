using FluentValidation;

namespace UMS.Application.Features.Students.Commands.CreateStudent
{
    public class CreateStudentCommandValidator : AbstractValidator<CreateStudentCommand>
    {
        public CreateStudentCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Password));

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required.")
                .LessThan(System.DateTime.Now).WithMessage("Date of birth must be in the past.");

            RuleFor(x => x.EnrollmentDate)
                .NotEmpty().WithMessage("Enrollment date is required.");

            RuleFor(x => x.ProgramId)
                .NotEmpty().WithMessage("Program ID is required.");

            RuleFor(x => x.AcademicStatus)
                .IsInEnum().WithMessage("Invalid academic status provided.");

            RuleFor(x => x.GPA)
                .InclusiveBetween(0m, 4m).WithMessage("GPA must be between 0.0 and 4.0.");

            RuleFor(x => x.TotalCredits)
                .GreaterThanOrEqualTo(0).WithMessage("Total credits cannot be negative.");
        }
    }
}
