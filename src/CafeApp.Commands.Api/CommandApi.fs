module CommandApi

open System.Text
open CommandHandler
open OpenTab
open Queries
open Chessie.ErrorHandling

let handleCommandRequest queries eventStore = function
| OpenTabRequest tab ->
  queries.Table.GetTableByTableNumber
  |> openTabCommander
  |> handleCommand eventStore tab
| _ -> err "Invalid Command" |> fail |> async.Return