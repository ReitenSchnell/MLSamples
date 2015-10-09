namespace NaiveBayes

module Classifier =

    let hello name = printfn "Hello, %s" name

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




