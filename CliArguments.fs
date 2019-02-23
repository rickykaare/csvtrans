module CliArguments

open Argu
open Model

[<Unique>] 
type CliArguments =
  | Sheet of document_id:string*sheet_name:string
  | Url of url:string
  | File of path:string
  | Format of OutputFormat
  | Output of folder_path:string
with 
  interface IArgParserTemplate with
    member s.Usage =
      match s with
      | Sheet _ -> "use the specified Google Sheet as input."
      | Url _ -> "use an online cvs file as input."
      | File _ -> "use a local csv file as input."
      | Format _ -> "specify the output translation format."
      | Output _ -> "specify the output folder" 

let Parse progName args = 
  let parser = 
      ArgumentParser.Create<CliArguments>
        (progName, errorHandler = ProcessExiter())
  let rec loop (o:Options) = function
    | Sheet (d,s)::t -> t |> loop {o with Input = Input.Sheet (d,s) }
    | Url u::t -> t |> loop {o with Input = Input.Url u }
    | File p::t -> t |> loop {o with Input = Input.File p }
    | Format f::t -> t |> loop {o with Format = f }
    | Output p::t -> t |> loop {o with BaseDir = p }
    | [] -> o
  let result = parser.Parse args
  result.GetAllResults() 
  |> loop Options.empty