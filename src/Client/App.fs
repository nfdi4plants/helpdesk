module App

open Elmish
open Elmish.React

open Elmish
open Elmish.UrlParser
open Elmish.Navigation

open Routing

#if DEBUG
open Elmish.Debug
open Elmish.HMR
#endif


Program.mkProgram Index.init Index.update Index.view
|> Program.toNavigable (parsePath Routing.route) Index.urlUpdate
#if DEBUG
|> Program.withConsoleTrace
#endif
|> Program.withReactBatched "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
