module State

open Elmish
open Fable.Remoting.Client
open Shared
open Fable.SimpleJson
open System

module InputIds =
    [<Literal>]
    let CaptchaInput = "CaptchaInput"

    [<Literal>]
    let TitleInput = "TitleInput"

    [<Literal>]
    let DescriptionInput = "DescriptionInput"

    [<Literal>]
    let EmailInput = "EmailInput"

    [<Literal>]
    let RadioQuestion = "radio1Question"

    let inputIdList = [
        CaptchaInput
        TitleInput
        DescriptionInput
        EmailInput
    ]

type Model = {
    DropdownIsActive: bool
    LoadingModal: bool
    DropdownActiveTopic: IssueTypes.IssueGeneralTopic option
    // True when user navigates to nested dropdown
    DropdownActiveSubtopic: IssueTypes.Topic option
    FormModel: Form.Model
    Captcha: CaptchaTypes.ClientCaptcha option
    CaptchaLoading: bool
    CaptchaDoneWrong: bool
}

type System.Exception with
    member this.GetPropagatedError() =
        match this with
        | :? ProxyRequestException as exn ->
            let response = exn.ResponseText |> Json.parseAs<{| error:string; ignored : bool; handled : bool |}>
            response.error
        | ex ->
            ex.Message

let curry f a b = f(a,b)

type Msg =
    // UI
    | ToggleIssueCategoryDropdown
    | UpdateDropdownActiveTopic of IssueTypes.IssueGeneralTopic option
    | UpdateDropdownActiveSubtopic of IssueTypes.Topic option
    | UpdateLoadingModal of bool
    | UpdateCaptchaLoading of bool
    | UpdateCaptchaDoneWrong of bool
    // Captcha input
    | UpdateCaptchaClient of CaptchaTypes.ClientCaptcha option
    // Form input
    | UpdateFormModel of Form.Model
    // API
    | SubmitIssueRequest
    | SubmitIssueResponse
    | GenericError of Cmd<Msg> * exn
    | GetCaptcha
    | GetCaptchaResponse of CaptchaTypes.ClientCaptcha option
    | CheckCaptcha
    | CheckCaptchaResponse of Result<CaptchaTypes.ClientCaptcha, CaptchaTypes.ClientCaptcha>

let api =
    Remoting.createApi ()
    |> Remoting.withRouteBuilder Route.builder
    |> Remoting.buildProxy<IHelpdeskAPI>