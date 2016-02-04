namespace NaiveBayes
open System.Text.RegularExpressions

module WordOperations =

    type Token = string
    type Tokenizer = string -> Token Set
    type TokenizedDocument = Token Set

    let matchWords = Regex(@"\w+")
    
    let wordsTokenizer(text:string) =
        text.ToLowerInvariant()
        |> matchWords.Matches
        |> Seq.cast<Match>
        |> Seq.map (fun m->m.Value)
        |> Set.ofSeq
        
    let casedTokenizer(text:string) =
        text
        |> matchWords.Matches
        |> Seq.cast<Match>
        |> Seq.map (fun m->m.Value)
        |> Set.ofSeq

    let vocabulary (tokenizer:Tokenizer) (corpus:string seq) =
        corpus
        |> Seq.map tokenizer
        |> Set.unionMany

    let allTokens (trainingSet:(_ * string)[]) = 
        trainingSet
        |> Seq.map snd
        |> vocabulary wordsTokenizer

    let casedTokens (trainingSet:(_ * string)[]) =
        trainingSet
        |> Seq.map snd
        |> vocabulary casedTokenizer

    let phoneWords = Regex(@"0[7-9]\d{9}")
    let phone(txt:string) =
        match (phoneWords.IsMatch txt) with
            | true -> "__PHONE__"
            | false -> txt

    let txtCode = Regex(@"\b\d{5}\b")
    let txt(text:string) = 
        match (txtCode.IsMatch text) with
            | true -> "__TXT__"
            | false -> text

    let smartTokenizer = casedTokenizer >> Set.map phone >> Set.map txt    

    
           

