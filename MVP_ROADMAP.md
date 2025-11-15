# UMS MVP Roadmap - Complete Working System

**Goal**: Deliver a fully functional Minimum Viable Product (MVP) that enables a university to:
1. Manage student registration and enrollment
2. Track academic progress
3. Manage course schedules
4. Handle basic grading
5. Support multi-tenant operations

**Target**: Production-ready MVP in 4-6 weeks

---

## 🎯 MVP SCOPE DEFINITION

### What's IN the MVP:
✅ Student registration and enrollment  
✅ Course catalog and section management  
✅ Faculty assignment to courses  
✅ Grade entry and GPA calculation  
✅ Basic reporting (transcripts, enrollment reports)  
✅ Multi-tenant support  
✅ Role-based access control  

### What's OUT of the MVP (Future Releases):
❌ Financial/payment processing  
❌ Advanced reporting and analytics  
❌ Alumni management  
❌ Admissions workflow  
❌ Document management system  
❌ Mobile apps  
❌ External integrations (LMS, payment gateways)  

---

## 📋 PRIORITY 1: CRITICAL FOR MVP (Must Complete First)

### 1.1 Faculty Management Module ⏱️ 2-3 days

**Why Critical**: Faculty must be assigned to sections before students can enroll

#### Tasks:
- [ ] Create `FacultyDto` with department info
- [ ] Implement Faculty CQRS handlers:
  - [ ] `CreateFacultyCommand/Handler/Validator`
  - [ ] `UpdateFacultyCommand/Handler/Validator`
  - [ ] `DeleteFacultyCommand/Handler`
  - [ ] `GetAllFacultyQuery/Handler`
  - [ ] `GetFacultyByIdQuery/Handler`
  - [ ] `GetFacultyByDepartmentQuery/Handler`
- [ ] Create `FacultyController` (AdminOnly for mutations)
- [ ] Update `FacultyRepository` with custom queries
- [ ] Add validation: email uniqueness, department existence
- [ ] Write unit tests for faculty operations

**Acceptance Criteria**:
- Admin can create/update/delete faculty
- Faculty can be assigned to departments
- Faculty can be queried by department
- All operations respect tenant isolation

---

### 1.2 Schedule & Time Conflict Management ⏱️ 3-4 days

**Why Critical**: Prevents double-booking of students, faculty, and rooms

#### Tasks:
- [ ] Create `Schedule` value object (DayOfWeek, StartTime, EndTime, Room)
- [ ] Update `Section` entity to include schedule details
- [ ] Implement conflict detection service:
  - [ ] `IScheduleConflictService` interface
  - [ ] Student schedule conflict checking
  - [ ] Faculty schedule conflict checking
  - [ ] Room conflict checking
- [ ] Add validation to `CreateSectionCommand`:
  - [ ] Faculty availability check
  - [ ] Room availability check
- [ ] Add validation to `CreateEnrollmentCommand`:
  - [ ] Student schedule conflict check
  - [ ] Section capacity check
- [ ] Create migration for Section schedule fields

**Acceptance Criteria**:
- System prevents students from enrolling in overlapping sections
- System prevents faculty from being assigned to overlapping sections
- System prevents room double-booking
- Clear error messages when conflicts detected

---

### 1.3 Prerequisite Validation ⏱️ 2-3 days

**Why Critical**: Ensures students meet course requirements before enrollment

#### Tasks:
- [ ] Create `CoursePrerequisite` entity (CourseId, PrerequisiteCourseId, MinimumGrade)
- [ ] Create `CoursePrerequisiteConfiguration` for EF Core
- [ ] Implement prerequisite validation service:
  - [ ] `IPrerequisiteValidationService` interface
  - [ ] Check student completed prerequisites
  - [ ] Check minimum grade requirements
  - [ ] Check co-requisites
- [ ] Add prerequisite checking to enrollment process
- [ ] Create CRUD operations for course prerequisites:
  - [ ] Add prerequisite (AdminOnly)
  - [ ] Remove prerequisite (AdminOnly)
  - [ ] Get course prerequisites
- [ ] Create migration for CoursePrerequisite table

**Acceptance Criteria**:
- Students cannot enroll in courses without meeting prerequisites
- System checks for minimum grade in prerequisite courses
- Admin can manage course prerequisites
- Clear error messages for unmet prerequisites

