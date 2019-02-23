module Logic

open System
open System.Text
open System.Web
open FSharp.Data
open Model
open Resx.Resources
open System.IO

module Format =
    let private stringEncode (s:string) = s.Replace("\"", "\\\"")
    let private xmlEncode = HttpUtility.HtmlEncode
    let private indent i = sprintf "%s%s" (String.replicate i " ")

    let Resx tokens =
        let sb = StringBuilder ()
        let writer = new ResXResourceWriter (new StringWriter (sb))
        let createNode t = new ResXDataNode (t.Key,t.Value,null) 
        tokens 
        |> Seq.map createNode
        |> Seq.iter writer.AddResource
        writer.Close ()
        sb.ToString ()

    let iOS tokens = 
        let format token = 
            sprintf "\"%s\" = \"%s\"" 
                (stringEncode token.Key)
                (stringEncode token.Value)
        Seq.map format tokens |> String.concat "\n"

    let Android tokens =
        let header = [ "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
                       "<resources>" ]
        let footer = [ "</resources>" ]
        let format token = 
            sprintf "<string name=\"%s\">%s</string>" 
                (stringEncode token.Key)
                (xmlEncode token.Value)
            |> indent 4
        tokens
        |> Seq.map format
        |> Seq.append header
        |> Seq.append <| footer
        |> String.concat "\n"

let rec getFormat = function
    | [] -> Format.Resx
    | (Format Ios)::_ -> Format.iOS
    | (Format Android)::_ -> Format.Android
    | (Format Resx)::_ -> Format.Resx
    | _::t -> getFormat t

module Writer =
    let Resx lang = 
        printfn "Translating '%s' for Resx:\n%s\n" lang
    let iOS lang = 
        printfn "Translating '%s' for iOS:\n%s\n" lang
    let Android lang = 
        printfn "Translating '%s' for Android:\n%s\n" lang

let rec getWriter = function
    | [] -> Writer.Resx
    | (Format Ios)::_ -> Writer.iOS
    | (Format Android)::_ -> Writer.Android
    | (Format Resx)::_ -> Writer.Resx
    | _::t -> getWriter t

let getTokens (keyColumn:int) (valueColumn:int) =
    let token (r:CsvRow) = { Key = r.[keyColumn]; Value = r.[valueColumn] }
    Seq.map token

let parse format output rows =
    let rec loop rows = function
        | [] -> ()
        | (i,h)::t -> 
            rows |> getTokens 0 i |> format |> output h
            loop rows t
    List.ofArray >> List.skip 1 >> List.indexed >> loop rows