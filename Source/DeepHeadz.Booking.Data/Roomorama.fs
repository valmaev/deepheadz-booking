﻿module DeepHeadz.Booking.Data.Roomorama

open System
open System.Collections.Generic
open System.Globalization
open RestSharp
open RestSharp.Authenticators
open Newtonsoft.Json

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

let getDestinations =
    let request = RestRequest("/destinations", Method.GET)
    let response = roomoramaClient.Execute request
    response

let getRoomsByDestinationId (destinationId: int, page: int) =
    let request = 
        RestRequest("/rooms", Method.GET)
            .AddParameter("destination_id", destinationId)
            .AddParameter("limit", 100)
            .AddParameter("page", page)
    let response = roomoramaClient.Execute request
    JsonConvert.DeserializeObject<RoomsResponse> response.Content

let getRoomsByDestinationName (destination: string, page: int) =
    let request = 
        RestRequest("/rooms", Method.GET)
            .AddParameter("destination", destination)
            .AddParameter("limit", 100)
            .AddParameter("page", page)
    let response = roomoramaClient.Execute request
    JsonConvert.DeserializeObject<RoomsResponse> response.Content

let getRoom (roomId: int) =
    let request = 
        RestRequest("/rooms/{room_id}", Method.GET)
            .AddUrlSegment("room_id", roomId.ToString(CultureInfo.InvariantCulture))
            .AddParameter("currency", "USD")
    let response = roomoramaClient.Execute request
    JsonConvert.DeserializeObject<RoomResponse> response.Content

let getRoomAvailability request =
    let restRequest = 
        RestRequest("/rooms/{room_id}/availabilities", Method.GET)
            .AddUrlSegment("room_id", request.room_id.ToString(CultureInfo.InvariantCulture))
            .AddParameter("start_date", request.start_date |> toIsoDate)
            .AddParameter("end_date", request.end_date |> toIsoDate)
            .AddParameter("currency", "USD")
    let response = roomoramaClient.Execute restRequest
    JsonConvert.DeserializeObject<RoomAvailabilityResponse> response.Content