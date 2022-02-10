module HelpdeskMain

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
            style.backgroundColor "whitesmoke"
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

let myCheckradio (id:string) (name:string) (text:string) =
    [
        Checkradio.radio [
            prop.id id
            prop.name name
            color.isBlack
            checkradio.isCircle
        ]
        Html.label [ prop.htmlFor id; prop.text text ]
    ]

let requestTypeElement =
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
                    yield! myCheckradio "radio1question" "radio1" "Question"
                    yield! myCheckradio "radio1bug" "radio1" "Bug"
                    yield! myCheckradio "radio1request" "radio1" "Request"
                ]
            ]
        ]
    ]

let titleInputElement =
    Bulma.content [
        Bulma.box [
            Bulma.label [
                Html.span "Title "
                Html.span [
                    prop.style [style.color NFDIColors.Red.Base]
                    prop.text "*"
                ]
            ]
            Bulma.field.div [
                field.hasAddons
                prop.children [
                    Bulma.control.p [
                        Bulma.button.button [
                            button.isStatic
                            prop.text "[Question]"
                        ]
                    ]
                    Bulma.control.p [
                        control.isExpanded
                        prop.children [
                            Bulma.input.text [
                                prop.placeholder "please give your issue a title .."
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]

let descriptionElement =
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
                prop.placeholder "please describe your issue .."
            ]
        ]
    ]

let issueCategoryCheckradio (checkradioGroup:string) (cat:IssueCategory) =
    let id = $"{checkradioGroup}{cat.toString.ToLower()}"
    [
        Checkradio.radio [
            prop.id id
            prop.name checkradioGroup
            color.isBlack
            checkradio.isCircle
        ]
        Html.label [ prop.htmlFor id; prop.text cat.toString ]
    ]

let categoryCheckradiosElement =
    Bulma.content [
        Bulma.box [
            Bulma.label [
                Html.span "Category "
                Html.span [
                    prop.style [style.color NFDIColors.Red.Base]
                    prop.text "*"
                ]
            ]
            Bulma.field.div [
                prop.style [style.justifyContent.spaceAround; style.display.flex;]
                prop.children [
                    yield! issueCategoryCheckradio "radio2" IssueCategory.RDM
                    yield! issueCategoryCheckradio "radio2" IssueCategory.Infrastructure
                    yield! issueCategoryCheckradio "radio2" IssueCategory.Tools
                ]
            ]
            Bulma.field.div [
                prop.style [style.justifyContent.spaceAround; style.display.flex;]
                prop.children [
                    yield! issueCategoryCheckradio "radio2" IssueCategory.Workflows
                    yield! issueCategoryCheckradio "radio2" IssueCategory.Metadata
                    yield! issueCategoryCheckradio "radio2" IssueCategory.Other
                ]
            ]
        ]
    ]

let subcategoryDropdown =
    Html.div []

let mainElement (model: Model) (dispatch: Msg -> unit) =
    Bulma.container [
        header
        requestTypeElement
        titleInputElement
        descriptionElement
        categoryCheckradiosElement
    ]