open System.IO
#load "WordOperations.fs"
#load "NaiveBayes.fs"
open NaiveBayes.Classifier
open NaiveBayes.WordOperations

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
   
let validation = dataset.[0..999]
let training = dataset.[1000..]

let tokens = allTokens training
let casedTokens = casedTokens training

let simple = evaluate dataset wordsTokenizer (["txt"] |> set)
let full = evaluate dataset wordsTokenizer tokens
let cased = evaluate dataset casedTokenizer casedTokens



