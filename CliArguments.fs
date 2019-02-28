module CliArguments

open Argu
open Model

[<Unique>] 
type CliArguments =
  | [<AltCommandLine("-s")>]Sheet of document_id:string*sheet_name:string
  | [<AltCommandLine("-c")>]Csv of url_or_path:string
  | [<Mandatory;AltCommandLine("-f")>]Format of OutputFormat
  | [<Mandatory;AltCommandLine("-o")>]OutputDir of directory_path:string
  | [<AltCommandLine("-n")>]Name of string
  | [<Hidden>]Output of folder_path:string
with 
  interface IArgParserTemplate with
    member s.Usage =
      match s with
      | Sheet _ -> "specify a Google Sheet as input."
      | Csv _ -> "specify a online or local cvs file as input."
      | Format _ -> "specify the output format."
      | OutputDir _ -> "specify the output directory." 
      | Name _ -> "specify an optional name for the output."
      | Output _ -> ""

let getSheetUrl (d,s) = sprintf "https://docs.google.com/spreadsheets/d/%s/gviz/tq?tqx=out:csv&sheet=%s&headers=0" d s

let validate = function
  | { InputUrl = "" } -> "ERROR: missing parameter '--sheet' or '--csv'." |> Error
  | o -> o |> Ok

let Parse progName args = 
  let exiter = ProcessExiter () :> IExiter
  let parser = ArgumentParser.Create<CliArguments>
                (progName, errorHandler = exiter)
  let rec loop o = function
    | Sheet (d,s)::t -> (d,s) |> getSheetUrl |> Csv |> (fun h -> h::t) |> loop o
    | Csv u::t -> t |> loop {o with InputUrl = u }
    | Format f::t -> t |> loop {o with Format = f }
    | Output p::t -> t |> loop {o with OutputDir = p }
    | OutputDir p::t -> t |> loop {o with OutputDir = p }
    | Name f::t -> t |> loop {o with Name = Some f }
    | [] -> o

  parser.Parse args
  |> (fun a -> a.GetAllResults())
  |> loop Options.empty
  |> validate
  |> function 
      | Ok o -> o
      | Error e ->
        exiter.Exit
          (String.concat "\n" [e;parser.PrintUsage()], 
          ErrorCode.CommandLine)