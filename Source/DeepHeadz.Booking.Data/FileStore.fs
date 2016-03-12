module DeepHeadz.Booking.Data.FileStore

open System.Collections
open System.IO
open DeepHeadz.Booking.Core

let toEnumerator(s: seq<'T>) = s.GetEnumerator()

type RoomInMemoryStore(seed: Room seq) =
    interface Room seq with
        member x.GetEnumerator() = seed.GetEnumerator()
        member x.GetEnumerator() = (x :> Room seq).GetEnumerator() :> IEnumerator

type RoomFileStore(serializer: ISerializer, fileName: string) =
    let readRooms (fileName): Room seq = 
        use stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Read)
        serializer.Deserialize stream

    member val Rooms = readRooms fileName with get 

    interface Room seq with
        member x.GetEnumerator() = x.Rooms.GetEnumerator()
        member x.GetEnumerator() = (x :> Room seq).GetEnumerator() :> IEnumerator
