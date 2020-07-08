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
    
    member _.Courses = _course.Force()
    member _.Enrollments = _enrollment.Force()
    member _.Students = _students.Force()

    override _.OnModelCreating(modelBuilder) =
        modelBuilder.Entity<Course>().ToTable("Course") |> ignore
        modelBuilder.Entity<Enrollment>().ToTable("Enrollment") |> ignore
        modelBuilder.Entity<Student>().ToTable("Student") |> ignore

 module DbInitializer = 
    let initialize (context:SchoolContext) =
        context.Database.EnsureCreated() |> ignore

        if not <| context.Students.Any() then
            let date = DateTime.Parse

            [
                Student(FirstMidName="Carson",LastName="Alexander",EnrollmentDate=date"2005-09-01")
                Student(FirstMidName="Meredith",LastName="Alonso",EnrollmentDate=date"2002-09-01")
                Student(FirstMidName="Arturo",LastName="Anand",EnrollmentDate=date"2003-09-01")
                Student(FirstMidName="Gytis",LastName="Barzdukas",EnrollmentDate=date"2002-09-01")
                Student(FirstMidName="Yan",LastName="Li",EnrollmentDate=date"2002-09-01")
                Student(FirstMidName="Peggy",LastName="Justice",EnrollmentDate=date"2001-09-01")
                Student(FirstMidName="Laura",LastName="Norman",EnrollmentDate=date"2003-09-01")
                Student(FirstMidName="Nino",LastName="Olivetto",EnrollmentDate=date"2005-09-01")
            ]
            |> List.iter (context.Students.Add >> ignore)
            context.SaveChanges() |> ignore

            [
                Course(CourseID=1050,Title="Chemistry",Credits=3)
                Course(CourseID=4022,Title="Microeconomics",Credits=3)
                Course(CourseID=4041,Title="Macroeconomics",Credits=3)
                Course(CourseID=1045,Title="Calculus",Credits=4)
                Course(CourseID=3141,Title="Trigonometry",Credits=4)
                Course(CourseID=2021,Title="Composition",Credits=3)
                Course(CourseID=2042,Title="Literature",Credits=4)
            ] 
            |> List.iter (context.Courses.Add >> ignore)
            context.SaveChanges() |> ignore

            [
               Enrollment(StudentID=1,CourseID=1050,Grade=Nullable Grade.A)
               Enrollment(StudentID=1,CourseID=4022,Grade=Nullable Grade.C)
               Enrollment(StudentID=1,CourseID=4041,Grade=Nullable Grade.B)
               Enrollment(StudentID=2,CourseID=1045,Grade=Nullable Grade.B)
               Enrollment(StudentID=2,CourseID=3141,Grade=Nullable Grade.F)
               Enrollment(StudentID=2,CourseID=2021,Grade=Nullable Grade.F)
               Enrollment(StudentID=3,CourseID=1050)
               Enrollment(StudentID=4,CourseID=1050)
               Enrollment(StudentID=4,CourseID=4022,Grade=Nullable Grade.F)
               Enrollment(StudentID=5,CourseID=4041,Grade=Nullable Grade.C)
               Enrollment(StudentID=6,CourseID=1045)
               Enrollment(StudentID=7,CourseID=3141,Grade=Nullable Grade.A)
            ]
            |> List.iter (context.Enrollments.Add >> ignore)
            context.SaveChanges() |> ignore

