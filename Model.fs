module Model

type Token = { 
  Key : string
  Value : string 
}

type OutputFormat =
  | Apple
  | Android 
  | Resx

type Options = {
  InputUrl : string
  Format : OutputFormat
  OutputDir : string
} 

module Options = 
  let empty = {
    InputUrl = ""
    Format = Resx
    OutputDir = "."
  }