module DeepHeadz.Booking.Http.Controllers

open System.Web.Http
open DeepHeadz.Booking.Core

type HomeController() =
    inherit ApiController()
    member x.Get() = "Welcome to DeepHeadz.Booking.Http API!"

type RoomsController(rooms: Room seq) = 
    inherit ApiController()
    let roomsInMemory = rooms |> Seq.toList
    member x.Get() = roomsInMemory
