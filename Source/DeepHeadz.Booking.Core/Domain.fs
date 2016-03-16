namespace DeepHeadz.Booking.Core

open System

[<CLIMutable>]
type Room = {
    Id: int
    Title: string
    Description: string
    Type: string
    Subtype: string
    Surface: float
    Floor: int
    NumberOfRooms: int
    NumberOfBathrooms: int
    NumberOfDoubleBeds: int
    NumberOfSofaBeds: int
    MinStayInDays: int
    MaxGuests: int

    CountryCode: string
    City: string
    Province: string
    Latitude: float
    Longitude: float

    PricePerNight: decimal
    CurrencyCode: string

    Amenities: string seq
    Services: string seq
    SmokingAllowed: bool
    PetsAllowed: bool

    CheckInTime: string
    CheckOutTime: string
    CancellationPolicy: string

    Images: string seq
    Url: string

    HostId: int
    HostName: string
    HostEmail: string
    HostUrl: string

    CreatedAt: DateTimeOffset
    UpdatedAt: DateTimeOffset }

[<CLIMutable>]
type RoomAvailability = {
    Date: DateTimeOffset
    RoomId: int
    
    Available: bool
    CanCheckIn: bool
    CanCheckOut: bool

    CurrencyCode: string
    NightlyRate: decimal
    WeeklyRate: decimal
    MonthlyRate: decimal

    MinStayInDays: int }
   
module Spatial =
    [<Measure>] type deg
    [<Measure>] type rad
    [<Measure>] type km
     
    let haversine (θ: float<rad>) = 0.5 * (1.0 - Math.Cos(θ / 1.0<rad>))
    let radPerDeg = (Math.PI / 180.0) * 1.0<rad/deg>
     
    type Coordinate(latitude: float<deg>, longitude: float<deg>) =
        member x.Latitude = latitude
        member x.Longitude = longitude
        member x.φ = latitude * radPerDeg
        member x.ψ = longitude * radPerDeg
     
    let earthRadius = 6372.8<km>
     
    let haversineDistance (p1: Coordinate) (p2: Coordinate) =
        2.0 * earthRadius * Math.Asin(
            Math.Sqrt(haversine(p2.φ - p1.φ) +
                Math.Cos(p1.φ / 1.0<rad>) * Math.Cos(p2.φ / 1.0<rad>) * haversine(p2.ψ - p1.ψ)))
    
    let circleGeoSearch (center: Coordinate) (radius: float<km>) (coordinates: Coordinate seq) =
       coordinates
       |> Seq.filter (fun c -> (haversineDistance c center) <= radius)

[<AutoOpen>]
module Domain =
    open Spatial

    let toRoomAvailavilityByRoomIdByDays (availability: RoomAvailability seq) =
        availability
        |> Seq.groupBy (fun a -> a.RoomId)
        |> dict
        |> Seq.map (fun a -> (a.Key, a.Value |> Seq.groupBy (fun x -> x.Date) |> dict))
        |> dict
    
    let toCoordinate room = 
        Coordinate(latitude = room.Latitude * 1.0<deg>, longitude = room.Longitude * 1.0<deg>)
    let haversineDistance (x: Room) (y: Room) =
        (x |> toCoordinate |> haversineDistance <| (y |> toCoordinate))
    let roomGeoSearchByCircle (center: Coordinate) (radius: float<km>) (rooms: Room seq) =
        rooms
        |> Seq.groupBy toCoordinate
        |> Seq.filter (fun c -> (Spatial.haversineDistance (fst c) center) <= radius)
        |> Seq.map snd
        |> Seq.concat
