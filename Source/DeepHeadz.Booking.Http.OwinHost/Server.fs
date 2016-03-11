module DeepHeadz.Booking.Http.OwinHost.Self

open Owin
open System
open System.Reflection
open System.Net.Http
open System.Web.Http
open Microsoft.Owin.Hosting
open Newtonsoft.Json.Serialization

type HelloWorldController() = 
    inherit ApiController()
    member x.Get() = "Hello World!"

type Startup() =
    member x.Configuration (app: IAppBuilder) =
        let config = new HttpConfiguration()
        config.Formatters.JsonFormatter.SerializerSettings.ContractResolver <-
            new CamelCasePropertyNamesContractResolver()
        let route =
            config.Routes.MapHttpRoute(
                "Default",
                "api/{controller}/{id}")
        route.Defaults.Add("id", RouteParameter.Optional)
        app.UseWebApi(config) |> ignore

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
