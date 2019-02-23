module Program

open IO
open Logic

[<EntryPoint>]
let main argv =
  let logger = printfn "%s"
  let options = CliArguments.Parse "csvtrans" argv
  let format = getFormat options
  let fileFormat = getFileFormat options
  let writer = createFile logger options.BaseDir
  let output = fileFormat writer
  let csv = getCsv logger options
  match csv.Headers with
  | Some h -> parse format output csv.Rows h
  | None -> eprintfn "ERROR: Specified input has no headers"
  0   