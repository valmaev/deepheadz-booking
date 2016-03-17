module DeepHeadz.Booking.Http.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Net
open System.Net.Http
open System.Reflection
open System.Web.Http
open DeepHeadz.Booking.Core
open DeepHeadz.Booking.Core.Domain
open DeepHeadz.Booking.Core.Spatial
open DeepHeadz.Booking.Http.Requests

type HomeController() =
    inherit ApiController()
    member x.Get() = 
        sprintf "Welcome to %s API v.%s!" 
            (x.GetType().Namespace) 
            (Assembly.GetExecutingAssembly().GetName().Version.ToString())

type RoomsController
    (rooms: Room seq, 
     roomAvailabilities: IDictionary<int, IDictionary<DateTimeOffset, RoomAvailability seq>>) = 
    inherit ApiController()
    let roomsInMemory = rooms |> Seq.toList

    member x.Get([<FromUri>] request) =
        if request.Latitude < -90.0<deg> || request.Latitude  > 90.0<deg>
        then x.Request.CreateResponse(HttpStatusCode.BadRequest, "Latitude should be in range [-90.0, 90.0]")

        elif request.Longitude < -180.0<deg> || request.Longitude > 180.0<deg>
        then x.Request.CreateResponse(HttpStatusCode.BadRequest, "Longitude should be in range [-180.0, 180.0]")

        elif request.CheckIn > request.CheckOut 
        then x.Request.CreateResponse(HttpStatusCode.BadRequest, "CheckIn date should be less than checkOut date")

        elif request.MinAvailabilityRatio < 0.0 || request.MinAvailabilityRatio > 1.0
        then x.Request.CreateResponse(HttpStatusCode.BadRequest, "MinAvailabilityRatio should be in range [0.0, 1.0]")

        else
        let center = Coordinate(latitude = request.Latitude, longitude = request.Longitude)
        let response =
            roomGeoSearchByCircle center request.Radius roomsInMemory
            |> Seq.map (toRoomsResponse roomAvailabilities request)
            |> Seq.filter (fun room -> room.Availability.Ratio >= request.MinAvailabilityRatio)
            |> toRoomsByMaxGuestsResponse
        x.Request.CreateResponse(HttpStatusCode.OK, response)
