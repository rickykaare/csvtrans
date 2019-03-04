module Logic

open FSharp.Data
open FSharp.Data.Runtime
open Model

let private getTokens (keyColumn:int) (commentColumn:int option) (valueColumn:int) =
    let getComment (r:CsvRow) = 
      match commentColumn with
      | Some c -> r.[c] |> Some
      | None -> None
    let token (r:CsvRow) = { 
      Key = r.[keyColumn]
      Value = r.[valueColumn]
      Comment = (getComment r)
    }
    Seq.map token 
    >> Seq.where (fun t-> t.Key <> "")
    >> Seq.where (fun t-> t.Value <> "")

let private logTokens log h (tokens:seq<'a>) = 
    log <| sprintf "Processing %i tokens for language '%s'" (Seq.length tokens) h
    tokens

let processRows logger format writer headers rows =  
  let keyColumn = headers |> Seq.findIndex ((=)Column.Key)
  let commentColumn = headers |> Seq.tryFindIndex ((=)Column.Comment)
  let rec loop rows = function
    | [] -> ()
    | (i,h)::t -> 
        rows 
        |> getTokens keyColumn commentColumn i 
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