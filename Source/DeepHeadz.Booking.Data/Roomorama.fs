module DeepHeadz.Booking.Data.Roomorama

open System
open System.Collections.Generic
open System.Globalization
open RestSharp
open RestSharp.Authenticators
open DeepHeadz.Booking.Core
open DeepHeadz.Booking.Core.Serialization

[<CLIMutable>]
type DestinationResult = {
    id: int
    url_name: string
    url_path: string
    ``type``: string
    name: string
    long_name: string
    url: string
    currency_code: string
    currency_display: string
    searchable: bool }

[<CLIMutable>]
type DestinationsResponse = {
    result: DestinationResult array
    count: int
    status: int }

[<CLIMutable>]
type HostResult = {
    id: int
    display: string
    certified: bool
    url: string }

[<CLIMutable>]
type RoomResult = {
    id: int;
    title: string
    ``type``: string
    subtype: string
    url: string
    num_rooms: int
    max_guests: int
    min_stay: int
    price: int
    currency_code: string
    thumbnail: string
    city: string
    country_code: string
    lat: string
    lng: string
    multi_unit: bool
    instantly_bookable: bool
    can_contact_host: bool
    created_at: DateTimeOffset
    updated_at: DateTimeOffset
    calendar_updated_at: DateTimeOffset
    host: HostResult }

[<CLIMutable>]
type Pagination = { 
    prevoius: int
    next: int
    current: int
    count: int
    pages: int
    per_page: int }

[<CLIMutable>]
type RoomsResponse = {
    result: RoomResult array
    min_price: int
    max_price: int
    pagination: Pagination
    status: string }

[<CLIMutable>]
type Image = { image: string; position: int }

[<CLIMutable>]
type ServiceInfo = { available: bool; rate: string }

[<CLIMutable>]
type RoomDetailedResult = {
    images: Image array
    id: int
    title: string
    ``type``: string
    subtype: string
    url: string
    num_rooms: int
    num_bathrooms: float
    max_guests: int
    min_stay: int
    floor: string
    num_double_beds: int
    num_sofa_beds: int
    surface: float
    price: int
    currency_code: string
    currency_display: string
    description: string
    city: string
    province: string
    country_code: string
    lat: string
    lng: string
    amenities: string
    conditions: Dictionary<string, string>
    services: Dictionary<string, ServiceInfo>
    check_in_time: string
    check_out_time: string
    cancellation_policy: string
    multi_unit: bool
    instantly_bookable: bool
    can_contact_host: bool
    created_at: DateTimeOffset
    updated_at: DateTimeOffset
    calendar_updated_at: DateTimeOffset
    host: HostResult }

[<CLIMutable>]
type RoomResponse = { result: RoomDetailedResult }

[<CLIMutable>]
type RoomAvailabilityRequest = {
    room_id: int
    start_date: DateTimeOffset option
    end_date: DateTimeOffset option }

[<CLIMutable>]
type RoomAvailabilityResult = {
    ``available?``: bool
    currency_code: string
    currency_display: string
    nightly_rate: int
    weekly_rate: int
    monthly_rate: int
    ``can_checkin?``: bool
    ``can_checkout?``: bool
    minimum_stay: int
    date: DateTimeOffset }
 
[<CLIMutable>]
type RoomAvailabilityResponse = { Result: RoomAvailabilityResult array }

let serializer = (Json.createSerializerWithDefaultSettings() :> ISerializer)
let deserialize = serializer.DeserializeFromUtf8String

let toIsoDate (offset: DateTimeOffset option) = 
    match offset with
    | Some x -> x.ToString "yyyy-MM-dd"
    | None -> ""

let roomoramaClient = 
    let client = RestClient(baseUrl = "https://api.roomorama.com/v1.0")
    client.Authenticator <- 
        OAuth2UriQueryParameterAuthenticator(
            accessToken = "EnG4TbobhqysUi6FDovARXn7ouisvwZ2mmX5bqstI")
    client

