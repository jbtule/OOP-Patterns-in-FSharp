namespace ContosoUniversity

open System.Linq
open FSharp.Control.Tasks.V2
open Microsoft.EntityFrameworkCore

type PaginatedResizeArray<'T>(items: 'T seq, count:int, pageIndex, pageSize) =
    inherit ResizeArray<'T>(items)
    let totalPages = (float count) / (float pageSize) |> ceil |> int
    member _.HasPreviousPage = pageIndex > 1
    member _.HasNextPage = pageIndex < totalPages
    member _.PageIndex = pageIndex
    static member CreateAsync(source:'T IQueryable, pageIndex, pageSize) =
        task {
            let! count = source.CountAsync()
            let! items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync()
            return PaginatedResizeArray(items, count, pageIndex, pageSize)
        }