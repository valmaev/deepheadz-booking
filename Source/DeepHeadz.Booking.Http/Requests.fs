module DeepHeadz.Booking.Http.Requests

open System
open System.Collections.Generic
open DeepHeadz.Booking.Core
open DeepHeadz.Booking.Core.Domain
open DeepHeadz.Booking.Core.Spatial

[<CLIMutable>]
type RoomsRequest = { 
    Longitude: float<deg>
    Latitude: float<deg>
    Radius: float<km>
    CheckIn: DateTimeOffset
    CheckOut: DateTimeOffset
    MinAvailabilityRatio: float }
[<CLIMutable>]
type AvailabilityResponse = { DaysUnavailable: DateTimeOffset seq; Ratio: float }
[<CLIMutable>]
type RoomsResponse = { Room: Room; Availability: AvailabilityResponse }
[<CLIMutable>]
type RoomsByMaxGuestsResponse = { 
    Rooms: IDictionary<int, RoomsResponse seq>
    TotalNumberOfRooms: int
    TotalNumberOfGuests: int
    AverageAvailability: float }

let bookingLength request = (request.CheckOut - request.CheckIn).Days

let toAvailabilityResponse 
    (roomAvailabilities: IDictionary<int, IDictionary<DateTimeOffset, RoomAvailability seq>>) 
    request 
    room =
        let length = bookingLength request
        match roomAvailabilities.TryGetValue room.Id with
        | (true, v) -> 
            let daysUnavailable = 
                v.Keys
                |> Seq.filter (fun d -> 
                    (d >= request.CheckIn) 
                    && (d <= request.CheckOut) 
                    && ((v.[d] |> Seq.exactlyOne).Available <> true))
            {
                DaysUnavailable = daysUnavailable
                Ratio = (float ((length - (daysUnavailable |> Seq.length))) / float length)
            }
        | (false, v) -> { DaysUnavailable = v.Keys; Ratio = 0.0 }

let toRoomsResponse 
    (roomAvailabilities: IDictionary<int, IDictionary<DateTimeOffset, RoomAvailability seq>>) 
    request 
    room =
        let length = bookingLength request
        { 
            Room = room; 
            Availability = toAvailabilityResponse roomAvailabilities request room
        }

let toRoomsByMaxGuestsResponse rooms =
    {
        TotalNumberOfRooms = rooms |> Seq.length
        TotalNumberOfGuests = rooms |> Seq.sumBy (fun r -> r.Room.MaxGuests)
        AverageAvailability = rooms |> Seq.averageBy (fun r -> r.Availability.Ratio)
        Rooms =
            rooms
            |> Seq.groupBy (fun room -> room.Room.MaxGuests)
            |> Seq.sortBy fst
            |> dict
    }
