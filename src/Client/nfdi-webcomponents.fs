module nfdi_webcomponents

open Fable.Core
open Fable.Core.JS
open Fable.Core.JsInterop

let _ = import "Navbar" "@nfdi4plants/web-components"

open Fable.React

let nfdiNavbar props children = domEl "nfdi-navbar" props children