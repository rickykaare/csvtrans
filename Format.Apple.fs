module Apple

open Model

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

let private formatPhrases ph phrases = 
  let createLine phrase =
    sprintf "\"%s\" = \"%s\";"
      (stringEncode phrase.Key)
      (phrase.Value |> ph (fun _ -> "%@") |> stringEncode)
  phrases
  |> Seq.map createLine
  |> String.concat "\n"

let format name ph lang phrases = {
    Path = (getPath name lang) 
    Contents = (formatPhrases ph phrases)
    Phrases = Seq.length phrases
}