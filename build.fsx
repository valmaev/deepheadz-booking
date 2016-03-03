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

"Clean"
    ==> "Build"

RunTargetOrDefault "Build"
