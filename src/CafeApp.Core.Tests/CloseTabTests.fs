module CloseTabTests

open Domain
open States
open Commands
open Events
open CafeAppTestsDSL
open NUnit.Framework
open TestData
open Errors

let ``Can close the tab by paying full amount`` () =
  let order = {order with Foods = [salad;pizza]; Drinks = [coke]}
  let payment = {Tab = tab; Amount = 10.5m}

  Given (ServedOrder order)
  |> When (CloseTab payment)
  |> ThenStateShouldBe (ClosedTab (Some tab.Id))
  |> WithEvents [TabClosed payment]