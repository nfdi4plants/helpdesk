module Targets

module GitHubUrl =

    let Swate = @"https://github.com/nfdi4plants/Swate"

    let Swobup = @"https://github.com/nfdi4plants/Swobup"

    let ArcCommander = @"https://github.com/nfdi4plants/arcCommander"

/// Most values here are sensitive data and are stored in the user secrets.
/// These can be accessed during api calls.
// https://github.com/giraffe-fsharp/Giraffe/blob/master/DOCUMENTATION.md#multiple-environments-and-configuration
// https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows&viewFallbackFrom=aspnetcore-2.2#secret-manager
module MSTeams =

    open Azure.Identity
    open Microsoft.AspNetCore.Http
    open Microsoft.Extensions.Configuration

    open IssueTypes

    let getMSModel (ctx:HttpContext) = 
        let settings = ctx.GetService<IConfiguration>()
        let clientID = settings.["clientId"]
        let tentantID = settings.["tenantId"]
        let serviceBotUsername = settings.["serviceBot-username"]
        let serviceBotPassword = settings.["serviceBot-pw"]
        UsernamePasswordCredential(serviceBotUsername, serviceBotPassword, tentantID, clientID)

    // ids can be found like this: https://powerusers.microsoft.com/t5/image/serverpage/image-id/188342i9A8AD2E8360DAF76/image-dimensions/1700?v=v2&px=-1

    type TeamsChannel = {
        Labels:         string list
        TypeBucketMap:  Map<IssueType,string>
        GetId:          HttpContext -> string
    } with
        static member getId(target:string,ctx:HttpContext) =
            let settings = ctx.GetService<IConfiguration>()
            settings.[target]

        static member createTypeBucketMap(bugBucketStr:string, questionBucketStr:string, requestBucketStr:string) =
            Map [IssueType.Bug, bugBucketStr; IssueType.Question, questionBucketStr; IssueType.Request, requestBucketStr ]

    let Helpdesk : TeamsChannel =  {
        Labels          = List.empty
        TypeBucketMap   = TeamsChannel.createTypeBucketMap("Bugs","Questions","Requests")
        GetId           = fun ctx -> TeamsChannel.getId("target:helpdesk", ctx)
    }

    let Swate : TeamsChannel =
        let issuesStr = "Issues"
        {
            Labels          = ["SWATE"]
            TypeBucketMap   = TeamsChannel.createTypeBucketMap(issuesStr,issuesStr,"Features")
            GetId           = fun ctx -> TeamsChannel.getId("target:Swate", ctx)
        }

    let Swobup : TeamsChannel =
        let issuesStr = "Issues"
        {
            Labels          = ["SWOBUP"]
            TypeBucketMap   = TeamsChannel.createTypeBucketMap(issuesStr,issuesStr,"Features")
            GetId           = fun ctx -> TeamsChannel.getId("target:Swobup", ctx)
        }

    let ArcCommander : TeamsChannel =
        let issuesStr = "Issues"
        {
            Labels          = List.empty
            TypeBucketMap   = TeamsChannel.createTypeBucketMap(issuesStr,issuesStr,"Features")
            GetId           = fun ctx -> TeamsChannel.getId("target:arcCommander", ctx)
        }
