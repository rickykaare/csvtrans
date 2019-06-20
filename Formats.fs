module Formats

open System.Text.RegularExpressions
open Model

let private convert placeholder f s =
  match placeholder with
  | None -> s
  | Some (re:Regex) -> 
      let i = ref 0
      re.Replace(s, fun _ -> let r = f !i in incr i; r)
  
let getFormat = function
  | { Format = Apple; Name = n; Placeholders = ph } 
    -> Apple.format n (convert ph)
  | { Format = Android; Name = n; Placeholders = ph } 
    -> Android.format n (convert ph)
  | { Format = Json; Name = n; Placeholders = ph } 
    -> Json.format n (convert ph)
  | { Format = Resx; Name = n; Placeholders = ph } 
    -> Resx.format n (convert ph)