using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using UMS.Core.Interfaces.Repositories;

namespace UMS.Application.Features.Students.Commands.UpdateStudent
{
    public class UpdateStudentCommandHandler : IRequestHandler<UpdateStudentCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateStudentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
        {
            var student = await _unitOfWork.Students.GetByIdAsync(request.Id);
            if (student == null)
            {
                throw new InvalidOperationException($"Student with ID '{request.Id}' not found.");
            }

            // Update student properties
            student.FirstName = request.FirstName;
            student.LastName = request.LastName;
            student.PhoneNumber = request.PhoneNumber;
            student.DateOfBirth = request.DateOfBirth;
            student.StudentNumber = request.StudentNumber;
            student.ExpectedGraduationDate = request.ExpectedGraduationDate;
            student.ProgramId = request.ProgramId;
            student.AcademicStatus = request.AcademicStatus;

            await _unitOfWork.Students.UpdateAsync(student);
            await _unitOfWork.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
