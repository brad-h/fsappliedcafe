module QueriesApi

open Queries
open Suave
open Suave.Filters
open Suave.Operators
open JsonFormatter
open CommandHandler

let readModels getReadModels wp (context : HttpContext) =
  async {
    let! models = getReadModels()
    return! wp models context
  }

let queriesApi queries eventStore =
  GET >=>
  choose [
    path "/tables" >=>
      readModels queries.Table.GetTables toTablesJSON
  ]
