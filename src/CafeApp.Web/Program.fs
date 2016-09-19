module Program

open Suave
open Suave.Web
open Suave.Successful
open Suave.RequestErrors
open Suave.Operators
open Suave.Filters
open Suave.WebSocket
open Suave.Sockets.Control.SocketMonad
open CommandApi
open InMemory
open System.Text
open Chessie.ErrorHandling
open Events
open Projections
open JsonFormatter
open QueriesApi
open System.Reflection
open System.IO

let eventsStream = new Control.Event<Event list>()

let project event =
  projectReadModel inMemoryActions event
  |> Async.RunSynchronously |> ignore

let projectEvents = List.iter project

let commandApiHandler eventStore (context : HttpContext) = async {
  let payload =
    Encoding.UTF8.GetString context.request.rawForm
  let! response =
    handleCommandRequest
      inMemoryQueries eventStore payload
  match response with
  | Ok ((state, events), _) ->
    eventsStream.Trigger(events)
    do! eventStore.SaveEvents state events
    return! toStateJson state context
  | Bad err ->
    return! toErrorJson err.Head context
}

let commandApi eventStore =
  path "/command"
    >=> POST
    >=> commandApiHandler eventStore

let socketHandler (ws : WebSocket) cx = socket {
  while true do
    let! events =
      Control.Async.AwaitEvent(eventsStream.Publish)
      |> Suave.Sockets.SocketOp.ofAsync
    for event in events do
      let eventData =
        event |> eventJObj |> string |> Encoding.UTF8.GetBytes
      do! ws.send Text eventData true
}

let clientDir =
  let exePath = Assembly.GetEntryAssembly().Location
  let exeDir = (new FileInfo(exePath)).Directory
  Path.Combine(exeDir.FullName, "public")

[<EntryPoint>]
let main argv =
  let app =
    let eventStore = inMemoryEventStore ()
    choose [
      commandApi eventStore
      queriesApi inMemoryQueries eventStore
      path "/websocket" >=>
        handShake socketHandler
      GET >=> choose [
        path "/" >=> Files.browseFileHome "index.html"
        Files.browseHome
      ]
    ]
  eventsStream.Publish.Add(projectEvents)
  let cfg =
    {defaultConfig with
      homeFolder = Some(clientDir)
      bindings = [Http.HttpBinding.mkSimple HTTP "0.0.0.0" 8083]}
  startWebServer cfg app
  0 // return an integer exit code
