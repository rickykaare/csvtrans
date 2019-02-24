module IO

open System.IO
open FSharp.Data
open Model

let getUrl = sprintf "https://docs.google.com/spreadsheets/d/%s/gviz/tq?tqx=out:csv&sheet=%s&headers=0"
let cache (csv:CsvFile) = csv.Cache()
let rec getCsv log options = 
  match options.Input with
  | None -> failwith "No input specified."
  | Sheet (d,s) -> 
      let url = getUrl d s
      log <| sprintf "Downloading %s..." url
      url |> CsvFile.Load |> cache
  | Url u -> 
      log <| sprintf "Downloading %s..." u
      u |> CsvFile.Load |> cache
  | File p -> 
      log <| sprintf "Reading %s..." p
      p |> CsvFile.Load |> cache

let createFile log baseDir path contents =
  let fullPath = Path.Combine(baseDir,path)
  let dir = Path.GetDirectoryName fullPath
  if not <| Directory.Exists dir then do 
    log <| sprintf "Creating folder \"%s\"..." dir
    Directory.CreateDirectory dir |> ignore
  log <| sprintf "Writing file \"%s\"..." path
  File.WriteAllText (fullPath, contents)
