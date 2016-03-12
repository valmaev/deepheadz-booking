namespace DeepHeadz.Booking.Data.UnitTests

open System.Linq
open Xunit
open DeepHeadz.Booking.Core
open DeepHeadz.Booking.Data.FileStore
open DeepHeadz.Booking.Data.Json

type RoomStoreTestCase() = 

    let serializer = createSerializerWithDefaultSettings() :> ISerializer
    let createSut fileName = RoomFileStore(serializer, fileName)

    let [<Fact>] ``Rooms should always return all deserialized rooms from file``() =
        let sut = createSut "Rooms.json"
        Assert.Equal(2, sut.Rooms.Count())
