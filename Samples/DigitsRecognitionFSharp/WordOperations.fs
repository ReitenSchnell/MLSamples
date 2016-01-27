namespace NaiveBayes
open System.Text.RegularExpressions

module WordOperations =

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
           

