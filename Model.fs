module Model

type Token = { 
  Key : string
  Value : string 
}

type OutputFormat =
  | Resx
  | Ios
  | Android 

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