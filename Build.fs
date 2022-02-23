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

module Docker =
    
    open Fake.Extensions.Release

    let dockerImageName = "freymaurer/nfdi-helpdesk"

    Target.create "docker-publish" (fun _ ->
        let releaseNotesPath = "RELEASE_NOTES.md"
        let port = "8085"

        Release.exists()
        let newRelease = ReleaseNotes.load releaseNotesPath
        let check = Fake.Core.UserInput.getUserInput($"Is version {newRelease.SemVer.Major}.{newRelease.SemVer.Minor}.{newRelease.SemVer.Patch} correct? (y/n/true/false)" )

        let dockerCreateImage() = run docker "build -t nfdi-helpdesk ." ""
        let dockerTestImage() = run docker $"run -it -p {port}:{port} nfdi-helpdesk" ""
        let dockerTagImage() =
            run docker $"tag nfdi-helpdesk:latest {dockerImageName}:{newRelease.SemVer.Major}.{newRelease.SemVer.Minor}.{newRelease.SemVer.Patch}" ""
            run docker $"tag nfdi-helpdesk:latest {dockerImageName}:latest" ""
        let dockerPushImage() =
            run docker $"push {dockerImageName}:{newRelease.SemVer.Major}.{newRelease.SemVer.Minor}.{newRelease.SemVer.Patch}" ""
            run docker $"push {dockerImageName}:latest" ""
        let dockerPublish() =
            Trace.trace $"Tagging image with :latest and :{newRelease.SemVer.Major}.{newRelease.SemVer.Minor}.{newRelease.SemVer.Patch}"
            dockerTagImage()
            Trace.trace $"Pushing image to dockerhub with :latest and :{newRelease.SemVer.Major}.{newRelease.SemVer.Minor}.{newRelease.SemVer.Patch}"
            dockerPushImage()
        /// Check if next SemVer is correct
        match check with
        | "y"|"true"|"Y" ->
            Trace.trace "Perfect! Starting with docker publish"
            Trace.trace "Creating image"
            dockerCreateImage()
            /// Check if user wants to test image
            let testImage = Fake.Core.UserInput.getUserInput($"Want to test the image? (y/n/true/false)" )
            match testImage with
            | "y"|"true"|"Y" ->
                Trace.trace $"Your app on port {port} will open on localhost:{port}."
                dockerTestImage()
                /// Check if user wants the image published
                let imageWorkingCorrectly = Fake.Core.UserInput.getUserInput($"Is the image working as intended? (y/n/true/false)" )
                match imageWorkingCorrectly with
                | "y"|"true"|"Y"    -> dockerPublish()
                | "n"|"false"|"N"   -> Trace.traceErrorfn "Cancel docker-publish"
                | anythingElse      -> failwith $"""Could not match your input "{anythingElse}" to a valid input. Please try again."""
            | "n"|"false"|"N"   -> dockerPublish()
            | anythingElse      -> failwith $"""Could not match your input "{anythingElse}" to a valid input. Please try again."""
        | "n"|"false"|"N" ->
            Trace.traceErrorfn "Please update your SemVer Version in %s" releaseNotesPath
        | anythingElse -> failwith $"""Could not match your input "{anythingElse}" to a valid input. Please try again."""

    )

Target.create "Clean" (fun _ ->
    Shell.cleanDir deployPath
    run dotnet "fable clean --yes" clientPath // Delete *.fs.js files created by Fable
)

Target.create "InstallClient" (fun _ -> run npm "install" ".")

Target.create "Bundle" (fun _ ->
    [ "server", dotnet $"publish -c Release -o \"{deployPath}\"" serverPath
      "client", dotnet "fable src\Client -s --run webpack --config webpack.config.js" "" ]
    |> runParallel
)

Target.create "bundlelinux" (fun _ ->
    [ "server", dotnet $"publish -c Release -r linux-x64 -o \"{deployPath}\"" serverPath
      "client", dotnet "fable src\Client -s --run webpack --config webpack.config.js" "" ]
    |> runParallel
)

// https://github.com/MangelMaxime/fulma-demo/blob/master/build.fsx
Target.create "Run" (fun _ ->
    run dotnet "build" sharedPath
    openBrowser devUrl
    [ "server", dotnet "watch run" serverPath
      "client", dotnet "fable watch src\Client -s --run webpack-dev-server" "" ]
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
        ==> "bundlelinux"

    "Clean"
    ==> "InstallClient"
    ==> "Bundle"

    "Clean"
        ==> "InstallClient"
        ==> "Run"

    "InstallClient"
        ==> "RunTests"

    "release"

    "docker-publish"

]

[<EntryPoint>]
let main args = runOrDefault args