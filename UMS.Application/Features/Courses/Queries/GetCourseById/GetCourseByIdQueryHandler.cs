using MediatR;
using UMS.Application.DTOs.Courses;
using UMS.Core.Entities.Academic;
using UMS.Core.Exceptions;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Courses.Queries.GetCourseById
{
    public class GetCourseByIdQueryHandler : IRequestHandler<GetCourseByIdQuery, CourseDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCourseByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CourseDto> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(request.Id);

            if (course == null)
            {
                throw new NotFoundException($"Course with ID {request.Id} not found");
            }

            return new CourseDto
            {
                Id = course.Id,
                Code = course.Code,
                Name = course.Name,
                Description = course.Description,
                Credits = course.Credits,
                DepartmentId = course.DepartmentId
            };
        }
    }
}
