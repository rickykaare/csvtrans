module Resx

open System
open System.IO
open System.Text
open Resx.Resources
open Model

let private getPath name lang = 
  Option.defaultValue "Resources" name
  |> match lang with
      | Column.Default -> sprintf "%s.resx"
      | s -> (fun f -> sprintf "%s.%s.resx" f s)

let private formatPhrases ph phrases =
  let sb = StringBuilder ()
  let typeConverter (t:Type) = 
    t.FullName.Replace("Resx.Resources.", "System.Resources.")
  use writer = new ResXResourceWriter (new StringWriter (sb), typeConverter)
  let createNode t = 
    let value:string = t.Value |> ph (sprintf "{%i}")
    let node = ResXDataNode (t.Key,value,null)
    match (t.Comment) with
    | Some c -> node.Comment <- c; node
    | None -> node
  phrases 
  |> Seq.map createNode
  |> Seq.iter writer.AddResource
  writer.Close ()
  sb.ToString ()

let format name ph lang phrases = {
  Path = (getPath name lang)
  Contents = (formatPhrases ph phrases)
  Phrases = Seq.length phrases
}