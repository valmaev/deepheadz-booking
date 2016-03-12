namespace DeepHeadz.Booking.Core

open System.IO
open System.Text

type IWriter<'a> =
    abstract Write: 'a -> Unit

type ISerializer =
    abstract Serialize: obj -> Stream -> unit
    abstract Deserialize: Stream -> 'a

module Serialization = 
    type ISerializer with 
        member x.SerializeAsString (value) (encoding: Encoding) =
            use stream = new MemoryStream()
            x.Serialize value stream
            stream.ToArray() |> encoding.GetString
        member x.SerializeAsUtf8String (value) = x.SerializeAsString value Encoding.UTF8
        member x.DeserializeFromString<'a> (value: string) (encoding: Encoding) =
            let bytes = encoding.GetBytes value
            use stream = new MemoryStream(bytes)
            x.Deserialize<'a> stream
        member x.DeserializeFromUtf8String value = x.DeserializeFromString value Encoding.UTF8
