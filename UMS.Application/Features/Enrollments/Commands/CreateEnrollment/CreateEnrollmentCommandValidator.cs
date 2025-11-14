using FluentValidation;

namespace UMS.Application.Features.Enrollments.Commands.CreateEnrollment
{
    public class CreateEnrollmentCommandValidator : AbstractValidator<CreateEnrollmentCommand>
    {
        public CreateEnrollmentCommandValidator()
        {
            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("Student ID is required");

            RuleFor(x => x.SectionId)
                .NotEmpty().WithMessage("Section ID is required");
        }
    }
}
