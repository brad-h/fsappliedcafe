module OpenTabTests

open NUnit.Framework
open CafeAppTestsDSL
open Domain
open Events
open Commands
open States
open Errors
open System

[<Test>]
let ``Can Open a new Tab``() =
  let tab = {Id = Guid.NewGuid(); TableNumber = 1}

  Given (ClosedTab None) // Current State
  |> When (OpenTab tab) // Command
  |> ThenStateShouldBe (OpenedTab tab) // New State
  |> WithEvents [TabOpened tab] // Event Emitted

[<Test>]
let ``Cannot open an already Opened tab`` () =
  let tab = {Id = Guid.NewGuid(); TableNumber = 1}

  Given (OpenedTab tab)
  |> When (OpenTab tab)
  |> ShouldFailWith TabAlreadyOpened