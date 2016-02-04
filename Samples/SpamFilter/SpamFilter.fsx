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

let ham, spam = 
    let hamRaw, spamRaw = 
        training
        |> Array.partition(fun(lbl, _) -> lbl = Ham)
    hamRaw|> Array.map snd, spamRaw|> Array.map snd

let hamCount = ham |> vocabulary casedTokenizer |> Set.count
let spamCount = spam |> vocabulary casedTokenizer |> Set.count

let topHam = top (hamCount/10) casedTokenizer ham
let topSpam = top (spamCount/10) casedTokenizer spam
let topTokens = Set.union topHam topSpam

let evaluatedTop = evaluate dataset casedTokenizer topTokens

//ham|>top 20 casedTokenizer|>Seq.iter (printfn "%s")
//spam|>top 20 casedTokenizer|>Seq.iter (printfn "%s")

let commonTokens = Set.intersect topSpam topHam
let specificTokens = Set.difference topTokens commonTokens

let evaluateSpecific = evaluate dataset casedTokenizer specificTokens

//let rareHam = rare 50 casedTokenizer ham
//let rareSpam = rare 50 casedTokenizer spam

let smartTokens = specificTokens|> Set.add "__TXT__" |> Set.add "__PHONE__"
let evaluateSmart = evaluate dataset smartTokenizer smartTokens

let lengthAnalysis len =
        let long (msg:string) = msg.Length > len
        let ham, spam = 
            dataset
            |> Array.partition(fun (label, _) -> label = Ham)
        let spamAndLongCount =
            spam
            |> Array.filter (fun(_, sms) -> long sms)
            |> Array.length
        let longCount =
            dataset
            |> Array.filter (fun(_, sms) -> long sms)
            |> Array.length
        let pSpam = (float spam.Length) / (float dataset.Length)
        let pLongIfSpam = (float spamAndLongCount) / (float longCount)
        let pLong = (float longCount) / (float dataset.Length)
        let pSpamIfLong = pLongIfSpam * pSpam/pLong
        pSpamIfLong

for l in 10..10..130 do
    printfn "Probability for %i is %.4f" l (lengthAnalysis l)





