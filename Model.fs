module Model

open System.Text.RegularExpressions

module Column =
  let [<Literal>] Key = "Key"
  let [<Literal>] Default = "Default"
  let [<Literal>] Comment = "Comment"

type Token = { 
  Key : string
  Value : string 
  Comment : string option
}

type LanguageFile = {
  Path : string
  Contents : string
}

type OutputFormat =
  | Apple
  | Android 
  | Resx

type Options = {
  InputUrl : string
  Format : OutputFormat
  OutputDir : string
  Name : string option
  Placeholders : Regex option
} 

module Options = 
  let empty = {
    InputUrl = ""
    Format = Resx
    OutputDir = "."
    Name = None
    Placeholders = None
  }