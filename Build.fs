open Fake.Core
open Fake.IO

open Helpers

initializeContext()

let devUrl = "http://localhost:8080"

let sharedPath = Path.getFullName "src\Shared"
let serverPath = Path.getFullName "src\Server"
let clientPath = Path.getFullName "src\Client"
let deployPath = Path.getFullName "deploy"
let sharedTestsPath = Path.getFullName "tests/Shared"
let serverTestsPath = Path.getFullName "tests/Server"
let clientTestsPath = Path.getFullName "tests/Client"

module ProjectInfo =
    
    let gitOwner = "Freymaurer"
    let gitName = "nfdi-helpdesk"

module ReleaseNoteTasks =

    open Fake.Extensions.Release

    //let createAssemblyVersion = Target.create "createvfs" (fun _ ->
    //    AssemblyVersion.create ProjectInfo.gitName
    //)

    let updateReleaseNotes = Target.create "release" (fun config ->
        Release.exists()

        Release.update(ProjectInfo.gitOwner, ProjectInfo.gitName, config)

        let newRelease = ReleaseNotes.load "RELEASE_NOTES.md"
        
        let releaseDate =
            if newRelease.Date.IsSome then newRelease.Date.Value.ToShortDateString() else "WIP"
        
        Fake.DotNet.AssemblyInfoFile.createFSharp  "src/Server/Version.fs"
            [   Fake.DotNet.AssemblyInfo.Title "nfdi-helpdesk"
                Fake.DotNet.AssemblyInfo.Version newRelease.AssemblyVersion
                Fake.DotNet.AssemblyInfo.Metadata ("ReleaseDate",releaseDate)
            ]

        Trace.trace "Update Version.fs done!"
    )

Target.create "Clean" (fun _ ->
    Shell.cleanDir deployPath
    run dotnet "fable clean --yes" clientPath // Delete *.fs.js files created by Fable
)

Target.create "InstallClient" (fun _ -> run npm "install" ".")

Target.create "Bundle" (fun _ ->
    [ "server", dotnet $"publish -c Release -o \"{deployPath}\"" serverPath
      "client", dotnet "fable -o output -s --run webpack -p" clientPath ]
    |> runParallel
)

// https://github.com/MangelMaxime/fulma-demo/blob/master/build.fsx
Target.create "Run" (fun _ ->
    run dotnet "build" sharedPath
    openBrowser devUrl
    [ "server", dotnet "watch run" serverPath
      "client", dotnet "fable watch src/Client -s --run webpack-dev-server" "" ]
    |> runParallel
)

Target.create "RunTests" (fun _ ->
    run dotnet "build" sharedTestsPath
    [ "server", dotnet "watch run" serverTestsPath
      "client", dotnet "fable watch -o output -s --run webpack-dev-server --config ../../webpack.tests.config.js" clientTestsPath ]
    |> runParallel
)

Target.create "Format" (fun _ ->
    run dotnet "fantomas . -r" "src"
)

open Fake.Core.TargetOperators

let dependencies = [
    "Clean"
        ==> "InstallClient"
        ==> "Bundle"

    "Clean"
        ==> "InstallClient"
        ==> "Run"

    "InstallClient"
        ==> "RunTests"

    "release"
]

[<EntryPoint>]
let main args = runOrDefault args