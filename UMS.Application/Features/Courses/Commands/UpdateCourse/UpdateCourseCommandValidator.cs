using FluentValidation;

namespace UMS.Application.Features.Courses.Commands.UpdateCourse
{
    public class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
    {
        public UpdateCourseCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Course ID is required");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Course code is required")
                .MaximumLength(20).WithMessage("Course code cannot exceed 20 characters");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Course name is required")
                .MaximumLength(200).WithMessage("Course name cannot exceed 200 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Course description is required")
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

            RuleFor(x => x.Credits)
                .GreaterThan(0).WithMessage("Credits must be greater than 0")
                .LessThanOrEqualTo(12).WithMessage("Credits cannot exceed 12");

            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("Department ID is required");
        }
    }
}
