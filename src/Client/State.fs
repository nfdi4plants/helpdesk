module State

open Elmish
open Fable.Remoting.Client
open Shared
open Fable.SimpleJson

type Model = {
    DropdownIsActive: bool
    DropdownActiveTopic: IssueTypes.IssueCategory option
    // True when user navigates to nested dropdown
    DropdownActiveSubtopic: IssueTypes.IssueTopic option
    FormModel: Form.Model
}

type System.Exception with
    member this.GetPropagatedError() =
        match this with
        | :? ProxyRequestException as exn ->
            let response = exn.ResponseText |> Json.parseAs<{| error:string; ignored : bool; handled : bool |}>
            response.error
        | ex ->
            ex.Message

let curry f a b = f(a,b)

type Msg =
    // UI
    | ToggleIssueCategoryDropdown
    | UpdateDropdownActiveTopic of IssueTypes.IssueCategory option
    | UpdateDropdownActiveSubtopic of IssueTypes.IssueTopic option
    // Form input
    | UpdateFormModel of Form.Model
    // API
    | SubmitIssueRequest
    | SubmitIssueResponse
    | GenericError of Cmd<Msg> * exn

let api =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<IHelpdeskAPI>