﻿module DeepHeadz.Booking.Http.OwinHost.Self

open Owin
open System
open System.Collections.Generic
open System.IO
open System.Reflection
open System.Web.Http
open Microsoft.Owin.Cors
open Microsoft.Owin.FileSystems
open Microsoft.Owin.Hosting
open Microsoft.Owin.StaticFiles
open DeepHeadz.Booking.Core
open DeepHeadz.Booking.Core.Domain
open DeepHeadz.Booking.Data.FileStore
open DeepHeadz.Booking.Data.Json
open DeepHeadz.Booking.Http.Infrastructure

type Startup() =
    let serializer = createSerializerWithDefaultSettings() :> ISerializer
    let roomStore = FileStore<Room>(serializer, "RoomStore.json")
    let roomAvailabilities =
        use stream = new FileStream("RoomAvailabilityStore.json", FileMode.OpenOrCreate, FileAccess.Read)
        serializer.Deserialize<IDictionary<int, IDictionary<DateTimeOffset, RoomAvailability seq>>> stream

    member x.Configuration (app: IAppBuilder) =
        let config = 
            new HttpConfiguration()
            |> Configure (roomStore :> Room seq) roomAvailabilities
        let fileSystem = PhysicalFileSystem "./DeepHeadz.Booking.Web.UI"
        app.UseCors(CorsOptions.AllowAll)
           .UseFileServer(FileServerOptions(FileSystem = fileSystem))
           .UseWebApi(config) |> ignore

let rec printException (ex: Exception) =
    printfn
        "%s Message: %s \nStackTrace: %s"
        (ex.GetType().FullName)
        ex.Message
        ex.StackTrace
    if ex.InnerException <> null
    then printException ex.InnerException

[<EntryPoint>]
let main argv =
    try
        let hostAddress = if argv.Length = 0 then "http://localhost:8080" else argv.[0]
        use server = WebApp.Start<Startup>(hostAddress)
        printfn
            "%s \n%s v.%s \nServer started at %s. \nPress Ctrl+C to shutdown..."
            @"
▓█████▄ ▓█████ ▓█████  ██▓███   ██░ ██ ▓█████ ▄▄▄      ▓█████▄ ▒███████▒
 ▒██▀ ██▌▓█   ▀ ▓█   ▀ ▓██░  ██▒▓██░ ██▒▓█   ▀▒████▄    ▒██▀ ██▌▒ ▒ ▒ ▄▀░
 ░██   █▌▒███   ▒███   ▓██░ ██▓▒▒██▀▀██░▒███  ▒██  ▀█▄  ░██   █▌░ ▒ ▄▀▒░
 ░▓█▄   ▌▒▓█  ▄ ▒▓█  ▄ ▒██▄█▓▒ ▒░▓█ ░██ ▒▓█  ▄░██▄▄▄▄██ ░▓█▄   ▌  ▄▀▒   ░
 ░▒████▓ ░▒████▒░▒████▒▒██▒ ░  ░░▓█▒░██▓░▒████▒▓█   ▓██▒░▒████▓ ▒███████▒
  ▒▒▓  ▒ ░░ ▒░ ░░░ ▒░ ░▒▓▒░ ░  ░ ▒ ░░▒░▒░░ ▒░ ░▒▒   ▓▒█░ ▒▒▓  ▒ ░▒▒ ▓░▒░▒
  ░ ▒  ▒  ░ ░  ░ ░ ░  ░░▒ ░      ▒ ░▒░ ░ ░ ░  ░ ▒   ▒▒ ░ ░ ▒  ▒ ░░▒ ▒ ░ ▒
  ░ ░  ░    ░      ░   ░░        ░  ░░ ░   ░    ░   ▒    ░ ░  ░ ░ ░ ░ ░ ░
    ░       ░  ░   ░  ░          ░  ░  ░   ░  ░     ░  ░   ░      ░ ░
  ░                                                      ░      ░        "
            (Assembly.GetExecutingAssembly().GetName().Name)
            (Assembly.GetExecutingAssembly().GetName().Version.ToString())
            hostAddress
        while true do Console.ReadLine() |> ignore
    with ex -> printException ex
    0
