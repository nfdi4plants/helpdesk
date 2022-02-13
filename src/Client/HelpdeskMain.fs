module HelpdeskMain

open Fable.React
open Elmish
open Fable.Remoting.Client
open State
open Shared
open IssueTypes
open Feliz
open Feliz.Bulma

module ButtonDropdown =

    let subcategories (model:Model) dispatch (block: IssueTypes.IssueCategory) =
        let subcategories = block.subcategories
        Html.div [
            prop.className "nested-dropdown"
            prop.children [
                for topic in subcategories do
                    let subCText =
                       match topic with
                       | IssueTopic.RDM rdm -> rdm.toString
                       | IssueTopic.Infrastructure i -> i.toString
                       | IssueTopic.Metadata m -> m.toString
                       | IssueTopic.Tools t -> t.toString
                       | IssueTopic.Workflows w -> w.toString
                       /// This could happen when somehow IssueTopic.Other gets this element created
                       | other -> failwith $"Could not match {other} with issue subcategories."
                    yield
                        Html.div [
                            prop.onClick (fun e ->
                                // prevent main element "dropdwon toggle"
                                e.stopPropagation()
                                ToggleIssueCategoryDropdown |> dispatch
                                let nextModel = {
                                    model.FormModel with
                                        IssueTopic = Some topic
                                }
                                UpdateFormModel nextModel |> dispatch
                            )
                            prop.text subCText
                        ]
            ]
        ]
        
    let createDropdownItem (model:Model) (dispatch:Msg -> unit) (block: IssueTypes.IssueCategory)  =
        Bulma.dropdownItem.a [
            prop.style [style.paddingRight(length.rem 1)]
            prop.onClick(fun e ->
                // prevent main element "dropdwon toggle"
                e.stopPropagation()
                let subMore =
                    match block with
                    | IssueCategory.RDM                 -> IssueTopic.RDM IssueSubcategories.RDM.More
                    | IssueCategory.Infrastructure      -> IssueTopic.Infrastructure IssueSubcategories.Infrastructure.More
                    | IssueCategory.Metadata            -> IssueTopic.Metadata IssueSubcategories.Metadata.More
                    | IssueCategory.Tools               -> IssueTopic.Tools IssueSubcategories.Tools.More
                    | IssueCategory.Workflows           -> IssueTopic.Workflows IssueSubcategories.Workflows.More
                    | IssueCategory.Other               -> IssueTopic.Other
                ToggleIssueCategoryDropdown |> dispatch
                let nextModel = {
                    model.FormModel with
                        IssueTopic = Some subMore
                }
                UpdateFormModel nextModel |> dispatch
            )
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
                if block <> IssueCategory.Other then
                    subcategories model dispatch block
            ]
        ]

    let createDropdown (model:Model) (dispatch:Msg -> unit) =
        Bulma.control.div [
            prop.children [
                Bulma.dropdown [
                    if model.DropdownIsActive then Bulma.dropdown.isActive
                    prop.children [
                        Bulma.dropdownTrigger [
                            Bulma.button.a [
                                let title =
                                    if model.FormModel.IssueTopic.IsNone then
                                        "Choose a topic"
                                    else
                                        $"{model.FormModel.IssueTopic.Value.toCategoryString} > {model.FormModel.IssueTopic.Value.toSubCategoryString}"
                                prop.title title
                                prop.onClick (fun e -> e.stopPropagation(); dispatch ToggleIssueCategoryDropdown)
                                prop.children [
                                    Html.span [
                                        prop.style [style.marginRight (length.px 5)]
                                        prop.text (
                                            let isMore = model.FormModel.IssueTopic.Value.toSubCategoryString = "More"
                                            if model.FormModel.IssueTopic.IsNone then
                                                "Please select"
                                            elif isMore then
                                                $"{model.FormModel.IssueTopic.Value.toCategoryString} > {model.FormModel.IssueTopic.Value.toSubCategoryString}"
                                            else
                                                model.FormModel.IssueTopic.Value.toSubCategoryString
                                        )
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

let myCheckradio (model:Model) dispatch (name:string) (issueType:IssueType) =
    [
        let text = issueType.ToString()
        let id = $"{name}{text}"
        Checkradio.radio [
            prop.id id
            prop.name name
            color.isBlack
            checkradio.isCircle
            prop.isChecked (model.FormModel.IssueType = issueType)
            prop.onClick(fun e ->
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
                    yield! myCheckradio model dispatch "radio1" IssueType.Question
                    yield! myCheckradio model dispatch "radio1" IssueType.Bug
                    yield! myCheckradio model dispatch "radio1" IssueType.Request
                ]
            ]
        ]
    ]

let issuetitleInputElement model dispatch =
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
                                prop.onChange(fun (e:Browser.Types.Event) ->
                                    let nextFormModel = {
                                        model.FormModel with
                                            IssueTitle = e.Value
                                    }
                                    UpdateFormModel nextFormModel |> dispatch
                                )
                            ]
                        ]
                    ]
                ]
            ]
        ]
    ]

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

let emailInput (model:Model) dispatch =
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
                        prop.placeholder "Email"
                        prop.onChange(fun (e:Browser.Types.Event) ->
                            let nextFormModel = {
                                model.FormModel with
                                    Email = e.Value
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

let captchaANDsubmit (model:Model) dispatch =
    Bulma.content [
        Bulma.box [
            Bulma.columns [
                // Captcha
                Bulma.column [prop.text "captcha"]
                // Submits
                Bulma.column [
                    Bulma.button.button [
                        color.isInfo
                        Bulma.button.isFullWidth
                        prop.text "Submit"
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
            issueTypeElement model dispatch
            issuetitleInputElement model dispatch
            issueContentElement model dispatch
            emailInput model dispatch
            captchaANDsubmit model dispatch
            //Html.div $"{model.FormModel}"
        ]
    ]