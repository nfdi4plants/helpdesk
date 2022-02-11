module State

open Elmish
open Fable.Remoting.Client
open Shared

type FormModel = {
    IssueType : IssueTypes.IssueType
    IssueCategory : IssueTypes.IssueCategory option
    IssueSubcategory : IssueTypes.IssueSubcategory option
} with
    static member init() = {
        IssueType           = IssueTypes.Question
        IssueCategory       = None
        IssueSubcategory    = None
    }

type Model = {
    DropdownIsActive: bool
    FormModel: FormModel
    Todos: Todo list;
    Input: string
}

type Msg =
    | ToggleIssueCategoryDropdown
    | GotTodos of Todo list
    | SetInput of string
    | AddTodo
    | AddedTodo of Todo

let todosApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<ITodosApi>