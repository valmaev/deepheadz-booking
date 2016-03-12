module DeepHeadz.Booking.TestConsole.Host

open System
open DeepHeadz.Booking.Core
open DeepHeadz.Booking.Core.Serialization
open DeepHeadz.Booking.Data.FileStore
open DeepHeadz.Booking.Data.Json

[<EntryPoint>]
let main argv = 
    let serializer = createSerializerWithDefaultSettings() :> ISerializer

    let room = { 
        Id = 1
        Title = "Room"
        Description = "Room description" 
        Type = "apartment"
        NumberOfRooms = 2
        MaxGuests = 1

        CountryCode = "RU"
        City = "Moscow"
        Latitude = 11.34
        Longitude = 32.55
        PricePerNight = 44.55
        CurrencyCode = "US"
        Amenities = []
        CancellationPolicy = None
        HostId = 12
        HostName = "host"
        HostEmail = "qwe@qwe.com"
        HostUrl = "erwe"

        CreatedAt = DateTimeOffset.Now
        UpdatedAt = DateTimeOffset.Now} 


    let serializedRoom = serializer.SerializeAsUtf8String room
    printfn "%s" serializedRoom
    let fileStore = RoomFileStore(serializer, "Rooms.json")

    printfn "%A" fileStore.Rooms

    0
