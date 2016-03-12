﻿open System.IO
open System.Collections.Generic
open DeepHeadz.Booking.Core
open DeepHeadz.Booking.Core.Serialization
open DeepHeadz.Booking.Data.FileStore
open DeepHeadz.Booking.Data.Json
open DeepHeadz.Booking.Data.Roomorama

let serializer = createSerializerWithDefaultSettings() :> ISerializer
let roomStore = FileStore<Room>(serializer, "room-store.json")
let roomWriter = roomStore :> IWriter<Room seq>

let roomDetailsStore = FileStore<RoomDetailedResult>(serializer, "moscow-room-details-3.json")
let roomDetailsWriter = roomDetailsStore :> IWriter<RoomDetailedResult seq>

//let roomResultStore = FileStore<RoomResult>(serializer, "rooms.json")
//let roomResultWriter = roomResultStore :> IWriter<RoomResult seq>

let concatFiles() =
    use stream = new FileStream("moscow-rooms.json", FileMode.Open, FileAccess.Read)
    let rooms = serializer.Deserialize<Room array> stream

    use stream = new FileStream("moscow-rooms-2.json", FileMode.Open, FileAccess.Read)
    let rooms2 = serializer.Deserialize<Room array> stream

    use stream = new FileStream("moscow-rooms-3.json", FileMode.Open, FileAccess.Read)
    let rooms3 = serializer.Deserialize<Room array> stream

    let allRooms =
        rooms
        |> Seq.append rooms2
        |> Seq.append rooms3
        |> Seq.toArray
    ()

[<EntryPoint>]
let main argv = 
    //let destinationName = "Moscow"
    //let response = getRoomsByDestinationName destinationName 1
    //let rooms = List<RoomResult> response.result
    //for i = 2 to respnse.pagination.pages do
    //    let response = getRoomsByDestinationName destinationName i
    //    rooms.AddRange response.result

    //roomResultWriter.Write rooms

    use stream = new FileStream("rooms.json", FileMode.Open, FileAccess.Read)
    let rooms = serializer.Deserialize stream

    let details = 
        rooms
        |> Seq.skip 200
        |> Seq.map (fun r -> (getRoom r.id).result)
        |> Seq.toList

    roomDetailsWriter.Write details
    
    roomWriter.Write 
        (details 
        |> Seq.map toRoom
        |> Seq.toList)
    0
