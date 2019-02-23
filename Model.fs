module Model

type Token = { 
  Key : string
  Value : string 
}

type Input =
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
    Input = Input.File ""
    Format = Resx
    BaseDir = "."
  }