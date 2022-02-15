module Alerts

open State
open Fable.Core.JsInterop
open Feliz.SweetAlert
open Feliz
open Feliz.Bulma

//let titleComp = React.functionComponent (fun (input: {| text: string |}) -> 
//    Html.p [ 
//        prop.className "modal-card-title"

//        prop.children (
//            Html.strong [ 
//                Html.u [ prop.text input.text ] 
//            ]
//        )
//    ])

let bodyComp = React.functionComponent (fun () ->
    Html.div [
        prop.className "modal-card-body"
        
        prop.text "Your ticket was submitted successfully"
    ])


let submitSuccessfullyAlert() =
    Swal.fire [
        swal.icon.success
        swal.html (bodyComp ())
        swal.focusConfirm true
        swal.confirmButtonColor NFDIColors.LightBlue.Base
    ]

let genericErrorAlert(exn:exn) =
    let e = exn.GetPropagatedError()
    Swal.Simple.error e