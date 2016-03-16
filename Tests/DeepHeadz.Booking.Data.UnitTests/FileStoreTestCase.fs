namespace DeepHeadz.Booking.Data.UnitTests

open System.Linq
open Xunit
open DeepHeadz.Booking.Core
open DeepHeadz.Booking.Core.Domain
open DeepHeadz.Booking.Data.FileStore
open DeepHeadz.Booking.Data.Json

type FileStoreTestCase() = 
    let serializer = createSerializerWithDefaultSettings() :> ISerializer
    let createSut fileName = FileStore<Room>(serializer, fileName)

    let [<Fact>] ``Data should always return all deserialized data from file``() =
        let sut = createSut "Rooms.json"
        Assert.Equal(2, sut.Data.Count())
