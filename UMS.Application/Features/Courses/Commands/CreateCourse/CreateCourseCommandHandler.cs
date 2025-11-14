using MediatR;
using UMS.Core.Entities.Academic;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Courses.Commands.CreateCourse
{
    public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateCourseCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
        {
            var course = new Course
            {
                Code = request.Code,
                Name = request.Name,
                Description = request.Description,
                Credits = request.Credits,
                DepartmentId = request.DepartmentId
            };

            await _unitOfWork.Courses.AddAsync(course);
            await _unitOfWork.SaveChangesAsync();

            return course.Id;
        }
    }
}
