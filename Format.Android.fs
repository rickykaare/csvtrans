module Android

open Model
open System.IO
open System.Text
open System.Xml

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

let private formatPhrases ph phrases =
  let settings = XmlWriterSettings() 
  settings.Indent <- true
  settings.Encoding <- UTF8Encoding false
  use output = new MemoryStream()
  use xml = XmlTextWriter.Create (output,settings)
  xml.WriteStartDocument ()
  xml.WriteStartElement "resources"
  for phrase in phrases do
    let value = 
      phrase.Value
      |> ph (fun _ -> "%s")
      |> stringEncode
    xml.WriteStartElement "string"
    xml.WriteAttributeString ("name",phrase.Key)
    xml.WriteValue (value)
    xml.WriteEndElement ()
  xml.WriteEndDocument ()
  xml.Close ()
  Encoding.Default.GetString (output.ToArray())
  
let format name ph lang phrases = {
  Path = (getPath name lang) 
  Contents = (formatPhrases ph phrases)
  Phrases = Seq.length phrases
}