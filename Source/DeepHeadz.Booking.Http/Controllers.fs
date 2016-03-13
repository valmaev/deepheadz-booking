module DeepHeadz.Booking.Http.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Web.Http
open DeepHeadz.Booking.Core
open DeepHeadz.Booking.Core.Spatial

type HomeController() =
    inherit ApiController()
    member x.Get() = "Welcome to DeepHeadz.Booking.Http API!"

[<CLIMutable>]
type AvailabilityResponse = { DaysUnavailable: DateTimeOffset seq; Ratio: float }
[<CLIMutable>]
type RoomsResponse = { Room: Room; Availability: AvailabilityResponse }

type RoomsController
    (rooms: Room seq, 
     roomAvailabilities: IDictionary<int, IDictionary<DateTimeOffset, RoomAvailability seq>>) = 
    inherit ApiController()
    let roomsInMemory = rooms |> Seq.toList
    member x.Get
        (latitude: float<deg>, 
         longitude: float<deg>, 
         radius: float<km>, 
         checkIn: DateTimeOffset,
         checkOut: DateTimeOffset,
         minAvailabilityRatio: float) =
        let center = Coordinate(latitude = latitude, longitude = longitude)
        let bookingLength = (checkOut - checkIn).Days
        roomGeoSearchByCircle center radius roomsInMemory
        |> Seq.map (fun r -> 
            { 
                Room = r; 
                Availability = 
                    match roomAvailabilities.TryGetValue r.Id with
                    | (true, v) -> 
                        let daysUnavailable = 
                            v.Keys
                            |> Seq.filter (fun d -> 
                                (d >= checkIn) 
                                && (d <= checkOut) 
                                && ((v.[d] |> Seq.exactlyOne).Available <> true))
                        {
                            DaysUnavailable = daysUnavailable
                            Ratio = 
                                (float ((bookingLength - (daysUnavailable |> Seq.length))) 
                                / float bookingLength)
                        }
                    | (false, v) -> { DaysUnavailable = v.Keys; Ratio = 0.0 }
            })
        |> Seq.filter (fun a -> a.Availability.Ratio >= minAvailabilityRatio)
