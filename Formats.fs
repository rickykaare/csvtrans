module Formats

open System.IO
open System.Text
open System.Xml
open Resx.Resources
open Model

module Apple =
  let private getPath name lang = 
    Option.defaultValue "Localizable" name
    |> match lang with
        | Column.Default -> sprintf "Base.lproj/%s.strings"
        | s -> sprintf "%s.lproj/%s.strings" s
  let private stringEncode (str:string) = 
    str
      .Replace("\\", "\\\\")
      .Replace("\n", "\\n")
      .Replace("\t", "\\t")
      .Replace("\"", "\\\"")
  let private formatTokens tokens = 
    let createLine token =
      sprintf "\"%s\" = \"%s\";"
        (stringEncode token.Key)
        (stringEncode token.Value)
    tokens
    |> Seq.map createLine
    |> String.concat "\n"
  let format name lang tokens = {
      Path = (getPath name lang) 
      Contents = (formatTokens tokens)
  }

module Android =
  let private getPath name lang = 
    Option.defaultValue "strings" name
    |> match lang with
        | Column.Default -> sprintf "values/%s.xml"
        | s -> sprintf "values-%s/%s.xml" s
  let private stringEncode (str:string) = 
    str
      .Replace("\\", "\\\\")
      .Replace("\n", "\\n")
      .Replace("@", "\\@")
      .Replace("?","\\?")
      .Replace("&","&amp;")
      .Replace("<","&lt;")
      .Replace(">","&gt;")
      .Replace("'","&apos;")
      .Replace("\"","&quot;") 
  let private formatTokens tokens =
    let sb = StringBuilder ()
    use xml = new XmlTextWriter (new StringWriter (sb))
    xml.Formatting <- Formatting.Indented
    xml.WriteStartDocument ()
    xml.WriteStartElement "resources"
    for token in tokens do
      xml.WriteStartElement "string"
      xml.WriteAttributeString ("name",token.Key)
      xml.WriteValue (stringEncode token.Value)
      xml.WriteEndElement ()
    xml.WriteEndDocument ()
    xml.Close ()
    sb.ToString ()
  let format name lang tokens ={
    Path = (getPath name lang) 
    Contents = (formatTokens tokens)
  }

module Resx =
  let private getPath name lang = 
    Option.defaultValue "Resources" name
    |> match lang with
        | Column.Default -> sprintf "%s.resx"
        | s -> (fun f -> sprintf "%s.%s.resx" f s)
  let private formatTokens tokens =
    let sb = StringBuilder ()
    use writer = new ResXResourceWriter (new StringWriter (sb))
    let createNode t = new ResXDataNode (t.Key,t.Value,null)
    tokens 
    |> Seq.map createNode
    |> Seq.iter writer.AddResource
    writer.Close ()
    sb.ToString ()
  let format name lang tokens = {
    Path = (getPath name lang)
    Contents = (formatTokens tokens)
  }

let getFormat = function
  | { Format = Apple; Name = n } -> Apple.format n
  | { Format = Android; Name = n } -> Android.format n
  | { Format = Resx; Name = n } -> Resx.format n