---

### 1.4 Grade Calculation & GPA System ⏱️ 3-4 days

**Why Critical**: Core academic functionality for tracking student progress

#### Tasks:
- [ ] Create grade calculation service:
  - [ ] `IGradeCalculationService` interface
  - [ ] Letter grade calculation (A, B+, B, etc.)
  - [ ] Grade points calculation (4.0 scale)
  - [ ] GPA calculation (term GPA, cumulative GPA)
  - [ ] Credit hours earned calculation
- [ ] Create grade configuration:
  - [ ] Grade scale configuration (90-100 = A, etc.)
  - [ ] Grade points mapping
- [ ] Implement GPA recalculation:
  - [ ] Triggered when grade is updated
  - [ ] Update student record with new GPA
  - [ ] Update total credits earned
- [ ] Add grade history tracking:
  - [ ] `GradeHistory` entity (audit trail)
  - [ ] Track who changed grade and when
- [ ] Update `UpdateEnrollmentGradeCommand`:
  - [ ] Auto-calculate letter grade from numeric grade
  - [ ] Auto-calculate grade points
  - [ ] Trigger GPA recalculation
- [ ] Create endpoints:
  - [ ] Get student GPA (term, cumulative)
  - [ ] Get student transcript data

**Acceptance Criteria**:
- Letter grades automatically calculated from numeric grades
- GPA accurately calculated (term and cumulative)
- Student record shows current GPA and credits
- Grade changes trigger GPA recalculation
- Grade history maintained for auditing

---

### 1.5 Academic Calendar & Registration Periods ⏱️ 2-3 days

**Why Critical**: Controls when students can register for courses

#### Tasks:
- [ ] Create `AcademicTerm` entity:
  - [ ] Term (Fall/Spring/Summer), Year
  - [ ] StartDate, EndDate
  - [ ] RegistrationStartDate, RegistrationEndDate
  - [ ] DropAddEndDate, WithdrawDeadlineDate
  - [ ] IsCurrentTerm flag
- [ ] Create `AcademicTermConfiguration` for EF Core
- [ ] Implement term management:
  - [ ] Create term (AdminOnly)
  - [ ] Update term dates (AdminOnly)
  - [ ] Set current term (AdminOnly)
  - [ ] Get current term (public)
  - [ ] Get all terms (public)
- [ ] Add registration period validation:
  - [ ] Check if registration is open
  - [ ] Check if drop/add period active
  - [ ] Check if withdrawal allowed
- [ ] Update enrollment workflow:
  - [ ] Validate registration period on enrollment
  - [ ] Validate drop/add period for schedule changes
  - [ ] Validate withdrawal deadline
- [ ] Create migration for AcademicTerm table

**Acceptance Criteria**:
- Admin can define academic terms with registration dates
- Students can only enroll during open registration periods
- Students can drop/add during drop/add period
- System enforces withdrawal deadlines
- Clear error messages for closed registration

---

### 1.6 Transcript Generation ⏱️ 2-3 days

**Why Critical**: Core student service requirement

#### Tasks:
- [ ] Create transcript service:
  - [ ] `ITranscriptService` interface
  - [ ] Generate transcript data (courses, grades, GPA by term)
  - [ ] Calculate cumulative metrics
  - [ ] Include student info, program info
- [ ] Create transcript DTO:
  - [ ] `TranscriptDto` with nested term/course data
  - [ ] Term summaries (term GPA, credits earned)
  - [ ] Cumulative summary (overall GPA, total credits)
- [ ] Implement transcript endpoints:
  - [ ] Get student transcript (GET /api/students/{id}/transcript)
  - [ ] Student can view own transcript
  - [ ] Faculty/Admin can view any transcript
- [ ] Add PDF generation (optional but recommended):
  - [ ] Use library like QuestPDF or iTextSharp
  - [ ] Professional transcript layout
  - [ ] Official vs. unofficial transcript distinction

**Acceptance Criteria**:
- Students can view their academic transcript
- Transcript shows all courses with grades
- GPA calculated by term and cumulatively
- Transcript respects tenant isolation
- PDF export available (optional)

---

### 1.7 Enrollment Status Management ⏱️ 1-2 days

