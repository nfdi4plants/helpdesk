module Logging

open Giraffe.ViewEngine

module RawJS =

    let private deleteTableErrorHtml = """<div class="notification is-danger"> Unable to find logs. </div>"""

    let deleteRequest (url:string) =
        sprintf
            """
            function deleteRequest() {
                fetch('%s', {method: 'DELETE',})
                    .then(async response => {
                        const isJson = response.headers.get('content-type')?.includes('application/json');
                        const data = isJson && await response.json();

                        // check for error response
                        if (!response.ok) {
                            // get error message from body or default to response status
                            const error = (data && data.message) || response.status;
                            return Promise.reject(error);
                        }

                        window.location.href = '/logging';
                    })
                    .catch(error => {
                        const element = document.getElementById('error_msg');
                        element.innerHTML = '%s'
                        console.error('There was an error!', error);
                    });
            };
            """ url deleteTableErrorHtml


module Views =

    let container attr child = div [_class "container"; yield! attr] child

    let navbarItem attr child = a [_class "navbar-item"; yield! attr] child

    let content attr child = div [_class "content"; yield! attr] child

    let masterpage subPageContent =
        html [_class "has-navbar-fixed-top"] [
            head [] [
                meta [_charset "utf-8"]
                meta [_name "viewport"; _content "width=device-width, initial-scale=1"]
                title [] [rawText "fabulous-minutes"]
                link [_rel "stylesheet"; _href @"https://cdn.jsdelivr.net/npm/bulma@0.9.4/css/bulma.min.css"]
                style [_type "text/css"; _media "screen"] [
                    rawText """
                        #wrapper {
                            display: flex;
                            min-height: 80vh;
                            flex-direction: column
                        }
                    """
                ]
            ]
            body [] [
                // navbar
                nav [_class "navbar is-link is-fixed-top"] [
                    container [] [
                        div [_class "navbar-brand"] [
                            a [_class "navbar-item"; _href "/logging"] [strong [_style "color: white !important; font-size: 22px"] [rawText "Fabulous-Minutes"]]
                            span [_class "navbar-burger"; attr "data-target" "navbarMenuHeroA"] [
                                span [] []
                                span [] []
                                span [] []
                            ] 
                        ]
                        div [_id "navbarMenuHeroA"; _class "navbar-menu"] [
                            div [_class "navbar-start"] [
                                navbarItem [_href "/"] [rawText "Main Page"]
                                div [_class "navbar-item has-dropdown is-hoverable"] [
                                    a [_class "navbar-link"] [rawText "Logs"]
                                    div [_class "navbar-dropdown"] [
                                        for table in LoggerFunctions.LoggerRead.getAllLoggerTables() do
                                            let l = sprintf "/logging/logs/%s" table
                                            yield navbarItem [_href l] [rawText table]
                                    ]
                                ]
                            ]
                        ]
                    ]
                ]
                // content
                div [_id "wrapper"] [
                    section [_class "section"; _style "flex: 1"] [
                        container [] [
                            subPageContent
                        ]
                    ]
                ]
                // footer
                footer [_class "footer"] [
                    div [_class "content has-text-centered"] [
                        p [] [
                            rawText "Built with "
                            strong [] [rawText "BULMA "]
                            img [_src "https://bulma.io/assets/Bulma%20Icon.svg"; _width "10"]
                            rawText ", "
                            strong [] [rawText "Giraffe 🦒"]
                            rawText " and "
                            strong [] [rawText "F# |> ❤️"]
                            rawText ""
                            rawText "."
                        ]
                    ]
                ]
            ]
        ]

    let index =
        content [] [
            h1 [] [rawText "Hello! 📚"]
            p [] [rawText "Go ahead and check out the logs written by fabulous-minutes!"]
        ]

    let logsPage (logsPath:string) =
        let js = RawJS.deleteRequest logsPath
        let tables() = LoggerFunctions.LoggerRead.getAllLoggerTables()
        // do this to avoid sql injection in 'getLogFromTableCommand' 
        if List.contains logsPath (tables()) then
            let logColNames, logs = LoggerFunctions.LoggerRead.getLogsFromTable logsPath
            div [] [
                script [_type "application/javascript"] [
                    rawText js
                ]
                h3 [_class "title is-4 is-spaced"] [rawText logsPath]
                div [_class "field"; _id "error_msg"] []
                div [_class "field"] [
                    a [_onclick "deleteRequest();"; _class "button is-danger"; _style "margin-left: auto; width: 100px; display: block"] [
                        str "delete"
                    ]
                ]
                div [_class "table-container"] [
                    table [_class "table is-striped is-hoverable is-fullwidth"] [
                        thead [] [tr [] [
                            for column in logColNames do
                                yield th [] [rawText column]
                        ]]
                        tfoot [] [tr [] [
                            for column in logColNames do
                                yield th [] [rawText column]
                        ]]
                        tbody [] [
                            for log in logs do
                                yield tr [] [
                                    for col in log do yield td [] [rawText (string col)]
                                ]
                        ]
                    ]
                ]
            ]
        else
            div [_class "notification is-danger"] [
                rawText $"Unable to find logs for '{logsPath}'."
            ]