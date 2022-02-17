module CaptchaClient

open Fable.React
open Elmish
open Fable.Remoting.Client
open Feliz
open Feliz.Bulma

open State
open Shared
open IssueTypes

let mainElement (model:Model) dispatch =
    Bulma.container [
        Bulma.label [
            Html.p "Captcha"
        ]
        if model.Captcha.IsSome then
            let src = "data:image/png;base64, " + model.Captcha.Value.ImageBase64
            Bulma.field.div [
                prop.style [style.textAlign.center]
                prop.children [
                    Html.img [
                        prop.style [style.width (length.percent 100); style.maxWidth (length.px 400) ]
                        prop.alt "Captcha"
                        prop.src src
                    ]
                ]
            ]
        Bulma.field.div [
            Bulma.field.hasAddons
            prop.children [
                Bulma.control.div [
                    Bulma.button.a [
                        Bulma.button.isSmall
                        prop.classes ["is-nfdidark"]
                        prop.title "Get new Captcha"
                        prop.onClick(fun _ ->
                            dispatch GetCaptcha
                        )
                        prop.children [
                            Html.span [
                                Html.i [prop.classes ["fa-solid fa-repeat"; if model.CaptchaLoading then "fa-spin"]]
                            ]
                        ]
                    ]
                ]
                Bulma.control.div [
                    prop.style [style.width (length.percent 100)]
                    prop.children [
                        Bulma.input.text [
                            prop.id InputIds.CaptchaInput
                            Bulma.input.isSmall
                            prop.placeholder "Captcha"
                            prop.onChange (fun (e:Browser.Types.Event) ->
                                let nextCaptcha = {
                                    model.Captcha.Value with
                                        UserInput = e.Value
                                }
                                UpdateCaptchaClient (Some nextCaptcha) |> dispatch
                            )
                        ]
                    ]
                ]
                Bulma.control.div [
                    if model.Captcha.IsSome && model.Captcha.Value.AccessToken <> "" then
                        Bulma.button.a [
                            Bulma.button.isSmall
                            color.isSuccess
                            prop.title "Captcha correct!"
                            prop.style [style.cursor "default"; style.pointerEvents.none; style.custom("--fa-animation-duration","1s"); style.custom("--fa-animation-iteration-count","2")]
                            prop.onClick(fun e -> e.preventDefault(); e.stopPropagation())
                            prop.children [
                                Html.span [
                                    Html.i [prop.classes ["fa-solid fa-check fa-bounce"]]
                                ]
                            ]
                        ]
                    else
                        Bulma.button.button [
                            Bulma.button.isSmall
                            prop.classes ["is-nfdidark"]
                            prop.title "Check Captcha"
                            prop.onClick(fun _ ->
                                dispatch CheckCaptcha
                            )
                            prop.children [
                                Html.span [
                                    Html.i [prop.classes ["fa-solid fa-share"]]
                                ]
                            ]
                        ]
                ]
            ]
        ]
        if model.CaptchaDoneWrong then
            Bulma.field.div [
                Bulma.notification [
                    color.isDanger
                    prop.children[
                        Bulma.delete [
                            prop.onClick (fun _ -> UpdateCaptchaDoneWrong false |> dispatch)
                        ]
                        Html.p "Ups! That was wrong, try again!"
                    ]
                ]
            ]
    ]
