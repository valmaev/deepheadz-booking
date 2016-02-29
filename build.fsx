#I @"packages/FAKE/tools"
#r @"packages/FAKE/tools/FakeLib.dll"

open System
open Fake

let outputDir = "Artifacts"

Target "Clean" (fun _ ->
    CleanDir outputDir)

Target "Build" (fun _ ->
    !! "DeepHeadz.Booking.sln"
    |> MSBuildRelease outputDir "Rebuild"
    |> Log "Build Output: ")

Target "Stage" (fun _ ->
    ExecProcess (fun info ->
        info.FileName <- sprintf "%s/DeepHeadz.Booking.Http.OwinHost.exe" outputDir
        info.WorkingDirectory <- ".") (TimeSpan.FromMinutes 1.0)
    |> ignore)

"Clean"
    ==> "Build"
    ==> "Stage"

RunTargetOrDefault "Build"
