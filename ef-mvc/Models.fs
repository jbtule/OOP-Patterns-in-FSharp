namespace rec ContosoUniversity.Models

open System
open System.Collections.Generic
open Unchecked
open System.ComponentModel.DataAnnotations.Schema;
open System.ComponentModel.DataAnnotations;

[<AllowNullLiteral>] //Since we are using EF we are going to have nulls, better for everyone if we let F# expect it.
type Student () =
    [<Key>]
    member val ID = defaultof<int> with get,set
    [<StringLength(50); Required>]
    [<Display(Name = "Last Name")>]
    member val LastName = defaultof<string> with get,set
    [<StringLength(50); Required; Column("FirstName")>]
    [<Display(Name = "First Name")>]
    member val FirstMidName = defaultof<string> with get,set
    [<DataType(DataType.Date)>]
    [<DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true); Display(Name = "Enrollment Date")>]
    member val EnrollmentDate = defaultof<DateTime> with get,set
    [<Display(Name = "Full Name")>]
    member this.FullName = sprintf "%s, %s" this.LastName this.FullName

    member val Enrollments = defaultof<Enrollment ICollection> with get,set

[<AllowNullLiteral>]
type Instructor () =
    [<Key>]
    member val ID = defaultof<int> with get,set
    [<StringLength(50); Required>]
    [<Display(Name = "Last Name")>]
    member val LastName = defaultof<string> with get,set
    [<StringLength(50); Required; Column("FirstName")>]
    [<Display(Name = "First Name")>]
    member val FirstMidName = defaultof<string> with get,set
    [<DataType(DataType.Date)>]
    [<DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true); Display(Name = "Hire Date")>]
    member val HireDate = defaultof<DateTime> with get,set
    [<Display(Name = "Full Name")>]
    member this.FullName = sprintf "%s, %s" this.LastName this.FullName
    
    member val CourseAssignments = defaultof<CourseAssignment ICollection> with get,set
    member val OfficeAssignment = defaultof<OfficeAssignment> with get,set

[<AllowNullLiteral>]
type OfficeAssignment () = 
    [<Key>]
    member val InstructorID = defaultof<int> with get,set
    [<StringLength(50)>]
    [<Display(Name = "Office Location")>]
    member val Location = defaultof<string> with get,set
    member val Instructor = defaultof<Instructor> with get,set

type CourseAssignment () = 
    member val InstructorID = defaultof<int> with get,set
    member val CourseID = defaultof<int> with get,set
    member val Instructor = defaultof<Instructor> with get,set
    member val Course = defaultof<Course> with get,set


type Grade = 
    | A = 0
    | B = 1 
    | C = 2 
    | D = 3 
    | F = 4

[<AllowNullLiteral>]
type Enrollment () =
    member val EnrollmentID = defaultof<int> with get,set
    member val CourseID = defaultof<int> with get,set
    member val StudentID = defaultof<int> with get,set
    [<DisplayFormat(NullDisplayText = "No grade")>]
    member val Grade = defaultof<Grade Nullable> with get,set
    member val Course = defaultof<Course> with get,set
    member val Student = defaultof<Student> with get,set

[<AllowNullLiteral>]
type Course () = 
    [<DatabaseGenerated(DatabaseGeneratedOption.None)>]
    [<Display(Name = "Number")>]
    member val CourseID = defaultof<int> with get,set
    [<StringLength(50, MinimumLength = 3)>]
    member val Title = defaultof<string> with get,set
    [<Range(0, 5)>]
    member val Credits = defaultof<int> with get,set

    member val DepartmentID = defaultof<int> with get,set
    member val Department = defaultof<Department> with get,set


    member val Enrollments = defaultof<Enrollment ICollection> with get,set
    member val CourseAssignments = defaultof<CourseAssignment ICollection> with get,set

type Department () =
    member val DepartmentID = defaultof<int> with get,set
    [<StringLength(50, MinimumLength = 3)>]
    member val Name = defaultof<string> with get,set
    [<DataType(DataType.Currency)>]
    member val Budget = defaultof<decimal> with get,set
    [<DataType(DataType.Date)>]
    [<DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true);Display(Name = "Start Date")>]
    member val StartDate = defaultof<DateTime> with get,set
    member val InstructorID = defaultof<int Nullable> with get,set
    member val Administrator = defaultof<Instructor> with get,set
    member val Courses = defaultof<Course ICollection> with get,set





