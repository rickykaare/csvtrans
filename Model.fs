module Model

open Argu

type Token = { Key : string; Value : string }

type InputType =
    | GoogleSheets
    | Url
    | File

type OutputFormat =
    | Resx
    | Ios
    | Android 

[<Unique>] 
type CliArguments =
    | Sheet of document_id:string*sheet_name:string
    | Url of url:string
    | File of path:string
    | Format of OutputFormat
    | Output of folder_path:string
with 
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | Sheet _ -> "use the specified Google Sheet as input."
            | Url _ -> "use an online cvs file as input."
            | File _ -> "use a local csv file as input."
            | Format _ -> "specify the output translation format."
            | Output _ -> "specify the output folder" 

module CliArguments =
    let Parse progName args = 
        let parser =
            ArgumentParser
                .Create<CliArguments>
                    (progName, errorHandler = ProcessExiter())
        let result = parser.Parse args
        result.GetAllResults()