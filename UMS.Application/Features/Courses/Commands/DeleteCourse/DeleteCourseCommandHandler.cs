using MediatR;
using UMS.Core.Entities.Academic;
using UMS.Core.Exceptions;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Courses.Commands.DeleteCourse
{
    public class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCourseCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(request.Id);
            
            if (course == null)
            {
                throw new BadRequestException($"Course with ID {request.Id} not found");
            }

            await _unitOfWork.Courses.DeleteAsync(course.Id);
            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
