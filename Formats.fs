module Formats

open System.IO
open System.Text
open System.Xml
open Resx.Resources
open Model

module Apple =
  let private getPath = function
    | Column.Default -> sprintf "Base.lproj/Localizable.strings"
    | s -> sprintf "%s.lproj/Localizable.strings" s
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
  let format writer lang tokens =
    writer
      (getPath lang) 
      (formatTokens tokens)

module Android =
  let private getPath = function
    | Column.Default -> sprintf "values/strings.xml"
    | s -> sprintf "values-%s/strings.xml" s
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
  let format writer lang tokens =
    writer
      (getPath lang) 
      (formatTokens tokens)

module Resx =
  let private getPath = function
    | Column.Default -> sprintf "Resources.resx"
    | s -> sprintf "Resources.%s.resx" s
  let private formatTokens tokens =
    let sb = StringBuilder ()
    use writer = new ResXResourceWriter (new StringWriter (sb))
    let createNode t = new ResXDataNode (t.Key,t.Value,null)
    tokens 
    |> Seq.map createNode
    |> Seq.iter writer.AddResource
    writer.Close ()
    sb.ToString ()
  let format writer lang tokens =
    writer
        (getPath lang)
        (formatTokens tokens)

let getFormat = function
  | { Format = Apple } -> Apple.format
  | { Format = Android } -> Android.format
  | { Format = Resx } -> Resx.format