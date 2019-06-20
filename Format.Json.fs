module Json

open Model
open Newtonsoft.Json
open System.Text
open System.IO

let private getPath = function
  | Some name -> sprintf "%s.%s.json" name
  | None -> sprintf "%s.json"

let private formatPhrases ph phrases = 
  let sb = StringBuilder ()
  use writer = new JsonTextWriter (new StringWriter (sb))
  writer.Formatting <- Formatting.Indented
  writer.WriteStartObject()
  for t in phrases do
    let value:string = t.Value |> ph (sprintf "{%i}")
    writer.WritePropertyName (t.Key, true)
    writer.WriteValue (value)
  writer.WriteEndObject ()
  writer.Close ()
  sb.ToString ()

let format name ph lang phrases = {
    Path = (getPath name lang) 
    Contents = (formatPhrases ph phrases)
    Phrases = Seq.length phrases
}