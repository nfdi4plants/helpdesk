module MSInterop

open Azure.Identity
open Microsoft.Graph
open FSharp.Control.Tasks

let scopes = [| "https://graph.microsoft.com/.default" |]

// Initialize the ms-graph api connection
let private initGraphClient scopes (authCodeCredential:UsernamePasswordCredential) =
    GraphServiceClient(authCodeCredential, scopes)

// Returns a bucket-name-to-Id mapping
let private getBuckets (planId) (graphClient:GraphServiceClient)  = 
    task {
        let! buckets = graphClient.Planner.Plans.Item(planId).Buckets.Request().GetAsync()
        let tmp = 
            buckets
            |> Seq.map (fun bucket -> bucket.Name,bucket.Id)
            |> Map.ofSeq
        return tmp
    }

// Creates a task with a title in a bucket assosiated to an existing plan
let private createPlanTask planId bucketId title = 
    let planTask = PlannerTask()
    planTask.PlanId <- planId
    planTask.BucketId <- bucketId
    planTask.Title <- title
    planTask

// Adds notes to a planner task
let private addNotes notes (planTask:PlannerTask) =
    let details = PlannerTaskDetails()
    details.Description <- notes
    details.PreviewType <- PlannerPreviewType.Description
    planTask.Details <- details
    planTask

open IssueTypes
open Microsoft.AspNetCore.Http

type Form.Model with
    member this.toMsTarget =
        if this.IssueTopic.IsNone then failwith "Error. Could not find associated topic for issue."
        match this.IssueTopic.Value with
        //| Tools IssueSubtopics.Tools.Swate          -> Targets.MSTeams.Swate
        //| Tools IssueSubtopics.Tools.Swobup         -> Targets.MSTeams.Swobup
        //| Tools IssueSubtopics.Tools.ARCCommander   -> Targets.MSTeams.ArcCommander
        | anyTopic                                  -> Targets.MSTeams.Helpdesk
    member this.toPlannerTask(ctx:HttpContext, (target:Targets.MSTeams.TeamsChannel), graphApi:GraphServiceClient) =
        if this.IssueTopic.IsNone then failwith "Error. Could not find associated topic for issue."
        // match for ms teams info
        let planId = target.GetId ctx
        task {
            let! bucketsMap = graphApi |> getBuckets (planId)

            // bucket
            let bucketName = target.TypeBucketMap.[this.IssueType]
            let bucketId = bucketsMap.[bucketName]
            let updatedTitle = $"{this.IssueTopic.Value.toSubCategoryString} | {this.IssueType} | {this.IssueTitle}"
            let task = 
                createPlanTask planId bucketId updatedTitle
            // add labels if existing
            if target.Labels |> List.isEmpty |> not then
                let! plannerDetails = graphApi.Planner.Plans.Item(planId).Details.Request().GetAsync()
                // labels
                let targetLabels =
                    let desc = plannerDetails.CategoryDescriptions
                    [|desc.Category1; desc.Category2; desc.Category3; desc.Category4; desc.Category5; desc.Category6|]
                    |> Array.indexed
                    |> Array.filter (fun (ind,label) -> target.Labels |> List.exists (fun x -> x = label))
                    |> Array.map fst
                let labels = PlannerAppliedCategories()
                if targetLabels |> Array.contains 0 then labels.Category1 <- true
                if targetLabels |> Array.contains 1 then labels.Category2 <- true
                if targetLabels |> Array.contains 2 then labels.Category3 <- true
                if targetLabels |> Array.contains 3 then labels.Category4 <- true
                if targetLabels |> Array.contains 4 then labels.Category5 <- true
                if targetLabels |> Array.contains 5 then labels.Category6 <- true
                task.AppliedCategories <- labels
            let updatedContent =
                let baseContent = this.IssueContent
                if this.UserEmail <> "" then
                    baseContent + "\n\n" + $"The user requested feedback to: {this.UserEmail}"
                else
                    baseContent
            return
                task |> addNotes updatedContent
        }
        

// Adds the task to a plan using ms-graph api
let private sendPlanTask (graphClient:GraphServiceClient) (planTask:PlannerTask) =
    task {
        return graphClient.Planner.Tasks.Request().AddAsync(planTask)
    }

// #######
// Example
    
let createPlannerTaskInTeams (formModel:Form.Model, ctx:HttpContext) =
    let authCodeCredential = Targets.MSTeams.getMSModel ctx
    let graphApi = initGraphClient scopes authCodeCredential
    let target = formModel.toMsTarget
    task {
        let! plannerTask = formModel.toPlannerTask(ctx,target,graphApi)

        return
            plannerTask
            |> sendPlanTask graphApi
    } 

