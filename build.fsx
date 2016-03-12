#I @"packages/FAKE/tools"
#r @"packages/FAKE/tools/FakeLib.dll"

open System
open System.IO
open Fake
open Fake.Testing

let outputDir = "Artifacts"
let testResultsDir = outputDir @@ "TestResults"
let testResultsFileName = testResultsDir @@ "UnitTestResults.xml"

Target "Clean" (fun _ ->
    CleanDir outputDir)

Target "Build" (fun _ ->
    !! "DeepHeadz.Booking.sln"
    |> MSBuildRelease outputDir "Rebuild"
    |> Log "Build Output: ")

Target "RunUnitTests" (fun _ ->
    !! (outputDir @@ "*UnitTests.dll")
    |> xUnit2 (fun p -> { p with ToolPath = "packages" @@ "xunit.runner.console" @@ "tools" @@ "xunit.console.exe" }))

"Clean"
    ==> "Build"
    ==> "RunUnitTests"

RunTargetOrDefault "Build"
