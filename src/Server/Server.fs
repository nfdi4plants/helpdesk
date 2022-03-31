module Server

open Fable.Remoting.Server
open Fable.Remoting.Giraffe
open Saturn
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Configuration
open Giraffe
open Shared

open Targets

let CaptchaStore = CaptchaStore.Storage()

open System.IO

let api (ctx: HttpContext) =
    {
        submitIssue = fun (formModel,captcha) -> async {
            let storedCaptcha = CaptchaStore.GetCaptcha(captcha.Id)
            let hasValidToken = captcha.AccessToken = storedCaptcha.Accesstoken
            if not hasValidToken then
                failwith "Error. Captcha access token is no longer valid. Please redo the captcha and try again."
            if formModel.IssueTopic.IsNone then failwith "Error. Could not find associated topic for issue."
            if formModel.IssueTitle = "" then failwith "Error. Cannot submit issue with empty title"
            try
                MSInterop.createPlannerTaskInTeams(formModel,ctx).Wait()
                CaptchaStore.RemoveCaptcha(storedCaptcha) |> ignore
            with
                | exn -> failwith $"Hit exception: {exn}"
            return ()
        }
        getCaptcha = fun () -> async {
            let newCaptcha = CaptchaStore.GenerateCaptcha()
            return newCaptcha
        }
        checkCaptcha = fun clientCaptcha -> async {
            let storedCaptcha = CaptchaStore.GetCaptcha(clientCaptcha.Id)
            let isCorrect = storedCaptcha.Cleartext = clientCaptcha.UserInput.Trim()
            let result =
                if isCorrect then 
                    Ok {clientCaptcha with AccessToken = storedCaptcha.Accesstoken}
                else
                    let wasRemoved = CaptchaStore.RemoveCaptcha(storedCaptcha)
                    let newCaptcha = CaptchaStore.GenerateCaptcha()
                    Error newCaptcha
            return result
        }
    }

let errorHandler (ex:exn) (routeInfo:RouteInfo<HttpContext>) =
    let msg = sprintf "%A %s @%s." ex.Message System.Environment.NewLine routeInfo.path
    Propagate msg

/// doesn't do anything?
let logger = printfn "TEST %A"

let webApp =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.fromContext api
    |> Remoting.withErrorHandler errorHandler
    |> Remoting.withDiagnosticsLogger(logger)
    |> Remoting.buildHttpHandler

////// Enable logging on an exisiting HttpHandler 
//let webAppWithLogging = SerilogAdapter.Enable(webApp)

open FSharp.Control.Tasks
open System
open System.IO
open DynamicObj
open Newtonsoft.Json

let dynamicAccess(dynObject:DynamicObj, accessStr:string) =
    let toDynArr = accessStr.Split(".")
    printfn "dynArr = %A" toDynArr
    let rec access (ind:int) (dynArr:string []) result =
        if ind >= dynArr.Length then
            printfn "1"
            result
        elif ind <> 0 && result = None then
            printfn "2"
            None
        else
            printfn "3"
            let obj = if ind = 0 then dynObject else result.Value :> obj :?> DynamicObj
            let next = obj.TryGetValue(dynArr.[ind])
            access (ind+1) dynArr next
    access 0 toDynArr None

