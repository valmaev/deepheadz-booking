FROM fsharp/fsharp
COPY . .
RUN mono ./.paket/paket.bootstrapper.exe
RUN mono ./.paket/paket.exe restore
RUN mono ./packages/FAKE/tools/FAKE.exe build.fsx
ENTRYPOINT ["mono", "./Artifacts/DeepHeadz.Booking.Http.OwinHost.exe", "http://*:8080"]
