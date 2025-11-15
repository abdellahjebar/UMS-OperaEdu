using MediatR;

namespace UMS.Application.Features.Sections.Commands.DeleteSection
{
    public class DeleteSectionCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}
