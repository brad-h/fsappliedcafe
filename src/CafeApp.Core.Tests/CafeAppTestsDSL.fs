module CafeAppTestsDSL

open FsUnit
open NUnit.Framework
open CommandHandlers
open States
open Events

let Given (state : State) = state

let When command state = command, state

let ThenStateShouldBe expectedState (command, state) =
  let actualState, actualEvent = evolve state command
  actualState |> should equal expectedState
  actualEvent

let WithEvents (expectedEvents: Event list) (actualEvents: Event list) =
  actualEvents |> should equal expectedEvents