**Why Critical**: Track enrollment lifecycle properly

#### Tasks:
- [ ] Enhance enrollment status workflow:
  - [ ] Add `DropEnrollmentCommand` (during drop/add period - full refund)
  - [ ] Differentiate from `WithdrawEnrollmentCommand` (after drop/add - no refund)
  - [ ] Update status to Withdrawn with timestamp
- [ ] Add enrollment status rules:
  - [ ] Enrolled → Dropped (during drop/add)
  - [ ] Enrolled → Withdrawn (after drop/add)
  - [ ] Enrolled → Completed (when grade entered)
  - [ ] Enrolled → Failed (when failing grade entered)
- [ ] Update section capacity:
  - [ ] Decrement on drop/withdraw
  - [ ] Increment on enrollment
- [ ] Add enrollment history:
  - [ ] Track status changes
  - [ ] Track who made changes

**Acceptance Criteria**:
- Enrollment status accurately reflects lifecycle
- Section capacity updated on enrollment changes
- Drop vs. withdraw properly differentiated
- Enrollment history maintained

---

## 📋 PRIORITY 2: IMPORTANT FOR MVP (Complete After Priority 1)

### 2.1 Enhanced Validation & Business Rules ⏱️ 2-3 days

#### Tasks:
- [ ] Maximum credit hours per term validation
- [ ] Minimum credit hours for full-time status
- [ ] Enrollment limit per student per term
- [ ] Academic standing calculations (Good Standing, Probation, Suspension)
- [ ] Registration holds (financial, academic, administrative)
- [ ] Graduation eligibility checking

---

### 2.2 Basic Reporting ⏱️ 3-4 days

#### Tasks:
- [ ] Enrollment reports:
  - [ ] Enrollment by term
  - [ ] Enrollment by program
  - [ ] Enrollment by course/section
- [ ] Grade distribution reports:
  - [ ] By course
  - [ ] By instructor
  - [ ] By term
- [ ] Student progress reports:
  - [ ] Credits earned vs. required
  - [ ] GPA trends
  - [ ] Courses remaining for graduation

---

### 2.3 Improved Error Handling & Logging ⏱️ 1-2 days

#### Tasks:
- [ ] Add structured logging for all operations
- [ ] Log enrollment attempts (success/failure)
- [ ] Log grade changes with audit trail
- [ ] Add correlation IDs for request tracking
- [ ] Implement log levels appropriately
- [ ] Add performance logging for slow queries

---

### 2.4 Data Validation & Integrity ⏱️ 1-2 days

#### Tasks:
- [ ] Add database constraints (foreign keys, check constraints)
- [ ] Add unique indexes (student number, email, course codes)
- [ ] Add cascading delete rules properly
- [ ] Validate referential integrity in commands
- [ ] Add concurrency handling (optimistic locking)

---

### 2.5 Enhanced Testing ⏱️ 3-4 days

#### Tasks:
- [ ] Unit tests for all CQRS handlers (target: 80% coverage)
- [ ] Integration tests for enrollment workflow
- [ ] Integration tests for grade calculation
- [ ] Repository tests with in-memory database
- [ ] Test data builders/factories
- [ ] Test coverage reports

---

## 📋 PRIORITY 3: NICE TO HAVE FOR MVP (Polish & UX)

### 3.1 Dashboard & Summary Endpoints ⏱️ 2-3 days

#### Tasks:
- [ ] Student dashboard endpoint:
  - [ ] Current courses
  - [ ] Current GPA
  - [ ] Total credits earned
  - [ ] Upcoming important dates
- [ ] Faculty dashboard endpoint:
  - [ ] Current courses teaching
  - [ ] Student roster counts
  - [ ] Grade entry status
- [ ] Admin dashboard endpoint:
  - [ ] Total enrollments
  - [ ] Total students/faculty
  - [ ] Current term info

---

### 3.2 Notification System (Basic) ⏱️ 2-3 days

#### Tasks:
- [ ] In-app notification entity
- [ ] Notification service interface
- [ ] Notification creation triggers:
  - [ ] Enrollment confirmation
  - [ ] Grade posted
  - [ ] Important date reminders
- [ ] Notification endpoints:
  - [ ] Get user notifications
  - [ ] Mark as read
  - [ ] Delete notification

