using MediatR;
using UMS.Core.Enums;

namespace UMS.Application.Features.Sections.Commands.CreateSection
{
    public class CreateSectionCommand : IRequest<Guid>
    {
        public Guid CourseId { get; set; }
        public string SectionNumber { get; set; } = string.Empty;
        public Term Term { get; set; }
        public int Year { get; set; }
        public Guid InstructorId { get; set; }
        public int MaxCapacity { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
