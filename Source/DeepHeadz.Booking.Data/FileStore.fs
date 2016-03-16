module DeepHeadz.Booking.Data.FileStore

open System.Collections
open System.IO
open DeepHeadz.Booking.Core

type InMemoryStore<'a>(seed: 'a seq) =
    interface 'a seq with
        member x.GetEnumerator() = seed.GetEnumerator()
        member x.GetEnumerator() = (x :> 'a seq).GetEnumerator() :> IEnumerator

type FileStore<'a>(serializer: ISerializer, fileName: string) =
    let readData(): 'a seq = 
        use stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Read)
        serializer.Deserialize stream
    
    let writeData (data: 'a seq) =
        use stream = new FileStream(fileName, FileMode.Create, FileAccess.Write)
        serializer.Serialize data stream

    member val Data = readData() with get 

    interface 'a seq with
        member x.GetEnumerator() = x.Data.GetEnumerator()
        member x.GetEnumerator() = (x :> 'a seq).GetEnumerator() :> IEnumerator

    interface IWriter<'a seq> with
        member x.Write data = writeData data
