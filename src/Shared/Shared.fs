namespace Shared

open System

module Route =
    let builder typeName methodName =
        sprintf "/api/%s/%s" typeName methodName

    let deconstructor (path:string) =
        let splitArr = path.Split("/",StringSplitOptions.RemoveEmptyEntries)
        splitArr.[1], splitArr.[2]


type IHelpdeskAPI = {
    submitIssue: Form.Model * CaptchaTypes.ClientCaptcha -> Async<unit>
    getCaptcha: unit -> Async<CaptchaTypes.ClientCaptcha>
    checkCaptcha: CaptchaTypes.ClientCaptcha ->Async<Result<CaptchaTypes.ClientCaptcha,CaptchaTypes.ClientCaptcha>>
}

