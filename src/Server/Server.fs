module Server

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Saturn
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Configuration

open Shared

open Targets

let api ctx = {
    submitIssue = fun (formModel) -> async {
        if formModel.IssueTopic.IsNone then failwith "Error. Could not find associated topic for issue."
        if formModel.IssueTitle = "" then failwith "Error. Cannot submit issue with empty title"
        printfn "%A" formModel
        try
            MSInterop.createPlannerTaskInTeams(formModel,ctx).Wait()
        with
            | exn -> failwith $"Hit exception: {exn}"
        return ()
    }
}

let errorHandler (ex:exn) (routeInfo:RouteInfo<HttpContext>) =
    let msg = sprintf "%A %s @%s." ex.Message System.Environment.NewLine routeInfo.path
    Propagate msg

let webApp =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromContext api
    |> Remoting.withErrorHandler errorHandler
    |> Remoting.withDiagnosticsLogger(printfn "%A")
    |> Remoting.buildHttpHandler

let app =
    application {
        url "http://0.0.0.0:8085"
        use_router webApp
        memory_cache
        use_static "public"
        use_gzip
    }

app
    .ConfigureAppConfiguration(
        System.Action<Microsoft.Extensions.Hosting.HostBuilderContext,IConfigurationBuilder> ( fun ctx config ->
            config.AddUserSecrets("de50dd48-e691-4599-89ab-9d56efdaaafc") |> ignore
        )
)
|> run
