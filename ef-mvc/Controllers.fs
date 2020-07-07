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

    member this.Index() : IActionResult Task=
        task {
            let! students = context.Students.ToListAsync()
            return upcast this.View(students)
        }