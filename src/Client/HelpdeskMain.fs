module HelpdeskMain

open Elmish
open Fable.Remoting.Client
open State
open Shared
open IssueTypes
open Feliz
open Feliz.Bulma

module ButtonDropdown =

    let subcategories (model:Model) (block: IssueTypes.IssueCategory) =
        let subcategories = block.subcategories
        Html.div [
            prop.className "nested-dropdown"
            prop.children [
                for subC in subcategories do
                    let subCText =
                       match subC with
                       | IssueSubcategory.RDM rdm -> rdm.toString
                       | _ -> ""
                    yield Html.div subCText
            ]
        ]
        
    let createDropdownItem (model:Model) (dispatch:Msg -> unit) (block: IssueTypes.IssueCategory)  =
        Bulma.dropdownItem.a [
            prop.style [style.paddingRight(length.rem 1)]
            prop.children [      
                Html.span [
                    prop.style [
                        style.display.flex
                        style.flexGrow 1
                        style.alignItems.center
                    ]
                    prop.children [
                        Html.span [
                            prop.style [style.marginRight(length.rem 1.5)]
                            prop.text block.toString
                        ]
                        /// 'Other' has no subcategory
                        if block <> IssueCategory.Other then
                            Html.i [
                                prop.style [
                                    style.custom("margin-left","auto")
                                ]
                                prop.className "fa-solid fa-angle-right"
                            ]
                    ]
                ]
                subcategories model IssueCategory.RDM
            ]
        ]

    let createDropdown (model:Model) (dispatch:Msg -> unit) =
        Bulma.control.div [
            Bulma.dropdown [
                if model.DropdownIsActive then Bulma.dropdown.isActive
                prop.children [
                    Bulma.dropdownTrigger [
                        Bulma.button.a [
                            prop.onClick (fun e -> e.stopPropagation(); dispatch ToggleIssueCategoryDropdown)
                            prop.children [
                                Html.span [
                                    prop.style [style.marginRight (length.px 5)]
                                    prop.text ("please select")
                                ]
                                Html.i [ prop.className "fa-solid fa-angle-down" ]
                            ]
                        ]
                    ]
                    Bulma.dropdownMenu [
                        Bulma.dropdownContent [
                            createDropdownItem model dispatch IssueCategory.RDM
                            createDropdownItem model dispatch IssueCategory.Infrastructure
                            createDropdownItem model dispatch IssueCategory.Metadata
                            createDropdownItem model dispatch IssueCategory.Tools
                            createDropdownItem model dispatch IssueCategory.Workflows
                            Bulma.dropdownDivider []
                            createDropdownItem model dispatch IssueCategory.Other
                        ]
                    ]
                ]
            ]
        ]

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

let titleInputElement model dispatch =
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
                    /// This is the dropdown button menu
                    ButtonDropdown.createDropdown model dispatch
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
        prop.style [
            //style.backgroundColor "whitesmoke"
        ]
        prop.children [
            header
            requestTypeElement
            titleInputElement model dispatch
            descriptionElement
        ]
    ]