---

### 3.3 Course Search & Filtering ⏱️ 1-2 days

#### Tasks:
- [ ] Advanced course search:
  - [ ] Search by code, name, department
  - [ ] Filter by term, credits, department
  - [ ] Filter available sections (not full)
- [ ] Section availability checking
- [ ] Course catalog view

---

### 3.4 User Profile Management ⏱️ 1-2 days

#### Tasks:
- [ ] View user profile
- [ ] Update profile (name, phone, email)
- [ ] Change password
- [ ] Profile photo upload (optional)

---

## 🗓️ SUGGESTED IMPLEMENTATION TIMELINE

### Week 1-2: Critical Foundation
**Days 1-3**: Faculty Management Module (1.1)  
**Days 4-7**: Schedule & Conflict Management (1.2)  
**Days 8-10**: Prerequisite Validation (1.3)  

### Week 3-4: Core Academic Functions
**Days 11-14**: Grade Calculation & GPA System (1.4)  
**Days 15-17**: Academic Calendar & Registration (1.5)  
**Days 18-20**: Transcript Generation (1.6)  
**Days 21-22**: Enrollment Status Management (1.7)  

### Week 5: Important Features
**Days 23-25**: Enhanced Validation (2.1)  
**Days 26-28**: Basic Reporting (2.2)  
**Days 29-30**: Error Handling & Logging (2.3)  

### Week 6: Polish & Testing
**Days 31-32**: Data Validation & Integrity (2.4)  
**Days 33-35**: Enhanced Testing (2.5)  
**Days 36-37**: Dashboard Endpoints (3.1)  
**Days 38-40**: Final testing, bug fixes, documentation  

---

## 📊 MVP COMPLETION CHECKLIST

### Core Functionality ✅
- [ ] Students can register for courses
- [ ] Faculty can enter grades
- [ ] GPA is calculated automatically
- [ ] Prerequisites are enforced
- [ ] Schedule conflicts are prevented
- [ ] Transcripts can be generated
- [ ] Multi-tenant operations work correctly

### Quality Standards ✅
- [ ] 80%+ test coverage
- [ ] All API endpoints documented
- [ ] Proper error handling
- [ ] Security best practices followed
- [ ] Performance acceptable (<500ms for most operations)
- [ ] Database properly indexed

### Deployment Ready ✅
- [ ] Database migrations applied cleanly
- [ ] Seed data working correctly
- [ ] Environment configuration documented
- [ ] Logging configured
- [ ] Health checks implemented

---

## 🚀 POST-MVP ROADMAP (Future Releases)

### Release 2.0 - Financial Management
- Tuition calculation
- Payment processing
- Financial aid tracking
- Refund processing

### Release 3.0 - Advanced Academic Features
- Degree planning tools
- Graduation audit
- Course recommendations
- Academic advising workflow

### Release 4.0 - External Integrations
- Payment gateway integration
- Email/SMS notifications
- LMS integration
- SSO/SAML authentication

### Release 5.0 - Analytics & Reporting
- Advanced reporting dashboard
- Student success analytics
- Retention metrics
- Predictive analytics

---

## 📝 RECOMMENDATIONS

### Start Immediately:
1. **Faculty Management** (1.1) - Blocks everything else
2. **Schedule Conflicts** (1.2) - Critical for registration
3. **Prerequisites** (1.3) - Required for enrollment validation

### Parallelizable Work:
- After 1.1-1.3 are done, these can be worked in parallel:
  - Grade Calculation (1.4)
  - Academic Calendar (1.5)
  - Transcript Generation (1.6)

### Can Wait Until Beta:
- Dashboards (3.1)
- Notifications (3.2)
- Advanced search (3.3)

### Key Success Factors:
✅ Complete Priority 1 items before moving to Priority 2  
✅ Write tests as you go (not at the end)  
✅ Keep documentation updated  
✅ Regular testing with seed data  
✅ Daily commits to Git  

---

**Estimated Total Effort**: 4-6 weeks (1 full-time developer)  
**MVP Completion Date**: ~January 10, 2026 (if starting now)  
**Production Ready**: ~January 31, 2026 (with testing & polish)

*This roadmap focuses on delivering a working, production-ready MVP that provides immediate value while setting the foundation for future enterprise features.*
