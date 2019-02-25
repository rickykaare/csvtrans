module Program

open IO
open Logic

[<EntryPoint>]
let main argv =
  let options = CliArguments.Parse "csvtrans" argv
  let logger = printfn "%s"
  let format = Formats.GetFormat options 
  let writer = createFile logger options      
  let csv = getCsv logger options
  
  match csv.Headers with
  | None -> eprintfn "ERROR: specified input has no headers."; exit 4
  | Some headers -> 
    try
      processRows 
        logger 
        (format writer)
        headers
        csv.Rows
    with e -> 
      eprintfn "ERROR: %s" e.Message; exit 4
  0