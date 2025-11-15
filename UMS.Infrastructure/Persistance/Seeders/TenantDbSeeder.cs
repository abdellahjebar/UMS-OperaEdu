using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UMS.Core.Entities.Academic;
using UMS.Core.Entities.Identity;
using UMS.Core.Enums;
using UMS.Infrastructure.Persistence;

namespace UMS.Infrastructure.Persistance.Seeders
{
    /// <summary>
    /// Seeds a tenant database with initial sample data
    /// </summary>
    public class TenantDbSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TenantDbSeeder> _logger;

        public TenantDbSeeder(ApplicationDbContext context, ILogger<TenantDbSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Seeds the tenant database with sample academic data
        /// </summary>
        public async Task SeedAsync()
        {
            try
            {
                // Check if database already has data
                if (await _context.Users.AnyAsync())
                {
                    _logger.LogInformation("Tenant database already contains data. Skipping seed.");
                    return;
                }

                _logger.LogInformation("Seeding tenant database with initial data...");

                // Seed data in order of dependencies
                await SeedUsersAsync();
                await SeedProgramsAsync();
                await SeedCoursesAsync();
                await SeedSectionsAsync();
                await SeedEnrollmentsAsync();

                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully seeded tenant database.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the tenant database.");
                throw;
            }
        }

        private async Task SeedUsersAsync()
        {
            // Faculty user
            var faculty = new Faculty
            {
                Email = "john.doe@demouniversity.edu",
                FirstName = "John",
                LastName = "Doe",
                PasswordHash = "$2a$11$V8K7K1YTbN5xJjGU6qKQHeZfj5Bz5q3r6Wz4X7L9C8H5J2N1M0Q8K", // "Faculty@123"
                UserType = UserType.Faculty,
                IsActive = true,
                DateOfBirth = new DateTime(1985, 5, 15),
                EmployeeNumber = "F2024001",
                DepartmentId = Guid.NewGuid(), // Will create actual departments later
                Title = FacultyTitle.AssociateProfessor,
                HireDate = new DateTime(2020, 8, 1),
                OfficeLocation = "Building A, Room 201",
                OfficeHours = "Mon/Wed 2-4 PM",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Student users
            var student1 = new Student
            {
                Email = "alice.smith@demouniversity.edu",
                FirstName = "Alice",
                LastName = "Smith",
                PasswordHash = "$2a$11$V8K7K1YTbN5xJjGU6qKQHeZfj5Bz5q3r6Wz4X7L9C8H5J2N1M0Q8K", // "Student@123"
                UserType = UserType.Student,
                IsActive = true,
                DateOfBirth = new DateTime(2003, 3, 20),
                StudentNumber = "S2024001",
                ProgramId = Guid.Empty, // Will be set after programs are created
                EnrollmentDate = new DateTime(2024, 9, 1),
                ExpectedGraduationDate = new DateTime(2028, 6, 1),
                AcademicStatus = AcademicStatus.Active,
                GPA = 3.5m,
                TotalCredits = 30,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var student2 = new Student
            {
                Email = "bob.jones@demouniversity.edu",
                FirstName = "Bob",
                LastName = "Jones",
                PasswordHash = "$2a$11$V8K7K1YTbN5xJjGU6qKQHeZfj5Bz5q3r6Wz4X7L9C8H5J2N1M0Q8K", // "Student@123"
                UserType = UserType.Student,
                IsActive = true,
                DateOfBirth = new DateTime(2002, 7, 10),
                StudentNumber = "S2024002",
                ProgramId = Guid.Empty, // Will be set after programs are created
                EnrollmentDate = new DateTime(2024, 9, 1),
                ExpectedGraduationDate = new DateTime(2028, 6, 1),
                AcademicStatus = AcademicStatus.Active,
                GPA = 3.2m,
                TotalCredits = 24,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Admin user (Staff)
            var admin = new Staff
            {
                Email = "admin@demouniversity.edu",
                FirstName = "System",
                LastName = "Administrator",
                PasswordHash = "$2a$11$V8K7K1YTbN5xJjGU6qKQHeZfj5Bz5q3r6Wz4X7L9C8H5J2N1M0Q8K", // "Admin@123"
                UserType = UserType.Admin,
                IsActive = true,
                DateOfBirth = new DateTime(1980, 1, 1),
                EmployeeNumber = "A2024001",
                DepartmentId = Guid.NewGuid(),
                JobTitle = "System Administrator",
                HireDate = new DateTime(2024, 1, 1),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Faculties.Add(faculty);
            _context.Students.AddRange(student1, student2);
            _context.Staff.Add(admin);

            await _context.SaveChangesAsync();

            // Store student references for later use
            _context.Set<Student>().Local.Add(student1);
            _context.Set<Student>().Local.Add(student2);
        }

        private async Task SeedProgramsAsync()
        {
            var programs = new[]
            {
                new Program
                {
                    Code = "CS-BS",
                    Name = "Bachelor of Science in Computer Science",
                    Description = "Comprehensive program covering software development, algorithms, and system design",
                    DepartmentId = Guid.NewGuid(),
                    DegreeType = DegreeType.Bachelor,
                    DurationYears = 4,
                    RequiredCredits = 120,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Program
                {
                    Code = "EE-BS",
                    Name = "Bachelor of Science in Electrical Engineering",
                    Description = "Program focusing on circuits, electronics, and power systems",
                    DepartmentId = Guid.NewGuid(),
                    DegreeType = DegreeType.Bachelor,
                    DurationYears = 4,
                    RequiredCredits = 128,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            _context.Programs.AddRange(programs);
            await _context.SaveChangesAsync();

            // Update student programs
            var students = await _context.Students.ToListAsync();
            if (students.Count >= 2)
            {
                students[0].ProgramId = programs[0].Id;
                students[1].ProgramId = programs[0].Id;
                await _context.SaveChangesAsync();
            }
        }

        private async Task SeedCoursesAsync()
        {
            var departmentId = Guid.NewGuid(); // In production, reference actual department

            var courses = new[]
            {
                new Course
                {
                    Code = "CS101",
                    Name = "Introduction to Programming",
                    Description = "Fundamentals of programming using modern languages",
                    Credits = 3,
                    DepartmentId = departmentId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Course
                {
                    Code = "CS201",
                    Name = "Data Structures",
                    Description = "Advanced data structures and algorithms",
                    Credits = 4,
                    DepartmentId = departmentId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Course
                {
                    Code = "CS301",
                    Name = "Database Systems",
                    Description = "Relational databases, SQL, and database design",
                    Credits = 3,
                    DepartmentId = departmentId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            _context.Courses.AddRange(courses);
            await _context.SaveChangesAsync();
        }

        private async Task SeedSectionsAsync()
        {
            var courses = await _context.Courses.ToListAsync();
            var faculties = await _context.Faculties.ToListAsync();

            if (courses.Count == 0 || faculties.Count == 0)
            {
                _logger.LogWarning("Cannot seed sections: no courses or faculties available.");
                return;
            }

            var sections = new[]
            {
                new Section
                {
                    CourseId = courses[0].Id,
                    SectionNumber = "001",
                    Term = Term.Fall,
                    Year = 2024,
                    InstructorId = faculties[0].Id,
                    MaxCapacity = 30,
                    CurrentEnrollment = 0,
                    StartDate = new DateTime(2024, 9, 1),
                    EndDate = new DateTime(2024, 12, 15),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Section
                {
                    CourseId = courses[1].Id,
                    SectionNumber = "001",
                    Term = Term.Fall,
                    Year = 2024,
                    InstructorId = faculties[0].Id,
                    MaxCapacity = 25,
                    CurrentEnrollment = 0,
                    StartDate = new DateTime(2024, 9, 1),
                    EndDate = new DateTime(2024, 12, 15),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            _context.Sections.AddRange(sections);
            await _context.SaveChangesAsync();
        }

        private async Task SeedEnrollmentsAsync()
        {
            var sections = await _context.Sections.ToListAsync();
            var students = await _context.Students.ToListAsync();

            if (sections.Count == 0 || students.Count == 0)
            {
                _logger.LogWarning("Cannot seed enrollments: no sections or students available.");
                return;
            }

            var enrollments = new List<Enrollment>();

            // Enroll first student in both sections
            if (students.Count > 0 && sections.Count > 0)
            {
                enrollments.Add(new Enrollment
                {
                    StudentId = students[0].Id,
                    SectionId = sections[0].Id,
                    EnrollmentDate = DateTime.UtcNow,
                    Status = EnrollmentStatus.Enrolled,
                    NumericGrade = null, // Not graded yet
                    GradePoints = null,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });

                if (sections.Count > 1)
                {
                    enrollments.Add(new Enrollment
                    {
                        StudentId = students[0].Id,
                        SectionId = sections[1].Id,
                        EnrollmentDate = DateTime.UtcNow,
                        Status = EnrollmentStatus.Enrolled,
                        NumericGrade = null,
                        GradePoints = null,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }
            }

            // Enroll second student in first section
            if (students.Count > 1 && sections.Count > 0)
            {
                enrollments.Add(new Enrollment
                {
                    StudentId = students[1].Id,
                    SectionId = sections[0].Id,
                    EnrollmentDate = DateTime.UtcNow,
                    Status = EnrollmentStatus.Enrolled,
                    NumericGrade = null,
                    GradePoints = null,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            _context.Enrollments.AddRange(enrollments);

            // Update section enrollment counts
            foreach (var section in sections)
            {
                section.CurrentEnrollment = enrollments.Count(e => e.SectionId == section.Id);
            }

            await _context.SaveChangesAsync();
        }
    }
}
