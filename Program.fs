module Program

open IO
open Logic

[<EntryPoint>]
let main argv =
  let options = CliArguments.Parse "csvtrans" argv
  let logger = printfn "%s"
  let format = getFormat options
  let output =  createFile logger options 
                |> getFileFormat options               
  let csv = getCsv logger options
  match csv.Headers with
  | None -> eprintfn "ERROR: specified input has no headers."; exit 4
  | Some h -> 
    try
      parse logger format output csv.Rows h
    with e -> 
      eprintfn "ERROR: %s" e.Message; exit 4
  0