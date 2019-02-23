module Logic

open System
open System.IO
open System.Text
open System.Web
open System.Xml
open FSharp.Data
open Resx.Resources
open Model

let getTokens (keyColumn:int) (valueColumn:int) =
  let token (r:CsvRow) = { Key = r.[keyColumn]; Value = r.[valueColumn] }
  Seq.map token

let parse format output rows =
  let rec loop rows = function
    | [] -> ()
    | (i,h)::t -> 
        rows |> getTokens 0 i |> format |> output h
        loop rows t
  Seq.indexed 
  >> Seq.filter (snd >> ((<>)"Key"))
  >> Seq.filter (snd >> ((<>)"Comment"))
  >> Seq.toList
  >> loop rows

module Format =
  let private stringEncode (s:string) = s.Replace("\"", "\\\"")
  let private xmlEncode = HttpUtility.HtmlEncode
  let private indent i = sprintf "%s%s" (String.replicate i " ")

  let Resx tokens =
    let sb = StringBuilder ()
    use writer = new ResXResourceWriter (new StringWriter (sb))
    let createNode t = new ResXDataNode (t.Key,t.Value,null) 
    tokens 
    |> Seq.map createNode
    |> Seq.iter writer.AddResource
    writer.Close ()
    sb.ToString ()

  let Ios tokens = 
    let format token = 
      sprintf "\"%s\" = \"%s\"" 
        (stringEncode token.Key)
        (stringEncode token.Value)
    Seq.map format tokens |> String.concat "\n"

  let Android tokens =
    let sb = StringBuilder ()
    use writer = new XmlTextWriter (new StringWriter (sb))
    writer.Formatting <- Formatting.Indented
    writer.WriteStartDocument ()
    writer.WriteStartElement "resources"
    for token in tokens do
      writer.WriteStartElement "string"
      writer.WriteAttributeString ("name",token.Key)
      writer.WriteValue token.Value
      writer.WriteEndElement ()
    writer.WriteEndDocument ()
    writer.Close ()
    sb.ToString ()

let getFormat = function
  | { Format = Ios } -> Format.Ios
  | { Format = Android } -> Format.Android
  | { Format = Resx } -> Format.Resx
    
module FileFormat =
  let Resx writer lang = 
    let path = 
      match lang with
      | "Default" -> sprintf "Resources.resx"
      | s -> sprintf "Resources.%s.resx" s
    writer path
    
  let Ios writer lang = 
    let path = 
      match lang with
      | "Default" -> sprintf "Base.lproj/Localizable.strings"
      | s -> sprintf "%s.lproj/Localizable.strings" s
    writer path
  
  let Android writer lang =
    let path = 
      match lang with
        | "Default" -> sprintf "values/strings.xml"
        | s -> sprintf "values-%s/strings.xml" s
    writer path

let getFileFormat = function
  | { Format = Ios } -> FileFormat.Ios
  | { Format = Android } -> FileFormat.Android
  | { Format = Resx } -> FileFormat.Resx