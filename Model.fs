module Model

type Token = { 
  Key : string
  Value : string 
}

type Input =
  | None
  | Sheet of document:string*sheet:string
  | Url of url:string
  | File of path:string

type OutputFormat =
  | Resx
  | Ios
  | Android 

type Options = {
  Input : Input
  Format : OutputFormat
  BaseDir : string
} 

module Options = 
  let empty = {
    Input = None
    Format = Resx
    BaseDir = "."
  }