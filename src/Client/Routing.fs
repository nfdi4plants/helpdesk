module Routing

open IssueTypes

// multi url part with routing ////
type Route =
    //| Root
    | Home of string option

let toRouteUrl route =
    match route with
    | Route.Home (topicOpt) ->
        match topicOpt with
        | Some generalTopic ->
            sprintf "/?topic=%s" generalTopic
        | None ->
            "/"

///explained here: https://elmish.github.io/browser/routing.html
let curry f x y = f (x,y)

module Routing =

    open Elmish.UrlParser

    let route =
        oneOf [
            map Route.Home (s "" <?> stringParam "topic")
        ]

    // Take a window.Location object and return an option of Route

    let parsePath (location:Browser.Types.Location) : Route option = Elmish.UrlParser.parsePath route location