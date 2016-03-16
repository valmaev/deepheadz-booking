module DeepHeadz.Booking.Core.Spatial

open System

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
 
let haversineDistance (c1: Coordinate) (c2: Coordinate) =
    2.0 * earthRadius * Math.Asin(
        Math.Sqrt(haversine(c2.φ - c1.φ) +
            Math.Cos(c1.φ / 1.0<rad>) * Math.Cos(c2.φ / 1.0<rad>) * haversine(c2.ψ - c1.ψ)))

let circleGeoSearch (center: Coordinate) (radius: float<km>) (coordinates: Coordinate seq) =
   coordinates
   |> Seq.filter (fun c -> (haversineDistance c center) <= radius)
