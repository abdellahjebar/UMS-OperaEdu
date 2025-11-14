using MediatR;
using UMS.Core.Enums;

namespace UMS.Application.Features.Programs.Commands.CreateProgram
{
    public class CreateProgramCommand : IRequest<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public DegreeType DegreeType { get; set; }
        public Guid DepartmentId { get; set; }
        public int RequiredCredits { get; set; }
        public int DurationYears { get; set; }
        public string? Description { get; set; }
    }
}
