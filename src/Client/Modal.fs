module Modal

open Fable.React
open Elmish
open Fable.Remoting.Client
open State
open Shared
open IssueTypes
open Feliz
open Feliz.Bulma

let loadingModal (model:Model) dispatch =
    Bulma.modal [
        prop.id "modal-sample"
        if model.LoadingModal then Bulma.modal.isActive
        prop.children [
            Bulma.modalBackground []
            Html.span [
                prop.style [style.color "white"; style.zIndex 31]
                prop.classes ["icon fa-6x fa-spin"]
                prop.children [
                    Html.i [
                        prop.className "fa-solid fa-circle-notch"
                    ]
                ]
            ]
            Bulma.modalClose [ prop.onClick(fun _ -> UpdateLoadingModal false |> dispatch) ]
        ]
    ]