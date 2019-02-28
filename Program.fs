module Program

open IO
open Logic
open Formats

[<EntryPoint>]
let main argv =
  let options = CliArguments.Parse "csvtrans" argv
  let logger = printfn "%s"
  let format = getFormat options 
  let writer = createFile logger options
  
  try
    getCsv logger options
    |> processCsv 
          logger 
          format
          writer
  with e -> 
    eprintfn "ERROR: %s" e.Message; exit 4
  0