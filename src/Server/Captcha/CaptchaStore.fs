module CaptchaStore

open System
open System.Security.Cryptography

open Captcha

type Captcha = {
    Id          : Guid
    Cleartext   : string
    ImageBase64 : string
    Accesstoken : string
    ServerTime  : DateTime
} with
    static member createToken(size) =
        let mutable byteArr  = 
            let b = Array.init size (fun _ -> byte 0)
            System.Span<Byte>(b)
        RandomNumberGenerator.Fill(byteArr)
        byteArr.ToArray()
        |> System.Convert.ToBase64String

    member this.toClientType :CaptchaTypes.ClientCaptcha = {
        Id          = this.Id
        ImageBase64 = this.ImageBase64
        UserInput   = ""
        AccessToken = ""
    }

type Storage() =
    let captchas = ResizeArray<Captcha>()

    member __.GetCaptchas() = List.ofSeq captchas

    member __.GenerateCaptcha() =
        let clearText = Captcha.createCaptchaString 7
        let captcha = {
            Id          = Guid.NewGuid()
            Cleartext   = clearText
            ImageBase64 = Captcha.createCaptchaImgBase64(clearText)
            Accesstoken = Captcha.createToken(32)
            ServerTime  = DateTime.Now.ToUniversalTime()
        }
        captchas.Add captcha
        captcha.toClientType

    member __.GetCaptcha(id:Guid) =
        let m =
            List.ofSeq captchas
            |> List.map (fun x -> x.Id, x)
            |> Map.ofList
        m.[id]

    member __.RemoveCaptcha(item:Captcha) =
        captchas.Remove(item)
        
