module DeepHeadz.Booking.Http.Controllers

open System
open System.Collections.Generic
open System.Web.Http
open DeepHeadz.Booking.Core
open DeepHeadz.Booking.Core.Spatial

type HomeController() =
    inherit ApiController()
    member x.Get() = "Welcome to DeepHeadz.Booking.Http API!"

type RoomsController
    (rooms: Room seq, 
     roomAvailabilities: IDictionary<int, IDictionary<DateTimeOffset, RoomAvailability seq>>) = 

    inherit ApiController()
    let roomsInMemory = rooms |> Seq.toList
    member x.Get() = roomsInMemory
    member x.Get(latitude: float, longitude: float, radius: float) =
        let center = Coordinate(latitude = latitude * 1.0<deg>, longitude = longitude * 1.0<deg>)
        roomGeoSearchByCircle center (radius * 1.0<km>) roomsInMemory |> Seq.toList
