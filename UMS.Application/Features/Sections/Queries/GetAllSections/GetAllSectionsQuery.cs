using MediatR;
using UMS.Application.DTOs.Sections;

namespace UMS.Application.Features.Sections.Queries.GetAllSections
{
    public class GetAllSectionsQuery : IRequest<List<SectionDto>>
    {
    }
}
