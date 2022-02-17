module Index

open Elmish
open Fable.Remoting.Client
open Shared

open Routing
open State

/// update on top of Model.init() according to entry url
let urlUpdate (route: Route option) (model:Model) =
    match route with
    | Some (Route.Home topicStr) ->
        let nextModel =
            if topicStr.IsNone then
                model
            else
                let parsedTopic = IssueTypes.Topic.ofUrlString topicStr.Value
                let nextFormModel = { model.FormModel with IssueTopic = Some parsedTopic }
                { model with FormModel = nextFormModel }
        nextModel, Cmd.none
    | None -> model, Cmd.none
        

let init(url: Routing.Route option) : Model * Cmd<Msg> =
    let model = {
        DropdownIsActive = false
        LoadingModal = false
        FormModel = Form.Model.init();
        DropdownActiveTopic = None
        DropdownActiveSubtopic = None
    }
    let route = Routing.parsePath Browser.Dom.document.location
    let model, cmd = urlUpdate route model
    model, cmd

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
        let nextModel = fst <| init(None) 
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
