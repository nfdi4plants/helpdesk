module State

open Elmish
open Fable.Remoting.Client
open Shared

type FormModel = {
    IssueType : IssueTypes.IssueType
    IssueCategories : IssueTypes.IssueCategory
    IssueSubcategory : IssueTypes.IssueSubcategory
}

type Model = { Todos: Todo list; Input: string }

type Msg =
    | GotTodos of Todo list
    | SetInput of string
    | AddTodo
    | AddedTodo of Todo

let todosApi =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<ITodosApi>