module Email

open Fable.React
open Elmish
open Fable.Remoting.Client
open State
open Shared
open IssueTypes
open Feliz
open Feliz.Bulma

let userEmailElement (model:Model) dispatch =
    Bulma.content [
        Bulma.box [
            Bulma.label [
                Html.p "Email"
            ]
            Bulma.help [
                prop.text "If you want updates about your issue you can give us your email address. We will keep your contact information only as long as your issue is open."
            ]
            Bulma.control.div [
                Bulma.control.hasIconsLeft
                prop.children [
                    Bulma.input.email [
                        prop.id InputIds.EmailInput
                        prop.placeholder "Email"
                        prop.onChange(fun (e:Browser.Types.Event) ->
                            let nextFormModel = {
                                model.FormModel with
                                    UserEmail = e.Value
                            }
                            UpdateFormModel nextFormModel |> dispatch
                        )
                    ]
                    Html.span [
                        prop.classes ["icon is-small is-left"]
                        prop.children [
                            Html.i [
                                prop.className "fas fa-envelope"
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]
