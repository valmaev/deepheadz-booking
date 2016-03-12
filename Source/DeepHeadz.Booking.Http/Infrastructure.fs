module DeepHeadz.Booking.Http.Infrastructure

open System
open System.Web.Http
open System.Web.Http.Controllers
open System.Web.Http.Dispatcher
open DeepHeadz.Booking.Data.Json
open DeepHeadz.Booking.Http.Controllers

type CompositionRoot(rooms) =
    interface IHttpControllerActivator with
        member x.Create(request, controllerDescriptor, controllerType) =
            match controllerType with
            | t when t = typeof<RoomsController> ->
                new RoomsController(rooms) :> IHttpController
            | t when t = typeof<HomeController> ->
                new HomeController() :> IHttpController
            | _ -> raise
                <| ArgumentException(
                    sprintf "Unknown controllerType requested %O" controllerType,
                    "controllerType")

type HttpRouteDefaults = { Controller: string; Id: obj }

let ConfigureRoutes (config: HttpConfiguration) =
    config.Routes.MapHttpRoute(
        "Default",
        "api/{controller}/{id}",
        { Controller = "Home"; Id = RouteParameter.Optional })
    |> ignore
    config
                    
let ConfigureServices rooms (config: HttpConfiguration) =
    config.Services.Replace(
        typeof<IHttpControllerActivator>,
        new CompositionRoot(rooms))
    config

let ConfigureFormatting (config: HttpConfiguration) =
    config.Formatters.Remove(config.Formatters.XmlFormatter) |> ignore
    config.Formatters.JsonFormatter.SerializerSettings.ContractResolver <-
        PreservingDictionaryCasingContractResolver()
    config

let Configure rooms config = 
    config
    |> ConfigureRoutes
    |> ConfigureServices rooms
    |> ConfigureFormatting