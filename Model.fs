module Model

open System.Text.RegularExpressions

module Column =
  let [<Literal>] Key = "Key"
  let [<Literal>] Default = "Default"
  let [<Literal>] Comment = "Comment"

type Phrase = { 
  Key : string
  Value : string 
  Comment : string option
}

type OutputFile = {
  Path : string
  Contents : string
  Phrases : int
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