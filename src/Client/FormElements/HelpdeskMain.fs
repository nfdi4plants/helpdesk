module HelpdeskMain

open Fable.React
open Elmish
open Fable.Remoting.Client
open State
open Shared
open IssueTypes
open Feliz
open Feliz.Bulma


let header =
    Bulma.content [
        prop.style [
            style.backgroundColor NFDIColors.lightgray
            style.padding (length.rem 1, length.rem 2)
        ]
        prop.children [
            Bulma.content [
                Bulma.media [
                    Bulma.mediaLeft [
                        Bulma.image [
                            Bulma.image.is128x128
                            prop.style [style.marginLeft 0]
                            prop.children[
                                Html.img [
                                    prop.src "https://raw.githubusercontent.com/nfdi4plants/Branding/master/logos/DataPLANT/DataPLANT_logo_square_bg_darkblue.svg"
                                ]
                            ]
                        ]
                    ]
                    Bulma.mediaContent [
                        prop.style [
                            style.margin.auto
                        ]
                        prop.children [
                            Bulma.content [
                                Html.h1 [
                                    prop.style [
                                        style.color "grey"
                                    ]
                                    prop.text "Submit a ticket"
                                ]
                            ]
                        ]
                    ]
                ]
            ]
            Bulma.content [
                Html.h5 [
                    prop.text "Please fill out this form to submit a new request to DataPLANT"
                ]
            ]
        ]
    ]

let captchaANDsubmit (model:Model) dispatch =
    Bulma.content [
        Bulma.box [
            Bulma.field.div [
                CaptchaClient.mainElement model dispatch
            ]
            Bulma.field.div [
                // Submits
                Bulma.button.button [
                    let isActive =
                        model.Captcha.IsSome && model.Captcha.Value.AccessToken <> ""
                        && model.FormModel.IssueTitle <> ""
                        && model.FormModel.IssueTopic.IsSome
                        && model.FormModel.IssueContent <> ""
                    prop.onClick (fun _ ->
                        if isActive then dispatch SubmitIssueRequest else ()
                    )
                    prop.classes ["is-nfdidark"]
                    if not isActive then Bulma.button.isStatic
                    Bulma.button.isFullWidth
                    prop.children [
                        Html.span "Submit"
                        Html.span [
                            prop.classes ["icon is-small"]
                            prop.children [
                                Html.i [
                                    prop.className "fa-solid fa-share"
                                ]
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]

let mainElement (model: Model) (dispatch: Msg -> unit) =
    Bulma.container [
        prop.style [
            //style.backgroundColor "whitesmoke"
        ]
        prop.children [
            header
            HelpdeskMain.Checkradio.issueTypeElement model dispatch
            HelpdeskMain.Title.issueTitleElement model dispatch
            HelpdeskMain.Description.issueContentElement model dispatch
            HelpdeskMain.Email.userEmailElement model dispatch
            captchaANDsubmit model dispatch
            //Html.div $"{model.FormModel}"
        ]
    ]