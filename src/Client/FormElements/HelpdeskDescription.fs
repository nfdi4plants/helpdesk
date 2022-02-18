module HelpdeskMain.Description

open Fable.React
open Elmish
open Fable.Remoting.Client
open State
open Shared
open IssueTypes
open Feliz
open Feliz.Bulma


let issueContentElement (model:Model) dispatch =
    Bulma.content [
        Bulma.box [
            Bulma.label [
                Html.span "Description "
                Html.span [
                    prop.style [style.color NFDIColors.Red.Base]
                    prop.text "*"
                ]
            ]
            Bulma.textarea [
                prop.id InputIds.DescriptionInput
                prop.placeholder "please describe your issue .."
                prop.onChange(fun (e:Browser.Types.Event) ->
                    let nextFormModel = {
                        model.FormModel with
                            IssueContent = e.Value
                    }
                    UpdateFormModel nextFormModel |> dispatch
                )
            ]
        ]
    ]