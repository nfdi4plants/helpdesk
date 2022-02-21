module Checkradio

open State
open IssueTypes
open Feliz
open Feliz.Bulma

let private myCheckradio (model:Model) dispatch (name:string) (issueType:IssueType) =
     Html.div [
        let text = issueType.ToString()
        let id = $"{name}{text}"
        Checkradio.radio [
            prop.id id
            prop.name name
            prop.classes ["is-nfdiolive"]
            checkradio.isCircle
            color.isInfo
            prop.isChecked (model.FormModel.IssueType = issueType)
            prop.onChange(fun (e:Browser.Types.Event) ->
                let nextModel = {
                    model.FormModel with
                        IssueType = issueType
                }
                UpdateFormModel nextModel |> dispatch
            )
        ]
        Html.label [ prop.htmlFor id; prop.text text ]
    ]

let issueTypeElement model dispatch =
    Bulma.content [
        Bulma.box [
            Bulma.label [
                Html.span "Type "
                Html.span [
                    prop.style [style.color NFDIColors.Red.Base]
                    prop.text "*"
                ]
            ]
            Bulma.field.div [
                prop.style [style.justifyContent.spaceAround; style.display.flex]
                prop.children [
                    myCheckradio model dispatch "radio1" IssueType.Question
                    myCheckradio model dispatch "radio1" IssueType.Bug
                    myCheckradio model dispatch "radio1" IssueType.Request
                ]
                //  Bulma.columns [
                //  ]
            ]
        ]
    ]
