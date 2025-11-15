using MediatR;
using UMS.Core.Entities.Academic;
using UMS.Core.Interfaces;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Sections.Commands.CreateSection
{
    public class CreateSectionCommandHandler : IRequestHandler<CreateSectionCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateSectionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateSectionCommand request, CancellationToken cancellationToken)
        {
            var section = new Section
            {
                Id = Guid.NewGuid(),
                CourseId = request.CourseId,
                SectionNumber = request.SectionNumber,
                Term = request.Term,
                Year = request.Year,
                InstructorId = request.InstructorId,
                MaxCapacity = request.MaxCapacity,
                CurrentEnrollment = 0,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Sections.AddAsync(section);
            await _unitOfWork.SaveChangesAsync();

            return section.Id;
        }
    }
}
