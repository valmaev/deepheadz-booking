module DeepHeadz.Booking.Http.OwinHost.Self

open Owin
open System
open System.Reflection
open System.Net.Http
open System.Web.Http
open Microsoft.Owin.Hosting

type HelloWorldController() = 
    inherit ApiController()
    member x.Get() = "Hello World!"

type Startup() =
    member x.Configuration (app: IAppBuilder) =
        let config = new HttpConfiguration()
        let route =
            config.Routes.MapHttpRoute(
                "Default",
                "api/{controller}/{id}")
        route.Defaults.Add("id", RouteParameter.Optional)
        app.UseWebApi(config) |> ignore

[<EntryPoint>]
let main argv =
    use server = WebApp.Start<Startup>("http://localhost:8080")
    printfn 
        "%s \n%s v.%s \nServer started. \nPress Enter to shutdown..."  
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
    Console.ReadLine() |> ignore
    0
