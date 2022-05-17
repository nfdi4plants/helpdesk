module LoggerFunctions

open FabulousMinutes

//let printLogger =
//    fun (logger: Logger) -> logger.ToJson() |> printfn "%A"

//let printLoggerInTemplate =
//    fun (logger: Logger) ->
//        let templateStr = """fabulous-minutes -- Response {Response.StatusCode}. Request at {Request.Path}, {Timestamp} with body: {Request.Body}"""
//        logger.ToTemplate(templateStr)
//        |> printfn "%A"

open Microsoft.Data.Sqlite

let connectionString = @"Data Source=Application.db;Cache=Shared"

let connection = new SqliteConnection(connectionString)

module LoggerWrite =
    module Commands = 

        let pathSanitizer = fun (p: string) -> p.Replace('\\','_').Replace('/','_').Trim([|'_'|])

        let createApiPathTable (path:string) =
            // id : https://www.sqlite.org/autoinc.html
            $"""
                CREATE TABLE IF NOT EXISTS path_{pathSanitizer path} (
                    id INTEGER PRIMARY KEY,
                    http_response_code INTEGER,
                    timestamp TEXT DEFAULT CURRENT_TIMESTAMP,
                    eval_length TEXT,
                    body TEXT DEFAULT ''
                );
            """

        let writeLogTo (path:string) =
            $"""
                INSERT INTO path_{pathSanitizer path} (http_response_code,timestamp,eval_length,body) VALUES (
                  $code,
                  $ts,
                  $length,
                  $body
                );
            """

        let dropTable (path:string) =
            $"""
                DROP TABLE {path};
            """


    let logToSqlite(logger:Logger) =
        let p = logger.GetPath()
        let body =
            match p with
            | "/api/IHelpdeskAPI/checkCaptcha" ->
                logger.DynamicAccess<DynamicObj.DynamicObj> "Request.Body.root^?" |> DynamicObj.toJson
            | _ ->
                logger.DynamicAccess<Logger> "Request.Body" |> fun x -> x.ToJson()
        let code = logger.DynamicAccess "Response.StatusCode"
        let ts = logger.DynamicAccess "Timestamp"
        let length = logger.DynamicAccess "Response.Time"
        connection.Open()

        try 

            let command = connection.CreateCommand()

            // create tabl, if not exists
            command.CommandText <- Commands.createApiPathTable p //"/api/IHelpdeskAPI/getCaptcha"

            command.ExecuteNonQuery() |> ignore

            // write log to it
            command.CommandText <- Commands.writeLogTo p

            let _ = command.Parameters.AddWithValue("$body", body)
            let _ = command.Parameters.AddWithValue("$code", code)
            let _ = command.Parameters.AddWithValue("$ts", ts)
            let _ = command.Parameters.AddWithValue("$length", length) 

            command.ExecuteNonQuery() |> printfn "fabulous-minutes: Logged %i to %s" <| p
            connection.Close()
        with
            | ex ->
                connection.Close()
                failwithf "%A" ex

    let deleteLogTable (path:string) =
        connection.Open()

        try 

            let command = connection.CreateCommand()

            // create tabl, if not exists
            command.CommandText <- Commands.dropTable path //"/api/IHelpdeskAPI/getCaptcha"

            let res = command.ExecuteNonQuery()
            connection.Close()
            res
        with
            | ex ->
                connection.Close()
                failwithf "%A" ex


let sqliteLogger =
    fun (logger:Logger) ->
        LoggerWrite.logToSqlite logger

module LoggerRead =

    module Commands =

        let showAllTablesCommand =
            """
                SELECT 
                    name
                FROM 
                    sqlite_schema
                WHERE 
                    type ='table' AND 
                    name NOT LIKE 'sqlite_%';
            """

        let getLogFromTableCommand (tableName:string) =
            $"""
                SELECT * FROM {tableName}
            """
            

    let getAllLoggerTables() =
        connection.Open()

        try 

            let command = connection.CreateCommand()

            command.CommandText <- Commands.showAllTablesCommand //"/api/IHelpdeskAPI/getCaptcha"

            let reader = command.ExecuteReader()

            let tables =
                [
                    while reader.Read() do
                        yield reader.GetString(0)
                ]
            connection.Close()
            tables
        with
            | ex ->
                connection.Close()
                failwithf "%A" ex

    let getLogsFromTable (table:string) =

        connection.Open()

        try
            let command = connection.CreateCommand()

            command.CommandText <- Commands.getLogFromTableCommand table

            let reader = command.ExecuteReader()

            let logColumnNames =
                let nColumns = reader.FieldCount
                [
                    for i in 0 .. nColumns - 1 do
                        yield reader.GetName(i)
                ]

            let logs =
                [
                    while reader.Read() do
                        let nColumns = reader.FieldCount
                        yield [
                            for i in 0 .. nColumns - 1 do
                                yield reader.GetValue(i)
                        ]
                ]
            connection.Close()
            logColumnNames, logs
        with
            | ex ->
                connection.Close()
                failwithf "%A" ex
            
        

