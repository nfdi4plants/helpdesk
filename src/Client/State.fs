module State

open Elmish
open Fable.Remoting.Client
open Shared

type FormModel = {
    IssueType : IssueTypes.IssueType
    /// Contains Category and Subcategory
    IssueTopic : IssueTypes.IssueTopic option
    IssueTitle : string
    IssueContent: string
    /// If the user wants updated on their issue, they can give us their email
    Email: string
} with
    static member init() = {
        IssueType = IssueTypes.Question
        IssueTopic = None
        IssueTitle = ""
        IssueContent = ""
        Email = ""
    }

type Model = {
    DropdownIsActive: bool
    FormModel: FormModel
}

type Msg =
    // UI
    | ToggleIssueCategoryDropdown
    // Form input
    | UpdateFormModel of FormModel

let todosApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<ITodosApi>