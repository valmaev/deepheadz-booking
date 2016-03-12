namespace DeepHeadz.Booking.Core

open System

[<CLIMutable>]
type Room = {
    Id: int
    Title: string
    Description: string
    Type: string
    NumberOfRooms: int
    MaxGuests: int

    CountryCode: string
    City: string
    Latitude: float
    Longitude: float

    PricePerNight: float
    CurrencyCode: string

    Amenities: string seq

    CancellationPolicy: string option

    HostId: int
    HostName: string
    HostEmail: string
    HostUrl: string

    CreatedAt: DateTimeOffset
    UpdatedAt: DateTimeOffset }
 