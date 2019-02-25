module Logic

open FSharp.Data
open Model

let private getTokens (keyColumn:int) (valueColumn:int) =
    let token (r:CsvRow) = { Key = r.[keyColumn]; Value = r.[valueColumn] }
    Seq.map token

let private logTokens log h (tokens:seq<'a>) = 
    log <| sprintf "Processing %i tokens for language '%s'" (Seq.length tokens) h
    tokens

let processRows logger formatter headers rows =  
  let rec loop rows = function
    | [] -> ()
    | (i,h)::t -> 
        rows |> getTokens 0 i |> logTokens logger h |> formatter h
        loop rows t
  headers
  |> Seq.indexed 
  |> Seq.filter (snd >> ((<>)"Key"))
  |> Seq.filter (snd >> ((<>)"Comment"))
  |> Seq.filter (snd >> ((<>)""))
  |> Seq.toList
  |> loop rows