/// https://csbiology.github.io/DynamicObj/
type Logger() =
    inherit DynamicObj()

    static member init(?props) =
        let t = Logger()
        if props.IsSome then
            props.Value |> List.iter t.setProp
            t
        else
            t.setProp("", None)
            t
            
    member this.setProp(key,value) = DynObj.setValueOpt this key value

    /// has issues with nested objects?
    member this.print() = DynObj.print this

    member this.toJson() = this |> JsonConvert.SerializeObject

    member this.ofJson(json:string) =
        let reader = new JsonTextReader(new StringReader(json))
        failwith "Logger.ofJson is not implemented yet"

    member this.ofHttpHandler(app:HttpHandler) =
        let st = System.DateTime.Now.ToUniversalTime()
        fun (next:HttpFunc) (ctx:HttpContext) ->
            task {
                use reader = new StreamReader(ctx.Request.Body)
                /// This will empty 'ctx.Request.Body', which we will have to reinsert afterwards
                let! body = reader.ReadToEndAsync()
                let nextSTREAM =
                    let toBytes = System.Text.Encoding.UTF8.GetBytes(body)
                    new MemoryStream(toBytes)
                //printfn "Text %A" text
                /// return stream back to body so our Fable.Remoting actually has parameters to work with
                ctx.Request.Body <- nextSTREAM
                let! result = app next ctx
                let response =
                    [
                        "StatusCode", string result.Value.Response.StatusCode
                        "Time", (System.DateTime.Now.ToUniversalTime() - st).ToString()
                    ]
                    |> List.map (fun x -> fst x, snd x |> Some)
                let request =
                    let query =
                        let queryLogger = Logger()
                        ctx.Request.Query |> Seq.iter (fun x -> queryLogger.setProp(x.Key, Some x.Value) )
                        queryLogger
                    let headers =
                        let queryLogger = Logger()
                        ctx.Request.Headers |> Seq.iter (fun x -> queryLogger.setProp(x.Key, x.Value |> String.concat "," |> Some ) )
                        queryLogger 
                    let userAgent =
                        ctx.Request.Headers
                        |> Seq.tryFind (fun x -> x.Key ="User-Agent")
                        |> Option.map (fun x -> x.Value |> String.concat ",")
                        |> Option.defaultValue ""
                    let contentType =
                        ctx.Request.Headers
                        |> Seq.tryFind (fun x -> x.Key ="Content-Type")
                        |> Option.map (fun x -> x.Value |> String.concat ",")
                        |> Option.defaultValue ""
                    [
                        "Path", box ctx.Request.Path
                        "PathBase", box ctx.Request.PathBase
                        "Method", box ctx.Request.Method
                        "Host", box ctx.Request.Host.Host
                        "Port",
                            if ctx.Request.Host.Port.HasValue then string ctx.Request.Host.Port.Value else ""
                            |> box
                        "QueryString",
                            if ctx.Request.QueryString.HasValue then string ctx.Request.Host.Port.Value else ""
                            |> box
                        "Query", if ctx.Request.Query.Count > 0 then box query else null
                        "Headers", if ctx.Request.Headers.Count > 0 then box headers else null
                        "UserAgent", box userAgent
                        "ContentType", box contentType
                        "Body", box body
                    ]
                    |> List.map (fun x -> fst x, snd x |> Some)
                let logger =
                    let props =
                        [
                            "Timestamp", st.ToString("yyyy.MM.dd hh:mm:ss.fffff") |> box
                            "Request", Logger.init(request) |> box
                            "Response", Logger.init(response) |> box
                        ]
                        |> List.map (fun x -> fst x, snd x |> Some)
                    Logger.init(props)
                logger.toJson() |> printfn "%A"

                //let testDynAccess = dynamicAccess(logger,"Request.Headers.Cookie")
                //printfn "%A" testDynAccess
                return result
            }

/// https://github.dev/Zaid-Ajaj/Giraffe.SerilogExtensions/blob/master/src/Giraffe.SerilogExtensions/SerilogAdapter.fs
let apiLogger (app:HttpHandler) =
    Logger().ofHttpHandler(app)

let app =
    application {
        url "http://0.0.0.0:8085"
        use_router (apiLogger webApp)
        memory_cache
        use_static "public"
        use_gzip
    }

app
    .ConfigureAppConfiguration(
        System.Action<Microsoft.Extensions.Hosting.HostBuilderContext,IConfigurationBuilder> ( fun ctx config ->
            config.AddJsonFile("helpdesk_config.json",true,true)            |> ignore
            config.AddUserSecrets("de50dd48-e691-4599-89ab-9d56efdaaafc")   |> ignore
        )
)
|> run