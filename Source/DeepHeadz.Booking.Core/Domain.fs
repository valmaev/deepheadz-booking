module DeepHeadz.Booking.Core.Domain

open System
open DeepHeadz.Booking.Core.Spatial

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
