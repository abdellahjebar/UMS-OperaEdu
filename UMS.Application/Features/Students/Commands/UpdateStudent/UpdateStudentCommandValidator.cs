using FluentValidation;

namespace UMS.Application.Features.Students.Commands.UpdateStudent
{
    public class UpdateStudentCommandValidator : AbstractValidator<UpdateStudentCommand>
    {
        public UpdateStudentCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Student ID is required.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name cannot exceed 50 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters.");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required.")
                .LessThan(System.DateTime.Now).WithMessage("Date of birth must be in the past.");

            RuleFor(x => x.ProgramId)
                .NotEmpty().WithMessage("Program ID is required.");
        }
    }
}
