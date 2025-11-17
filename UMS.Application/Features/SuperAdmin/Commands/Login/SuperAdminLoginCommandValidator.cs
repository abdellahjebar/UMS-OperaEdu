using FluentValidation;

namespace UMS.Application.Features.SuperAdmin.Commands.Login
{
    /// <summary>
    /// Validator for SuperAdmin login command
    /// </summary>
    public class SuperAdminLoginCommandValidator : AbstractValidator<SuperAdminLoginCommand>
    {
        public SuperAdminLoginCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(100).WithMessage("Email must not exceed 100 characters");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters");
        }
    }
}