let getDestinations(): DestinationsResponse =
    let request = RestRequest("/destinations", Method.GET)
    let response = roomoramaClient.Execute request
    deserialize response.Content

let getRoomsByDestinationId (destinationId: int) (page: int): RoomsResponse =
    let request = 
        RestRequest("/rooms", Method.GET)
            .AddParameter("destination_id", destinationId)
            .AddParameter("limit", 100)
            .AddParameter("page", page)
    let response = roomoramaClient.Execute request
    deserialize response.Content

let getRoomsByDestinationName (destination: string) (page: int): RoomsResponse =
    let request = 
        RestRequest("/rooms", Method.GET)
            .AddParameter("destination", destination)
            .AddParameter("limit", 100)
            .AddParameter("page", page)
    let response = roomoramaClient.Execute request
    deserialize response.Content

let getRoom (roomId: int): RoomResponse =
    let request = 
        RestRequest("/rooms/{room_id}", Method.GET)
            .AddUrlSegment("room_id", roomId.ToString(CultureInfo.InvariantCulture))
            .AddParameter("currency", "USD")
    let response = roomoramaClient.Execute request
    deserialize response.Content

let getRoomAvailability (request): RoomAvailabilityResponse =
    let restRequest = 
        RestRequest("/rooms/{room_id}/availabilities", Method.GET)
            .AddUrlSegment("room_id", request.room_id.ToString(CultureInfo.InvariantCulture))
            .AddParameter("start_date", request.start_date |> toIsoDate)
            .AddParameter("end_date", request.end_date |> toIsoDate)
            .AddParameter("currency", "USD")
    let response = roomoramaClient.Execute restRequest
    deserialize response.Content

let private isConditionAllowed (conditions: Dictionary<string, string>) name =
    match conditions.TryGetValue name with
    | (true, x) -> bool.Parse x
    | _ -> false

let private toInt s =
    match Int32.TryParse s with
    | (true, x) -> x
    | _ -> 0

let toRoom (roomResult: RoomDetailedResult): Room = {
        Id = roomResult.id
        Title = roomResult.title
        Description = roomResult.description
        Type = roomResult.``type``
        Subtype = roomResult.subtype
        Surface = roomResult.surface
        Floor = roomResult.floor |> toInt
        NumberOfRooms = roomResult.num_rooms
        NumberOfBathrooms = int roomResult.num_bathrooms 
        NumberOfDoubleBeds = roomResult.num_double_beds
        NumberOfSofaBeds = roomResult.num_sofa_beds
        MinStayInDays = roomResult.min_stay
        MaxGuests = roomResult.max_guests

        CountryCode = roomResult.country_code
        City = roomResult.city
        Province = roomResult.province
        Latitude = float roomResult.lat
        Longitude = float roomResult.lng

        PricePerNight = decimal roomResult.price
        CurrencyCode = roomResult.currency_code

        Amenities = 
            roomResult.amenities.Split(',') 
            |> Seq.map (fun s -> s.Trim())
        Services = 
            roomResult.services
            |> Seq.filter (fun s -> s.Value.available)
            |> Seq.map (fun s -> s.Key)
        SmokingAllowed = isConditionAllowed roomResult.conditions "smoking"
        PetsAllowed = isConditionAllowed roomResult.conditions "pets"

        CheckInTime = roomResult.check_in_time
        CheckOutTime = roomResult.check_out_time
        CancellationPolicy =
            match roomResult.cancellation_policy with
            | null -> ""
            | x -> x

        Images = roomResult.images |> Seq.map (fun i -> i.image)
        Url = roomResult.url

        HostId = roomResult.host.id
        HostName = roomResult.host.display
        HostEmail = "valmaev@aquivalabs.com"
        HostUrl = roomResult.host.url

        CreatedAt = roomResult.created_at
        UpdatedAt = roomResult.updated_at
    }
