using MediatR;
using UMS.Application.DTOs.Sections;

namespace UMS.Application.Features.Sections.Queries.GetSectionById
{
    public class GetSectionByIdQuery : IRequest<SectionDto>
    {
        public Guid Id { get; set; }
    }
}
