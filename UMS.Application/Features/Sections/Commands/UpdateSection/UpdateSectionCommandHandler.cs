using MediatR;
using UMS.Core.Entities.Academic;
using UMS.Core.Exceptions;
using UMS.Core.Interfaces;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Sections.Commands.UpdateSection
{
    public class UpdateSectionCommandHandler : IRequestHandler<UpdateSectionCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSectionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateSectionCommand request, CancellationToken cancellationToken)
        {
            var section = await _unitOfWork.Sections.GetByIdAsync(request.Id);
            if (section == null)
            {
                throw new NotFoundException($"Section with ID {request.Id} not found");
            }

            section.CourseId = request.CourseId;
            section.SectionNumber = request.SectionNumber;
            section.Term = request.Term;
            section.Year = request.Year;
            section.InstructorId = request.InstructorId;
            section.MaxCapacity = request.MaxCapacity;
            section.StartDate = request.StartDate;
            section.EndDate = request.EndDate;
            section.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Sections.UpdateAsync(section);
            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
