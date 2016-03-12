﻿module DeepHeadz.Booking.Http.OwinHost.Self

open Owin
open System
open System.Reflection
open System.Web.Http
open Microsoft.Owin.Hosting
open DeepHeadz.Booking.Core
open DeepHeadz.Booking.Data.FileStore
open DeepHeadz.Booking.Data.Json
open DeepHeadz.Booking.Http.Infrastructure


type Startup() =
    let roomStore = 
        FileStore<Room>(
            createSerializerWithDefaultSettings() :> ISerializer,
            "RoomStore.json") 
    member x.Configuration (app: IAppBuilder) =
        new HttpConfiguration()
        |> Configure (roomStore :> Room seq)
        |> app.UseWebApi
        |> ignore

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
