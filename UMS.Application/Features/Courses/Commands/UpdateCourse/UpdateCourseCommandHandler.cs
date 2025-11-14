using MediatR;
using UMS.Core.Entities.Academic;
using UMS.Core.Exceptions;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Courses.Commands.UpdateCourse
{
    public class UpdateCourseCommandHandler : IRequestHandler<UpdateCourseCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCourseCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(request.Id);
            
            if (course == null)
            {
                throw new BadRequestException($"Course with ID {request.Id} not found");
            }

            course.Code = request.Code;
            course.Name = request.Name;
            course.Description = request.Description;
            course.Credits = request.Credits;
            course.DepartmentId = request.DepartmentId;

            await _unitOfWork.Courses.UpdateAsync(course);
            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
