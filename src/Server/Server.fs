module Server

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Saturn
open Shared


open Shared
open Microsoft.AspNetCore.Http

let api = {
    submitIssue = fun (formModel) -> async {
        printfn "%A" formModel
        return ()
    }
}

let errorHandler (ex:exn) (routeInfo:RouteInfo<HttpContext>) =
    let msg = sprintf "%A %s @%s." ex.Message System.Environment.NewLine routeInfo.path
    Propagate msg

let webApp =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromValue api
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

run app
