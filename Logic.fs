module Logic

open FSharp.Data
open FSharp.Data.Runtime
open Model

let private getPhrases (keyColumn:int) (commentColumn:int option) (valueColumn:int) =
    let getComment (r:CsvRow) = 
      match commentColumn with
      | Some c -> r.[c] |> Some
      | None -> None
    let phrase (r:CsvRow) = { 
      Key = r.[keyColumn]
      Value = r.[valueColumn]
      Comment = (getComment r)
    }
    Seq.map phrase 
    >> Seq.where (fun t-> t.Key <> "")
    >> Seq.where (fun t-> t.Value <> "")

let processRows format writer headers rows =  
  let keyColumn = headers |> Seq.findIndex ((=)Column.Key)
  let commentColumn = headers |> Seq.tryFindIndex ((=)Column.Comment)
  let rec loop rows = function
    | [] -> ()
    | (i,h)::t -> 
        rows 
        |> getPhrases keyColumn commentColumn i 
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

let processCsv format writer (csv:CsvFile<CsvRow>) =
  match csv.Headers with
  | None -> failwith "Specified input has no headers."
  | Some headers -> 
      processRows
        format
        writer
        headers
        csv.Rows