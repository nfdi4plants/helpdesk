module CaptchaTypes

type ClientCaptcha = {
    Id          : System.Guid
    ImageBase64 : string
    UserInput   : string
    AccessToken : string
}