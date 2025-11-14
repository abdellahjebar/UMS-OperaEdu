using FluentValidation;

namespace UMS.Application.Features.Programs.Commands.CreateProgram
{
    public class CreateProgramCommandValidator : AbstractValidator<CreateProgramCommand>
    {
        public CreateProgramCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Program name is required")
                .MaximumLength(200).WithMessage("Program name cannot exceed 200 characters");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Program code is required")
                .MaximumLength(20).WithMessage("Program code cannot exceed 20 characters");

            RuleFor(x => x.DegreeType)
                .IsInEnum().WithMessage("Invalid degree type");

            RuleFor(x => x.DepartmentId)
                .NotEmpty().WithMessage("Department ID is required");

            RuleFor(x => x.RequiredCredits)
                .GreaterThan(0).WithMessage("Required credits must be greater than 0")
                .LessThanOrEqualTo(300).WithMessage("Required credits cannot exceed 300");

            RuleFor(x => x.DurationYears)
                .GreaterThan(0).WithMessage("Duration must be greater than 0")
                .LessThanOrEqualTo(10).WithMessage("Duration cannot exceed 10 years");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");
        }
    }
}
