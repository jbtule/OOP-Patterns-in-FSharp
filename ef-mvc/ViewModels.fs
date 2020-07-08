namespace ContosoUniversity.ViewModels

open System

type ErrorViewModel =
    { RequestId: string }

    member this.ShowRequestId =
        not (String.IsNullOrEmpty(this.RequestId))

type EnrollmentDateGroup  =
    { 
       EnrollmentDate: DateTime Nullable
       StudentCount: int
    }