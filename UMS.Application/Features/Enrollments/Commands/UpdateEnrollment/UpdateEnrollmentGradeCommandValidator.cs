using FluentValidation;

namespace UMS.Application.Features.Enrollments.Commands.UpdateEnrollment
{
    public class UpdateEnrollmentGradeCommandValidator : AbstractValidator<UpdateEnrollmentGradeCommand>
    {
        public UpdateEnrollmentGradeCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Enrollment ID is required");

            RuleFor(x => x.NumericGrade)
                .GreaterThanOrEqualTo(0).WithMessage("Numeric grade must be 0 or greater")
                .LessThanOrEqualTo(100).WithMessage("Numeric grade cannot exceed 100");

            RuleFor(x => x.GradePoints)
                .GreaterThanOrEqualTo(0).WithMessage("Grade points must be 0 or greater")
                .LessThanOrEqualTo(4).WithMessage("Grade points cannot exceed 4.0");
        }
    }
}
