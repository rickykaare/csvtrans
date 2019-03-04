module Formats

open System.IO
open System.Text
open System.Text.RegularExpressions
open System.Xml
open Resx.Resources
open Model

let convertph ph f s =
  match ph with
  | None -> s
  | Some (re:Regex) -> 
      let i = ref 0
      re.Replace(s, fun _ -> let r = f !i in incr i; r)
  
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

  let private formatTokens ph tokens = 
    let createLine token =
      let value = 
        token.Value 
        |> convertph ph (fun _ -> "%@")
        |> stringEncode
      sprintf "\"%s\" = \"%s\";"
        (stringEncode token.Key)
        (value)
    tokens
    |> Seq.map createLine
    |> String.concat "\n"

  let format name ph lang tokens = {
      Path = (getPath name lang) 
      Contents = (formatTokens ph tokens)
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
      .Replace("'","\\'")
      .Replace("\"","\\\"") 

  let private formatTokens ph tokens =
    let settings = XmlWriterSettings ()
    settings.Indent <- true
    settings.Encoding <- UTF8Encoding false
    use output = new MemoryStream()
    use xml = XmlTextWriter.Create (output,settings)
    xml.WriteStartDocument ()
    xml.WriteStartElement "resources"
    for token in tokens do
      let value = 
        token.Value
        |> convertph ph (fun _ -> "%s")
        |> stringEncode
      xml.WriteStartElement "string"
      xml.WriteAttributeString ("name",token.Key)
      xml.WriteValue (value)
      xml.WriteEndElement ()
    xml.WriteEndDocument ()
    xml.Close ()
    Encoding.Default.GetString (output.ToArray())
  
  let format name ph lang tokens ={
    Path = (getPath name lang) 
    Contents = (formatTokens ph tokens)
  }

module Resx =
  let private getPath name lang = 
    Option.defaultValue "Resources" name
    |> match lang with
        | Column.Default -> sprintf "%s.resx"
        | s -> (fun f -> sprintf "%s.%s.resx" f s)

  let private formatTokens ph tokens =
    let sb = StringBuilder ()
    use writer = new ResXResourceWriter (new StringWriter (sb))
    let createNode t = 
      let value = t.Value |> convertph ph (sprintf "{%i}")
      let node = ResXDataNode (t.Key,value,null)
      match (t.Comment) with
      | Some c -> node.Comment <- c; node
      | None -> node
    tokens 
    |> Seq.map createNode
    |> Seq.iter writer.AddResource
    writer.Close ()
    sb.ToString ()

  let format name ph lang tokens = {
    Path = (getPath name lang)
    Contents = (formatTokens ph tokens)
  }

let getFormat = function
  | { Format = Apple; Name = n; Placeholders = ph } -> Apple.format n ph
  | { Format = Android; Name = n; Placeholders = ph } -> Android.format n ph
  | { Format = Resx; Name = n; Placeholders = ph } -> Resx.format n ph