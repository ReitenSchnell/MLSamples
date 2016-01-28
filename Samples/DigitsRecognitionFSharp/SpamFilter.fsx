open System.IO
#load "NaiveBayes.fs"
#load "WordOperations.fs"
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

let txtClassifier = train training wordsTokenizer (["txt"] |> set)
let result = validate validation txtClassifier

let tokens = allTokens training wordsTokenizer
let fullClassifier = train training wordsTokenizer tokens
let fullResult = validate validation fullClassifier     



