module CaptchaStore

open System


open Captcha

type Captcha = {
    Id          : Guid
    Cleartext   : string
    ImageBase64 : string
    Accesstoken : string
    CreatedAt   : DateTime
} with
    member this.toClientType :CaptchaTypes.ClientCaptcha = {
        Id          = this.Id
        ImageBase64 = this.ImageBase64
        UserInput   = ""
        AccessToken = ""
    }

type Storage() =
    let captchas = ResizeArray<Captcha>()

    member this.GetCaptchas() =
        this.removeOldCaptchas()
        List.ofSeq captchas

    member this.GenerateCaptcha() =
        this.removeOldCaptchas()
        let clearText = Captcha.createCaptchaString 7
        let captcha = {
            Id          = Guid.NewGuid()
            Cleartext   = clearText
            ImageBase64 = Captcha.createCaptchaImgBase64(clearText)
            Accesstoken = Captcha.createToken(32)
            CreatedAt   = DateTime.Now.ToUniversalTime()
        }
        captchas.Add captcha
        captcha.toClientType

    member this.GetCaptcha(id:Guid) =
        this.removeOldCaptchas()
        let m =
            List.ofSeq captchas
            |> List.map (fun x -> x.Id, x)
            |> Map.ofList
        m.[id]

    member __.RemoveCaptcha(item:Captcha) =
        captchas.Remove(item)

    member __.removeOldCaptchas() =
        let now = DateTime.Now.ToUniversalTime()
        let clearedList =
            List.ofSeq captchas
            /// remove captchas older than 24h
            |> List.choose (fun storedCaptcha ->
                let timeSinceCreated = now.Subtract(storedCaptcha.CreatedAt)
                if timeSinceCreated.Hours >= 24 then None else Some storedCaptcha
            )
        captchas.Clear()
        captchas.AddRange clearedList

        
