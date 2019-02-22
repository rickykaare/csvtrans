module CsvTrans.Data

open FSharp.Data
open Model

let getUrl = sprintf "https://docs.google.com/spreadsheets/d/%s/gviz/tq?tqx=out:csv&sheet=%s&headers=0"
let cache (csv:CsvFile) = csv.Cache()
let rec getCsv = 
      function
    | [] -> Error "Please specify an input"
    | h::t -> 
        match h with
        | Sheet (d,s) -> getUrl d s |> CsvFile.Load |> cache |> Ok
        | Url u -> u |> CsvFile.Load |> cache |> Ok
        | File p -> p |> CsvFile.Load |> cache |> Ok
        | _ -> getCsv t