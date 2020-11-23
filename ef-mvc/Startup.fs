namespace ContosoUniversity

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.HttpsPolicy;
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open ContosoUniversity.Data
open Microsoft.EntityFrameworkCore
open Microsoft.Extensions.Configuration



type Startup private () =

    new (configuration: IConfiguration) as this =
        Startup() then
        this.Configuration <- configuration

    // This method gets called by the runtime. Use this method to add services to the container.
    member this.ConfigureServices(services: IServiceCollection) =
        // Add framework services.
        
        services.AddDbContext<SchoolContext>(
                fun options -> 
                    this.Configuration.GetConnectionString("DefaultConnection") 
                    |> options.UseSqlite 
                    |> ignore) 
                .AddControllersWithViews().AddRazorRuntimeCompilation() |> ignore

        services.AddRazorPages() |> ignore

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    member this.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =

        (if (env.IsDevelopment()) then
            app.UseDeveloperExceptionPage()
        else
            app.UseExceptionHandler("/Home/Error")
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
               .UseHsts()
        )
            .UseHttpsRedirection()
            .UseStaticFiles() 

            .UseRouting()

            .UseAuthorization()

            .UseEndpoints(fun endpoints ->
                endpoints.MapControllerRoute(
                    name = "default",
                    pattern = "{controller=Home}/{action=Index}/{id?}") |> ignore
                endpoints.MapRazorPages() |> ignore)

            |> ignore

    member val Configuration : IConfiguration = null with get, set
