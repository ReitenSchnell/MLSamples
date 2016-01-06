open System.IO
#load "NaiveBayes.fs"
open NaiveBayes.Classifier

type DocType = 
    |Ham
    |Spam

let parseDocType(label:string)=
    match label with
    |"ham" -> Ham
    |"spam" ->  Spam
    |_ -> failwith "Unknown label"

let parseline (line:string) =
    let split = line.Split('\t')
    let label = split.[0] |> parseDocType
    let message = split.[1]
    (label, message)

let fileName = "SMSSpamCollection"
let path = __SOURCE_DIRECTORY__ + @"..\..\Data\" + fileName

let dataset = 
    File.ReadAllLines path
    |> Array.map parseline

let spamWithFree = 
    dataset
    |> Array.filter (fun(docType, _) -> docType = Spam)
    |> Array.filter (fun(_, content) -> content.Contains "FREE")
    |> Array.length

let hamWithFree = 
    dataset
    |> Array.filter (fun(docType, _) -> docType = Ham)
    |> Array.filter (fun(_, content) -> content.Contains "FREE")
    |> Array.length

let spamSMS = 
    dataset
    |> Array.filter (fun(docType, _) -> docType = Spam)
    |> Array.length

let hamSMS =
    dataset
    |> Array.filter (fun(docType, _) -> docType = Ham)
    |> Array.length

        
    

