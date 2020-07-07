namespace ContosoUniversity.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open System.Diagnostics
open System.Runtime.InteropServices

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open Microsoft.EntityFrameworkCore

open ContosoUniversity.Data
open ContosoUniversity.ViewModels
open FSharp.Control.Tasks.V2
open Microsoft.FSharp.Linq.NullableOperators
open ContosoUniversity.Models

type MVCTask = IActionResult Task

type HomeController (logger : ILogger<HomeController>) =
    inherit Controller()

    member this.Index () =
        this.View()

    member this.Privacy () =
        this.View()

    [<ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)>]
    member this.Error () =
        let reqId = 
            if isNull Activity.Current then
                this.HttpContext.TraceIdentifier
            else
                Activity.Current.Id

        this.View({ RequestId = reqId })

type StudentsController (context:SchoolContext) =
    inherit Controller ()

    member this.Index() : MVCTask =
        task {
            let! students = context.Students.ToListAsync()
            return upcast this.View(students)
        }

    member this.Details (id:int Nullable) : MVCTask =
        task {
            if id.HasValue |> not then
                return upcast this.NotFound()
            else
                let! student = 
                    context
                        .Students
                        .Include(fun s->s.Enrollments :> IEnumerable<Enrollment>)
                        .ThenInclude(fun (e:Enrollment)->e.Course)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(fun m-> m.StudentID =? id)
                if student |> isNull then
                    return upcast this.NotFound()
                else
                    return upcast this.View(student)

        }
    [<HttpPost;ValidateAntiForgeryToken>]
    member this.Create([<Bind("EnrollmentDate,FirstMidName,LastName")>]student:Student) : MVCTask = 
        task {
            if this.ModelState.IsValid then
                try
                    context.Add(student) |> ignore
                    let! _ = context.SaveChangesAsync()
                    return upcast this.RedirectToAction(nameof Index)
                with :? DbUpdateException ->
                    this.ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists " +
                        "see your system administrator.");    
                    return upcast this.View(student)     
            else
                return upcast this.View(student)
        }
(*
    [<HttpPost;ActionName("Edit");ValidateAntiForgeryToken>]
    member this.EditPost(id:int Nullable) : MVCTask = 
        task {
            if id.HasValue |> not then
                return upcast this.NotFound()
            else
                let! studentToUpdate = context.Students.FirstOrDefaultAsync(fun s -> s.StudentID =? id)
                match! this.TryUpdateModelAsync<Student>(studentToUpdate, "", 
                            fun s -> s.FirstMidName, fun s -> s.LastName, fun s-> s.EnrollmentDate) with
                | true,_ ->
                    try
                        let! _ = context.SaveChangesAsync()
                        return upcast this.RedirectToAction(nameof Index)
                    with :? DbUpdateException ->
                        this.ModelState.AddModelError("", "Unable to save changes. " +
                            "Try again, and if the problem persists " +
                            "see your system administrator.");    
                        return upcast this.View(studentToUpdate)     
                | false,_ -> 
                    return upcast this.View(studentToUpdate)    

        }
*)

    member this.Create(id:int, [<Bind("StudentID,EnrollmentDate,FirstMidName,LastName")>]student:Student) : MVCTask = 
        task {
            if id <> student.StudentID then
                return upcast this.NotFound()
            else if this.ModelState.IsValid then
                try
                    context.Update(student) |> ignore
                    let! _ = context.SaveChangesAsync()
                    return upcast this.RedirectToAction(nameof Index)
                with :? DbUpdateException ->
                    this.ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists " +
                        "see your system administrator.");    
                    return upcast this.View(student)     
            else
                return upcast this.View(student)
        }

    member this.Delete(id:int Nullable, 
        [<Optional;DefaultParameterValue(false)>] saveChangesError: bool Nullable) : MVCTask =
        task {
            if id.HasValue |> not then
                return upcast this.NotFound()
            else
                let! student = context.Students.FirstOrDefaultAsync(fun s -> s.StudentID =? id)
                if student |> isNull then
                    return upcast this.NotFound()
                else if (saveChangesError |> Option.ofNullable |> Option.defaultValue false) then
                    this.ViewData.["ErrorMessage"] <-
                            "Delete failed. Try again, and if the problem persists " +
                            "see your system administrator."
                    return upcast this.View(student)
                else
                    return upcast this.View(student)
        }
        


    [<HttpPost;ActionName("Delete");ValidateAntiForgeryToken>]
    member this.DeleteConfirmed(id:int) : MVCTask = 
        task {
            let! student = context.Students.FindAsync(id)
            if student |> isNull then
                return upcast this.RedirectToAction(nameof this.Index)
            else 
                try
                    context.Remove(student) |> ignore
                    let! _ = context.SaveChangesAsync()
                    return upcast this.RedirectToAction(nameof this.Index)
                with :? DbUpdateException ->
                    return upcast this.RedirectToAction(nameof this.Delete, {| id = id; saveChangesError = true |})

        }