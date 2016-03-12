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
 