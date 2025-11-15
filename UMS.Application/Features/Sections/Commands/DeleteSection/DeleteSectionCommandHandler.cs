using MediatR;
using UMS.Core.Entities.Academic;
using UMS.Core.Exceptions;
using UMS.Core.Interfaces;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Sections.Commands.DeleteSection
{
    public class DeleteSectionCommandHandler : IRequestHandler<DeleteSectionCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteSectionCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteSectionCommand request, CancellationToken cancellationToken)
        {
            var section = await _unitOfWork.Sections.GetByIdAsync(request.Id);
            if (section == null)
            {
                throw new NotFoundException($"Section with ID {request.Id} not found");
            }

            await _unitOfWork.Sections.DeleteAsync(request.Id);
            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
