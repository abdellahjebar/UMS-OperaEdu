using MediatR;
using UMS.Application.DTOs.Courses;
using UMS.Core.Entities.Academic;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Courses.Queries.GetAllCourses
{
    public class GetAllCoursesQueryHandler : IRequestHandler<GetAllCoursesQuery, List<CourseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllCoursesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<CourseDto>> Handle(GetAllCoursesQuery request, CancellationToken cancellationToken)
        {
            var courses = await _unitOfWork.Courses.GetAllAsync();

            return courses.Select(c => new CourseDto
            {
                Id = c.Id,
                Code = c.Code,
                Name = c.Name,
                Description = c.Description,
                Credits = c.Credits,
                DepartmentId = c.DepartmentId
            }).ToList();
        }
    }
}
