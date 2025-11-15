using FluentValidation;

namespace UMS.Application.Features.Sections.Commands.CreateSection
{
    public class CreateSectionCommandValidator : AbstractValidator<CreateSectionCommand>
    {
        public CreateSectionCommandValidator()
        {
            RuleFor(x => x.CourseId)
                .NotEmpty().WithMessage("Course ID is required.");

            RuleFor(x => x.SectionNumber)
                .NotEmpty().WithMessage("Section number is required.")
                .MaximumLength(10).WithMessage("Section number must not exceed 10 characters.");

            RuleFor(x => x.Year)
                .GreaterThan(2000).WithMessage("Year must be greater than 2000.")
                .LessThanOrEqualTo(DateTime.UtcNow.Year + 2).WithMessage("Year cannot be more than 2 years in the future.");

            RuleFor(x => x.InstructorId)
                .NotEmpty().WithMessage("Instructor ID is required.");

            RuleFor(x => x.MaxCapacity)
                .GreaterThan(0).WithMessage("Max capacity must be greater than 0.")
                .LessThanOrEqualTo(500).WithMessage("Max capacity cannot exceed 500.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required.")
                .LessThan(x => x.EndDate).WithMessage("Start date must be before end date.");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date is required.");
        }
    }
}
