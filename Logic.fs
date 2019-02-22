module CsvTrans.Logic

open System.Web
open FSharp.Data
open Model

module Format =
    let iOS lang tokens = 
        let formatToken t = t.Value.Replace("\"", "\\\"") |> sprintf "\"%s\" = \"%s\"" t.Key
        tokens 
        |> Seq.map formatToken 
        |> String.concat "\n" 
        |> sprintf "Translating %s:\n%s\n" lang

    let android lang tokens =
        let formatToken t = HttpUtility.HtmlEncode t.Value |> sprintf "  <string name=\"%s\">%s</string>" t.Key
        tokens 
        |> Seq.map formatToken 
        |> String.concat "\n" 
        |> sprintf "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<resources>\n%s\n</resources>\n"
        |> sprintf "Translating %s:\n%s" lang

let rec getFormat = 
      function
    | [] -> Format.iOS
    | h::t -> 
        match h with 
        | Format Ios -> Format.iOS
        | Format Android -> Format.android
        | _ -> getFormat t

let getLang (index:int) (rows:seq<CsvRow>) =
    rows |> Seq.map (fun row -> { Key = row.[0]; Value = row.[index] })

let parse format rows =
    let rec loop rows = 
      function
    | [] -> ()
    | (i,h)::t -> 
        rows |> getLang i |> format h |> printfn "%s"
        loop rows t
    List.ofArray >> List.skip 1 >> List.indexed >> loop rows

