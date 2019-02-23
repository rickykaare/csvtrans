module Data

open FSharp.Data
open Model

let getUrl = sprintf "https://docs.google.com/spreadsheets/d/%s/gviz/tq?tqx=out:csv&sheet=%s&headers=0"
let cache (csv:CsvFile) = csv.Cache()
let rec getCsv = function
    | Sheet (d,s)::_ -> getUrl d s |> CsvFile.Load |> cache |> Ok
    | Url u::_ -> u |> CsvFile.Load |> cache |> Ok
    | File p::_ -> p |> CsvFile.Load |> cache |> Ok
    | _::t -> getCsv t
    | [] -> Error "Please specify an input using either '--sheet', '--url' or '--file'"

