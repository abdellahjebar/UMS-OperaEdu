using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;
using UMS.Core.Interfaces.Repositories;
using UMS.Infrastructure.Persistence;

namespace UMS.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        public IStudentRepository Students { get; }
        public IFacultyRepository Faculty { get; }
        public IDepartmentRepository Departments { get; }
        public ISectionRepository Sections { get; }
        public ICourseRepository Courses { get; }
        public IEnrollmentRepository Enrollments { get; }
        public IProgramRepository Programs { get; }

        public UnitOfWork(
            ApplicationDbContext context,
            IStudentRepository studentRepository,
            IFacultyRepository facultyRepository,
            IDepartmentRepository departmentRepository,
            ISectionRepository sectionRepository,
            ICourseRepository courseRepository,
            IEnrollmentRepository enrollmentRepository,
            IProgramRepository programRepository)
        {
            _context = context;
            Students = studentRepository;
            Faculty = facultyRepository;
            Departments = departmentRepository;
            Sections = sectionRepository;
            Courses = courseRepository;
            Enrollments = enrollmentRepository;
            Programs = programRepository;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
