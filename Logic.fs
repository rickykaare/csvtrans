module Logic

open FSharp.Data
open FSharp.Data.Runtime
open Model

let private getTokens (keyColumn:int) (valueColumn:int) =
    let token (r:CsvRow) = { Key = r.[keyColumn]; Value = r.[valueColumn] }
    Seq.map token 
    >> Seq.where (fun t-> t.Key <> "")
    >> Seq.where (fun t-> t.Value <> "")

let private logTokens log h (tokens:seq<'a>) = 
    log <| sprintf "Processing %i tokens for language '%s'" (Seq.length tokens) h
    tokens

let processRows logger format writer headers rows =  
  let rec loop rows = function
    | [] -> ()
    | (i,h)::t -> 
        rows 
        |> getTokens 0 i 
        |> logTokens logger h 
        |> format h 
        |> writer
        loop rows t
  headers
  |> Seq.indexed 
  |> Seq.filter (snd >> ((<>)Column.Key))
  |> Seq.filter (snd >> ((<>)Column.Comment))
  |> Seq.filter (snd >> ((<>)""))
  |> Seq.toList
  |> loop rows

let processCsv logger format writer (csv:CsvFile<CsvRow>) =
  match csv.Headers with
  | None -> failwith "Specified input has no headers."
  | Some headers -> 
      processRows
        logger
        format
        writer
        headers
        csv.Rows