using FluentValidation;

namespace UMS.Application.Features.Departments.Commands.UpdateDepartment
{
    public class UpdateDepartmentCommandValidator : AbstractValidator<UpdateDepartmentCommand>
    {
        public UpdateDepartmentCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Department ID is required");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Department name is required")
                .MaximumLength(200).WithMessage("Department name cannot exceed 200 characters");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Department code is required")
                .MaximumLength(20).WithMessage("Department code cannot exceed 20 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.Building)
                .MaximumLength(100).WithMessage("Building cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.Building));

            RuleFor(x => x.Phone)
                .MaximumLength(20).WithMessage("Phone cannot exceed 20 characters")
                .When(x => !string.IsNullOrEmpty(x.Phone));

            RuleFor(x => x.Email)
                .MaximumLength(100).WithMessage("Email cannot exceed 100 characters")
                .EmailAddress().WithMessage("Email must be a valid email address")
                .When(x => !string.IsNullOrEmpty(x.Email));
        }
    }
}
