namespace ContosoUniversity.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open System.Diagnostics

open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open Microsoft.EntityFrameworkCore

open ContosoUniversity.Data
open ContosoUniversity.ViewModels
open FSharp.Control.Tasks.V2
open Microsoft.FSharp.Linq.NullableOperators
open ContosoUniversity.Models

type TaskResult = IActionResult Task

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

    member this.Index() : TaskResult =
        task {
            let! students = context.Students.ToListAsync()
            return upcast this.View(students)
        }

    member this.Details (id:int Nullable) : TaskResult =
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