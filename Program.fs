module CsvTrans.Program

open Model
open Data
open Logic

[<EntryPoint>]
let main argv =
    let options = CliArguments.Parse "csvtrans" argv
    let format = getFormat options
    getCsv options
    |> function
     | Ok data -> 
         match data.Headers with
         | None -> eprintfn "Sheet has no headers"
         | Some h -> h |> parse format data.Rows
     | Error e -> eprintfn "%s" e
    0
