open System.IO
#load "NaiveBayes.fs"
open NaiveBayes.Classifier
open System.Text.RegularExpressions

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

let matchWords = Regex(@"\w+")

let wordsTokenizer(text:string) =
    text.ToLowerInvariant()
    |> matchWords.Matches
    |> Seq.cast<Match>
    |> Seq.map (fun m->m.Value)
    |> Set.ofSeq
    
let validation = dataset.[0..999]
let training = dataset.[1000..]

let txtClassifier = train training wordsTokenizer (["txt"] |> set)

validation
|> Seq.averageBy(fun(docType, sms) -> if docType = txtClassifier sms then 1.0 else 0.0)
|> printfn "Based on 'txt', correctly classified: %.3f"    
    

