module Index

open Elmish
open Fable.Remoting.Client
open Shared

open State


let init () : Model * Cmd<Msg> =
    let model = {
        DropdownIsActive = false
        FormModel = Form.Model.init();
        DropdownActiveTopic = None
        DropdownActiveSubtopic = None
    }
    model, Cmd.none

let update (msg: Msg) (model: Model) : Model * Cmd<Msg> =
    match msg with
    // UI
    | ToggleIssueCategoryDropdown ->
        { model with
            DropdownIsActive = not model.DropdownIsActive
            DropdownActiveTopic = None
            DropdownActiveSubtopic = None },
        Cmd.none
    | UpdateDropdownActiveTopic next ->
        { model with
            DropdownActiveTopic = next },
        Cmd.none
    | UpdateDropdownActiveSubtopic next ->
        { model with
            DropdownActiveSubtopic = next },
        Cmd.none
    // Form input
    | UpdateFormModel nextFormModel ->
        { model with
            FormModel = nextFormModel },
        Cmd.none
    // API
    | SubmitIssueRequest ->
        let cmd =
            Cmd.OfAsync.either
                api.submitIssue
                    model.FormModel
                    (fun () -> SubmitIssueResponse)
                    (curry GenericError Cmd.none)
        model, cmd
    | SubmitIssueResponse ->
        Alerts.submitSuccessfullyAlert()
        let nextModel = fst <| init()
        model, Cmd.none
    | GenericError (nextCmd,exn) ->
        Alerts.genericErrorAlert(exn)
        model, nextCmd
        

        
        

open Feliz
open Feliz.Bulma

let navBrand =
    Bulma.navbarBrand.div [
        Bulma.navbarItem.a [
            prop.href "https://safe-stack.github.io/"
            navbarItem.isActive
            prop.children [
                Html.img [
                    prop.src "/favicon.png"
                    prop.alt "Logo"
                ]
            ]
        ]
    ]

let view (model: Model) (dispatch: Msg -> unit) =
    Bulma.hero [
        hero.isFullHeight
        prop.onClick(fun e ->
            if model.DropdownIsActive then dispatch ToggleIssueCategoryDropdown
        )
        prop.children [
            Bulma.heroHead [
                nfdi_webcomponents.nfdiNavbar [] []
            ]
            Bulma.heroBody [
                Bulma.container [
                    Bulma.column [
                        column.is8
                        column.isOffset2
                        prop.children [
                            HelpdeskMain.mainElement model dispatch
                        ]
                    ]
                ]
            ]
            nfdi_webcomponents.nfdiFooter [] []
        ]
    ]
