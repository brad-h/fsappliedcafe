module CommandHandlers

open States
open Events
open System
open Commands

let execute state command =
  match command with
  | OpenTab tab -> [TabOpened tab]
  | _ -> failwith "Todo"

let evolve state command =
  let event = execute state command
  let newState = List.fold apply state event
  (newState, event)