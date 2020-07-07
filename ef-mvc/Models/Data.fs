namespace ContosoUniversity.Data

open ContosoUniversity.Models
open Microsoft.EntityFrameworkCore
open Unchecked

type SchoolContext (options:SchoolContext DbContextOptions) =
    inherit DbContext(options)
    
    member val Course = defaultof<Course DbSet> with get,set
    member val Enrollments = defaultof<Enrollemnt DbSet> with get,set
    member val Student = defaultof<Student DbSet> with get,set

    override _.OnModelCreating(modelBuilder) =
        modelBuilder.Entity<Course>().ToTable("Course") |> ignore
        modelBuilder.Entity<Course>().ToTable("Enrollment") |> ignore
        modelBuilder.Entity<Course>().ToTable("Student") |> ignore