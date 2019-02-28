module Model

module Column =
  let [<Literal>] Key = "Key"
  let [<Literal>] Default = "Default"
  let [<Literal>] Comment = "Comment"

type Token = { 
  Key : string
  Value : string 
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
} 

module Options = 
  let empty = {
    InputUrl = ""
    Format = Resx
    OutputDir = "."
    Name = None
  }