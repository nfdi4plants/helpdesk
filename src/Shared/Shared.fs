namespace Shared

open System

module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName


type IHelpdeskAPI = {
    submitIssue: Form.Model * CaptchaTypes.ClientCaptcha -> Async<unit>
    getCaptcha: unit -> Async<CaptchaTypes.ClientCaptcha>
    checkCaptcha: CaptchaTypes.ClientCaptcha ->Async<Result<CaptchaTypes.ClientCaptcha,CaptchaTypes.ClientCaptcha>>
}

