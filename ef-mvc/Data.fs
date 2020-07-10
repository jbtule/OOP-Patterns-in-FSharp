namespace ContosoUniversity.Data

open ContosoUniversity.Models
open Microsoft.EntityFrameworkCore
open Unchecked
open System
open System.Linq


type SchoolContext (options:SchoolContext DbContextOptions) as this =
    inherit DbContext(options)
    let _course= lazy this.Set<Course>() 
    let _enrollment = lazy this.Set<Enrollment>()
    let _students = lazy this.Set<Student>()  
    let _departments = lazy this.Set<Department>()
    let _instructors = lazy this.Set<Instructor>()
    let _officeAssignments = lazy this.Set<OfficeAssignment>()
    let _courseAssignments = lazy this.Set<CourseAssignment>()

    member _.Courses = _course.Force()
    member _.Enrollments = _enrollment.Force()
    member _.Students = _students.Force()
    member _.Departments = _departments.Force()
    member _.Instructors = _instructors.Force()
    member _.OfficeAssignments= _officeAssignments.Force()
    member _.CourseAssignments = _courseAssignments.Force()

    override _.OnModelCreating(modelBuilder) =
        modelBuilder.Entity<Course>().ToTable("Course") |> ignore
        modelBuilder.Entity<Enrollment>().ToTable("Enrollment") |> ignore
        modelBuilder.Entity<Student>().ToTable("Student") |> ignore
        modelBuilder.Entity<Department>().ToTable("Department") |> ignore
        modelBuilder.Entity<Instructor>().ToTable("Instructor") |> ignore
        modelBuilder.Entity<OfficeAssignment>().ToTable("OfficeAssignment") |> ignore
        modelBuilder.Entity<CourseAssignment>().ToTable("CourseAssignment") |> ignore
        modelBuilder.Entity<CourseAssignment>()
            .HasKey(fun c -> {|CourseID = c.CourseID; InstructorID=c.InstructorID |} :> obj) |> ignore

