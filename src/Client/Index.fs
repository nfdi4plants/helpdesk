module Index

open Elmish
open Fable.Remoting.Client
open Shared

open State


let init () : Model * Cmd<Msg> =
    let model = {
        DropdownIsActive = false
        LoadingModal = false
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
    | UpdateLoadingModal isActive ->
        { model with
            LoadingModal = isActive },
        Cmd.none
    // Form input
    | UpdateFormModel nextFormModel ->
        { model with
            FormModel = nextFormModel },
        Cmd.none
    // API
    | SubmitIssueRequest ->
        let nextModel = {
            model with
                LoadingModal = true
        }
        let cmd =
            Cmd.OfAsync.either
                api.submitIssue
                    model.FormModel
                    (fun () -> SubmitIssueResponse)
                    (curry GenericError <| Cmd.ofMsg (UpdateLoadingModal false))
        nextModel, cmd
    | SubmitIssueResponse ->
        Alerts.submitSuccessfullyAlert()
        let nextModel = fst <| init() 
        {model with LoadingModal = false}, Cmd.none
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
            Modal.loadingModal model dispatch
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
