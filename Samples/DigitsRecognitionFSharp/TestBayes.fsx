open System.IO
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

let validation = dataset.[0..999]
let training = dataset.[1000..]

let matchWords = Regex(@"\w+")

let wordsTokenizer(text:string) =
    text.ToLowerInvariant()
    |> matchWords.Matches
    |> Seq.cast<Match>
    |> Seq.map (fun m->m.Value)
    |> Set.ofSeq

type Token = string
type Tokenizer = string -> Token Set
type TokenizedDocument = Token Set

type DocsGroup =
    { Proportion:float;  
        TokenFrequiencies:Map<Token, float>}

let tokenScore (group:DocsGroup) (token:Token) =
    if group.TokenFrequiencies.ContainsKey token
    then log group.TokenFrequiencies.[token]
    else 0.0

let score (document:TokenizedDocument) (group:DocsGroup) =
    let scoreToken = tokenScore group
    log group.Proportion +
    (document |> Seq.sumBy scoreToken)

let classify (groups:(_*DocsGroup)[]) (tokenizer:Tokenizer) (txt:string) =
    let tokenized = tokenizer txt
    groups
    |> Array.maxBy(fun(label, group) -> score tokenized group)
    |> fst

let proportion count total = float count / float total

let laplace count total = float(count + 1) / float(total + 1)

let countIn (group:TokenizedDocument seq)(token:Token) = 
    group
    |> Seq.filter(Set.contains token)
    |> Seq.length

let analyze (group:TokenizedDocument seq)
    (totalDocs:int)
    (classificationTokens:Token Set) =
    let groupSize = group|>Seq.length
    let score token = 
        let count = countIn group token
        laplace count groupSize
    let scoredTokens =
        classificationTokens
        |> Set.map(fun token -> token, score token)
        |> Map.ofSeq
    let groupProportion = proportion groupSize totalDocs
    {

        Proportion = groupProportion
        TokenFrequiencies = scoredTokens
    }

let learn (docs:(_ * string)[]) 
            (tokenizer:Tokenizer)
            (classificationTokens:Token Set) =
    let total = docs.Length
    docs
    |> Array.map(fun(label,docs) -> label, tokenizer docs)
    |> Seq.groupBy fst
    |> Seq.map(fun(label, group) -> label, group|>Seq.map snd)
    |> Seq.map(fun(label, group) -> label, analyze group total classificationTokens)
    |> Seq.toArray

let train (docs:(_ * string)[])
            (tokenizer:Tokenizer)
            (classificationTokens: Token Set) =
    let groups = learn docs tokenizer classificationTokens
    let classifier = classify groups tokenizer
    classifier

let txtClassifier = train training wordsTokenizer (["txt"] |> set)

validation
|> Seq.averageBy(fun(docType, sms) -> if docType = txtClassifier sms then 1.0 else 0.0)
|> printfn "Based on 'txt', correctly classified: %.3f"  
