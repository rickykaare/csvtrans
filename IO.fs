module IO

open System.IO
open FSharp.Data
open Model

let getCsv log options =
  let cache (c:CsvFile) = c.Cache () 
  log <| sprintf "Loading %s" options.InputUrl
  options.InputUrl
  |> CsvFile.Load
  |> cache

let createFile log options path contents =
  let fullPath = Path.Combine (options.OutputDir,path)
  let dir = Path.GetDirectoryName fullPath
  if not <| Directory.Exists dir then do 
    log <| sprintf "Creating folder '%s'" dir
    Directory.CreateDirectory dir |> ignore
  log <| sprintf "Writing file '%s'" path
  File.WriteAllText (fullPath, contents)