module DeepHeadz.Booking.Data.Json

open System.IO
open System.Runtime.Serialization
open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open DeepHeadz.Booking.Core

type JsonNetSerializer(settings: JsonSerializerSettings) =
    interface ISerializer with 
        member x.Serialize value input =
            try
                let serializer = JsonSerializer.Create settings
                use streamWriter = new StreamWriter(input)
                use jsonWriter = new JsonTextWriter(streamWriter)
                serializer.Serialize(jsonWriter, value)
            with
            | :? JsonException as ex ->
                raise (new SerializationException("Exception occurred while serializing", ex))

        member x.Deserialize input =
            try
                let serializer = JsonSerializer.Create settings
                use streamReader = new StreamReader(input)
                use jsonReader = new JsonTextReader(streamReader)
                serializer.Deserialize<'a> jsonReader
            with
            | :? JsonException as ex ->
                raise (new SerializationException("Exception occurred while deserializing", ex))

type PreservingDictionaryCasingContractResolver() =
    inherit CamelCasePropertyNamesContractResolver()
    override x.CreateDictionaryContract objectType =
        let contract = base.CreateDictionaryContract objectType
        contract.DictionaryKeyResolver <- (fun n -> n)
        contract

let createSerializerWithDefaultSettings() =
    JsonNetSerializer(
        JsonSerializerSettings(
            DefaultValueHandling = DefaultValueHandling.Ignore,
            ContractResolver = (PreservingDictionaryCasingContractResolver() :> IContractResolver)))
