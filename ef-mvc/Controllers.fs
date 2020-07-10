namespace ContosoUniversity.Controllers

open System
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
open ContosoUniversity
open ContosoUniversity.Models

type MVCTask = IActionResult Task
type UpdateExpr<'T> when 'T: not struct = System.Linq.Expressions.Expression<Func<'T,obj>>
[<AbstractClass;Sealed>]
type internal Helper = 
    static member UpdateExpr ([<ReflectedDefinition>]expr:UpdateExpr<'a>) = expr

open Helper

type HomeController (logger : ILogger<HomeController>, context:SchoolContext) =
    inherit Controller()


    member this.About() = task {
        let data = 
            context
                .Students
                .GroupBy(fun s-> s.EnrollmentDate)
                .Select(fun x -> {EnrollmentDate = Nullable x.Key; StudentCount =x.Count() })
        let! model = data.AsNoTracking().ToListAsync()
        return this.View(model)
    }
                
                
    member this.Index () = this.View()
    member this.Privacy () = this.View()

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

    member this.Index(sortOrder:string, currentFilter:String, searchString:string, [<DefaultParameterValue(null)>] pageNumber:int Nullable) : MVCTask =
        task {
            this.ViewData.["CurrentSort"] <- sortOrder
            this.ViewData.["NameSortParm"] <- if String.IsNullOrEmpty(sortOrder) then "name_desc" else "" 
            this.ViewData.["DateSortParm"] <- if sortOrder = "Date" then "date_desc" else "Date"

            let searchString', pageNumber' =
                if searchString |> (not << isNull) then
                    searchString,Some 1
                else
                    currentFilter,pageNumber |> Option.ofNullable

            this.ViewData.["CurrentFilter"] <- searchString';

            let students = 
                if not <| String.IsNullOrEmpty searchString' then
                    context.Students.Where(fun s-> s.LastName.Contains(searchString') || s.FirstMidName.Contains(searchString'))
                else
                    upcast context.Students
        
            let students' = 
                match sortOrder with
                | "name_desc" ->  students.OrderByDescending(fun s -> s.LastName)
                | "Date" -> students.OrderBy(fun s->s.EnrollmentDate)
                | "date_desc" -> students.OrderByDescending(fun s->s.EnrollmentDate)
                | _ -> students.OrderBy(fun s->s.LastName)
             
            let pageSize = 3; 
            let students'' = students'.AsNoTracking()
            let! model = PaginatedResizeArray.CreateAsync(students'', pageNumber' |> Option.defaultValue 1, pageSize)
            return upcast this.View(model)
        }

    member this.Details (id:int Nullable) : MVCTask =
        task {
            match Option.ofNullable id with
            | None -> return upcast this.NotFound()   
            | Some id' ->
                let! student = 
                    context
                        .Students
                        .Include(fun s->s.Enrollments :> seq<_>)
                        .ThenInclude(fun (e:Enrollment)->e.Course)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(fun m-> m.ID = id')
                match student |> Option.ofObj with
                | None -> return upcast this.NotFound()
                | Some student' ->
                    return upcast this.View(student')
        }

    member this.Create():IActionResult = upcast this.View()

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
         
    member this.Edit(id:int Nullable): MVCTask = 
        task {
            match id |> Option.ofNullable with
            | None -> return upcast this.NotFound()
            | Some id' ->
                let! student = context.Students.FindAsync(id')
                match student |> Option.ofObj with
                | None -> return upcast this.NotFound()
                | Some student' ->
                    return upcast this.View(student')
        }

    [<HttpPost;ActionName("Edit");ValidateAntiForgeryToken>]
    member this.EditPost(id:int Nullable) : MVCTask = 
        task {
            match id |> Option.ofNullable with
            | None -> return upcast this.NotFound()
            | Some id' ->
                let! studentToUpdate = context.Students.FirstOrDefaultAsync(fun s -> s.ID = id')
                match studentToUpdate |> Option.ofObj with
                | None -> return upcast this.NotFound()
                | Some studentToUpdate' ->
                    let includeExpr: UpdateExpr<Student> array = 
                        [|
                            UpdateExpr(fun s -> upcast s.FirstMidName)
                            UpdateExpr(fun s -> upcast s.LastName)
                            UpdateExpr(fun s -> upcast s.EnrollmentDate)
                        |]
                    match! this.TryUpdateModelAsync<Student>(studentToUpdate', "", includeExpr) with
                    | true ->
                        try
                            let! _ = context.SaveChangesAsync()
                            return upcast this.RedirectToAction(nameof Index)
                        with :? DbUpdateException ->
                            this.ModelState.AddModelError("", "Unable to save changes. " +
                                "Try again, and if the problem persists " +
                                "see your system administrator.");    
                            return upcast this.View(studentToUpdate')     
                    | false -> return upcast this.View(studentToUpdate')    
        }
        

    member this.Delete(id:int Nullable, 
        [<Optional;DefaultParameterValue(false)>] saveChangesError: bool) : MVCTask =
        task {
            match id |> Option.ofNullable with
            | None -> return upcast this.NotFound()
            | Some id' ->
                let! student = context.Students.FirstOrDefaultAsync(fun s -> s.ID = id')
                match student |> Option.ofObj with
                | None -> return upcast this.NotFound()
                | Some student' ->
                    if saveChangesError then
                        this.ViewData.["ErrorMessage"] <-
                                "Delete failed. Try again, and if the problem persists " +
                                "see your system administrator."
                    return upcast this.View(student')
        }
        


    [<HttpPost;ActionName("Delete");ValidateAntiForgeryToken>]
    member this.DeleteConfirmed(id:int) : MVCTask = 
        task {
            let! student = context.Students.FindAsync(id)
            match student |> Option.ofObj with
            | None -> return upcast this.RedirectToAction(nameof this.Index)
            | Some student' -> 
                try
                    context.Remove(student') |> ignore
                    let! _ = context.SaveChangesAsync()
                    return upcast this.RedirectToAction(nameof this.Index)
                with :? DbUpdateException ->
                    return upcast this.RedirectToAction(nameof this.Delete, {| id = id; saveChangesError = true |})
        }