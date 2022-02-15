module MSInterop

open Azure.Identity
open Microsoft.Graph
open FSharp.Control.Tasks



let scopes = [| "https://graph.microsoft.com/.default" |]

let authCodeCredential = 
    // Values from app registration
    let tenantId = "" // tenant id (or use "common")
    let clientId = "" // app id
    // User info
    let username = "" 
    let password = ""   

    UsernamePasswordCredential(username, password,tenantId, clientId)


// Initialize the ms-graph api connection
let initGraphClient scopes (authCodeCredential:Azure.Core.TokenCredential) =
    GraphServiceClient(authCodeCredential, scopes)

// Plan ids
module PlanIds =
    let helpdesk = ""

// Returns a bucket-name-to-Id mapping
let getBuckets (planId) (graphClient:GraphServiceClient)  = 
    task {
        let! buckets = graphClient.Planner.Plans.Item(planId).Buckets.Request().GetAsync()
        let tmp = 
            buckets
            |> Seq.map (fun bucket -> bucket.Name,bucket.Id)
            |> Map.ofSeq
        return tmp
    }

// Creates a task with a title in a bucket assosiated to an existing plan
let createPlanTask planId bucketId title = 
    let planTask = PlannerTask()
    planTask.PlanId <- planId
    planTask.BucketId <- bucketId
    planTask.Title <- title
    planTask

// Adds notes to a planner task
let addNotes notes (planTask:PlannerTask) =
    let details = PlannerTaskDetails()
    details.Description <- notes
    planTask.Details <- details

    planTask

// Adds the task to a plan using ms-graph api
let sendPlanTask (graphClient:GraphServiceClient) (planTask:PlannerTask) =
    task {
        return graphClient.Planner.Tasks.Request().AddAsync(planTask)
    }


// #######
// Example
let graphApi = initGraphClient scopes authCodeCredential
    
let pltsk () =
    task {
        let! bucketsMap = graphApi |> getBuckets PlanIds.helpdesk

        return 
            createPlanTask PlanIds.helpdesk bucketsMap.["Issues"]
                "Testing Bugs"
            |> addNotes """Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."""
            |> sendPlanTask graphApi
    } 

pltsk().Wait()
