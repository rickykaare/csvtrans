module Program

open Model
open Data
open Logic

[<EntryPoint>]
let main argv =
    let options = CliArguments.Parse "csvtrans" argv
    let format = getFormat options
    let output = getWriter options
    let t = getCsv options
            |> Result.map id

    getCsv options
    |> function
     | Ok data -> 
         match data.Headers with
         | Some h -> h |> parse format output data.Rows
         | None -> eprintfn "ERROR: Specified input has no headers"
     | Error e -> eprintfn "ERROR: %s" e
    0