module DbInitializer = 
    let initialize (context:SchoolContext) =
        context.Database.EnsureCreated() |> ignore

        if not <| context.Students.Any() then
            let date = DateTime.Parse

            let students = [
                Student(FirstMidName="Carson",LastName="Alexander",EnrollmentDate=date"2005-09-01")
                Student(FirstMidName="Meredith",LastName="Alonso",EnrollmentDate=date"2002-09-01")
                Student(FirstMidName="Arturo",LastName="Anand",EnrollmentDate=date"2003-09-01")
                Student(FirstMidName="Gytis",LastName="Barzdukas",EnrollmentDate=date"2002-09-01")
                Student(FirstMidName="Yan",LastName="Li",EnrollmentDate=date"2002-09-01")
                Student(FirstMidName="Peggy",LastName="Justice",EnrollmentDate=date"2001-09-01")
                Student(FirstMidName="Laura",LastName="Norman",EnrollmentDate=date"2003-09-01")
                Student(FirstMidName="Nino",LastName="Olivetto",EnrollmentDate=date"2005-09-01")
            ]
            students |> List.iter (context.Students.Add >> ignore)
            context.SaveChanges() |> ignore

            let instructors =
                [
                    Instructor ( FirstMidName = "Kim",     LastName = "Abercrombie", HireDate = date"1995-03-11" )
                    Instructor ( FirstMidName = "Fadi",    LastName = "Fakhouri",    HireDate = date"2002-07-06" )
                    Instructor ( FirstMidName = "Roger",   LastName = "Harui",       HireDate = date"1998-07-01" )
                    Instructor ( FirstMidName = "Candace", LastName = "Kapoor",      HireDate = date"2001-01-15" )
                    Instructor ( FirstMidName = "Roger",   LastName = "Zheng",       HireDate = date"2004-02-12" )
                ] 
                
            instructors |> List.iter (context.Instructors.Add >> ignore)
            context.SaveChanges() |> ignore

            let departments = [
                Department ( Name = "English",     Budget = 350_000m,
                    StartDate = date"2007-09-01",
                    InstructorID  = instructors.Single(fun i -> i.LastName = "Abercrombie").ID )
                Department ( Name = "Mathematics", Budget = 100_000m,
                    StartDate = date"2007-09-01",
                    InstructorID  = instructors.Single(fun i -> i.LastName = "Fakhouri").ID )
                Department ( Name = "Engineering", Budget = 350_000m,
                    StartDate = date"2007-09-01",
                    InstructorID  = instructors.Single(fun i -> i.LastName = "Harui").ID )
                Department ( Name = "Economics",   Budget = 100_000m,
                    StartDate = date"2007-09-01",
                    InstructorID  = instructors.Single(fun i -> i.LastName = "Kapoor").ID )
                ]
            departments|> List.iter (context.Departments.Add >> ignore)
            context.SaveChanges() |> ignore

            let courses = [
                Course (CourseID = 1050, Title = "Chemistry",      Credits = 3,
                    DepartmentID = departments.Single(fun s -> s.Name = "Engineering").DepartmentID
                )
                Course (CourseID = 4022, Title = "Microeconomics", Credits = 3,
                    DepartmentID = departments.Single(fun s -> s.Name = "Economics").DepartmentID
                )
                Course (CourseID = 4041, Title = "Macroeconomics", Credits = 3,
                    DepartmentID = departments.Single(fun s -> s.Name = "Economics").DepartmentID
                )
                Course (CourseID = 1045, Title = "Calculus",       Credits = 4,
                    DepartmentID = departments.Single(fun s -> s.Name = "Mathematics").DepartmentID
                )
                Course (CourseID = 3141, Title = "Trigonometry",   Credits = 4,
                    DepartmentID = departments.Single(fun s -> s.Name = "Mathematics").DepartmentID
                )
                Course (CourseID = 2021, Title = "Composition",    Credits = 3,
                    DepartmentID = departments.Single(fun s -> s.Name = "English").DepartmentID
                )
                Course (CourseID = 2042, Title = "Literature",     Credits = 4,
                    DepartmentID = departments.Single(fun s -> s.Name = "English").DepartmentID
                )
                ] 
            courses |> List.iter (context.Courses.Add >> ignore)
            context.SaveChanges() |> ignore

            [
                OfficeAssignment (
                    InstructorID = instructors.Single(fun i -> i.LastName = "Fakhouri").ID,
                    Location = "Smith 17" )
                OfficeAssignment (
                    InstructorID = instructors.Single(fun i -> i.LastName = "Harui").ID,
                    Location = "Gowan 27" )
                OfficeAssignment (
                    InstructorID = instructors.Single(fun i -> i.LastName = "Kapoor").ID,
                    Location = "Thompson 304" )
            ] |> List.iter (context.OfficeAssignments.Add >> ignore)
            context.SaveChanges() |> ignore

            [
                CourseAssignment (
                    CourseID = courses.Single(fun c -> c.Title = "Chemistry" ).CourseID,
                    InstructorID = instructors.Single(fun i-> i.LastName = "Kapoor").ID
                    )
                CourseAssignment (
                    CourseID = courses.Single(fun c -> c.Title = "Chemistry" ).CourseID,
                    InstructorID = instructors.Single(fun i-> i.LastName = "Harui").ID
                    )
                CourseAssignment (
                    CourseID = courses.Single(fun c -> c.Title = "Microeconomics" ).CourseID,
                    InstructorID = instructors.Single(fun i-> i.LastName = "Zheng").ID
                    )
                CourseAssignment (
                    CourseID = courses.Single(fun c -> c.Title = "Macroeconomics" ).CourseID,
                    InstructorID = instructors.Single(fun i-> i.LastName = "Zheng").ID
                    )
                CourseAssignment (
                    CourseID = courses.Single(fun c -> c.Title = "Calculus" ).CourseID,
                    InstructorID = instructors.Single(fun i-> i.LastName = "Fakhouri").ID
                    )
                CourseAssignment (
                    CourseID = courses.Single(fun c -> c.Title = "Trigonometry" ).CourseID,
                    InstructorID = instructors.Single(fun i-> i.LastName = "Harui").ID
                    )
                CourseAssignment (
                    CourseID = courses.Single(fun c -> c.Title = "Composition" ).CourseID,
                    InstructorID = instructors.Single(fun i-> i.LastName = "Abercrombie").ID
                    )
                CourseAssignment (
                    CourseID = courses.Single(fun c -> c.Title = "Literature" ).CourseID,
                    InstructorID = instructors.Single(fun i-> i.LastName = "Abercrombie").ID
                    )
            ] |> List.iter (context.CourseAssignments.Add >> ignore)
            context.SaveChanges() |> ignore


            [
                Enrollment (
                    StudentID = students.Single(fun s -> s.LastName = "Alexander").ID,
                    CourseID = courses.Single(fun c -> c.Title = "Chemistry" ).CourseID,
                    Grade = Grade.A
                )
                Enrollment (
                    StudentID = students.Single(fun s -> s.LastName = "Alexander").ID,
                    CourseID = courses.Single(fun c -> c.Title = "Microeconomics" ).CourseID,
                    Grade = Grade.C
                )
                Enrollment (
                    StudentID = students.Single(fun s -> s.LastName = "Alexander").ID,
                    CourseID = courses.Single(fun c -> c.Title = "Macroeconomics" ).CourseID,
                    Grade = Grade.B
                )
                Enrollment (
                    StudentID = students.Single(fun s -> s.LastName = "Alonso").ID,
                    CourseID = courses.Single(fun c -> c.Title = "Calculus" ).CourseID,
                    Grade = Grade.B
                )
                Enrollment (
                    StudentID = students.Single(fun s -> s.LastName = "Alonso").ID,
                    CourseID = courses.Single(fun c -> c.Title = "Trigonometry" ).CourseID,
                    Grade = Grade.B
                )
                Enrollment (
                    StudentID = students.Single(fun s -> s.LastName = "Alonso").ID,
                    CourseID = courses.Single(fun c -> c.Title = "Composition" ).CourseID,
                    Grade = Grade.B
                )
                Enrollment (
                    StudentID = students.Single(fun s -> s.LastName = "Anand").ID,
                    CourseID = courses.Single(fun c -> c.Title = "Chemistry" ).CourseID
                )
                Enrollment (
                    StudentID = students.Single(fun s -> s.LastName = "Anand").ID,
                    CourseID = courses.Single(fun c -> c.Title = "Microeconomics").CourseID,
                    Grade = Grade.B
                )
                Enrollment (
                    StudentID = students.Single(fun s -> s.LastName = "Barzdukas").ID,
                    CourseID = courses.Single(fun c -> c.Title = "Chemistry").CourseID,
                    Grade = Grade.B
                )
                Enrollment (
                    StudentID = students.Single(fun s -> s.LastName = "Li").ID,
                    CourseID = courses.Single(fun c -> c.Title = "Composition").CourseID,
                    Grade = Grade.B
                )
                Enrollment (
                    StudentID = students.Single(fun s -> s.LastName = "Justice").ID,
                    CourseID = courses.Single(fun c -> c.Title = "Literature").CourseID,
                    Grade = Grade.B
                )
            ]
            |> List.iter (context.Enrollments.Add >> ignore)
            context.SaveChanges() |> ignore

