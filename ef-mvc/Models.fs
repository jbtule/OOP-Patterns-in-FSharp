namespace rec ContosoUniversity.Models

open System
open System.Collections.Generic
open Unchecked
open System.ComponentModel.DataAnnotations.Schema;


[<AllowNullLiteral>] //Since we are using EF we are going to have nulls, better for everyone if we let F# expect it.
type Student () =
    member val ID = defaultof<int> with get,set
    member val LastName = defaultof<string> with get,set
    member val FirstMidName = defaultof<string> with get,set
    member val EnrollmentDate = defaultof<DateTime> with get,set

    member val Enrollments = defaultof<Enrollment ICollection> with get,set

type Grade = 
    | A = 0
    | B = 1 
    | C = 2 
    | D = 3 
    | F = 4

[<AllowNullLiteral>]
type Enrollment () =
    member val EnrollemntID = defaultof<int> with get,set
    member val CourseID = defaultof<int> with get,set
    member val StudentID = defaultof<int> with get,set
    member val Grade = defaultof<Grade Nullable> with get,set
    member val Course = defaultof<Course> with get,set
    member val Student = defaultof<Student> with get,set

[<AllowNullLiteral>]
type Course () = 
    [<DatabaseGenerated(DatabaseGeneratedOption.None)>]
    member val CourseID = defaultof<int> with get,set
    member val Title = defaultof<string> with get,set
    member val Credits = defaultof<int> with get,set

    member val Enrollments = defaultof<Enrollment ICollection> with get,set


