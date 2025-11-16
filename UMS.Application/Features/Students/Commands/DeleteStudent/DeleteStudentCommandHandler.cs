using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Students.Commands.DeleteStudent
{
    public class DeleteStudentCommandHandler : IRequestHandler<DeleteStudentCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteStudentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(request.Id);
            if (student == null)
            {
                throw new InvalidOperationException($"Student with ID '{request.Id}' not found.");
            }

            await _unitOfWork.Students.DeleteAsync(request.Id);
            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
