module NaiveBayesClassifierTests

open Xunit
open Swensen.Unquote
open NaiveBayes.Classifier
open System

[<Fact>]
let ``Proportion counts correctly`` () =
    let result = proportion 2 4    
    0.5 =! result

[<Fact>]
let ``Laplace counts correctly`` () =
    let result = laplace 2 4
    0.6 =! result

let setupDocsGroup =
    let map = [("token1", 0.2); ("token2", 0.3)]|> Map.ofSeq
    let group = {
        Proportion = 0.1 
        TokenFrequiencies = map
    }
    group

[<Fact>]
let ``Scores token to 0.0 when not in group`` () =
    let group = setupDocsGroup
    let result = tokenScore group "token3"
    0.0 =! result

[<Fact>]
let ``Scores token to freq group when not in group`` () =
    let group = setupDocsGroup
    let result = tokenScore group "token2"
    log(0.3) =! result

[<Fact>]
let ``Document should be scored`` () =
    let document = set ["token1"; "token2"; "token3"]
    let group = setupDocsGroup
    let result = score document group
    let expected = log(0.1) + log(0.2) + log(0.3)
    Assert.True(Math.Abs(expected-result) < 0.000001)

[<Fact>]
let ``Count of token should be calculated`` () =
    let group = [|set["token1"; "token2"]; set["token3"; "token2"]; set["token3"; "token1"]|]
    let result = countIn group "token3"
    2 =! result

[<Fact>]
let ``Should classify text`` () =
    let text = "token1 token2 token3"
    let tokenizer (str:string) = Set.ofList(str.Split(' ')|> Array.toList)
    let group1 = {
        Proportion = 0.1 
        TokenFrequiencies = [("token1", 0.2); ("token2", 0.3)]|> Map.ofSeq
    }
    let group2 = {
        Proportion = 0.3 
        TokenFrequiencies = [("token2", 0.2); ("token3", 0.3)]|> Map.ofSeq
    }
    let group3 = {
        Proportion = 0.1 
        TokenFrequiencies = [("token1", 0.2); ("token3", 0.3)]|> Map.ofSeq
    }
    let result = classify [|("Class1", group1); ("Class2", group2); ("Class3", group3);|] tokenizer text
    "Class2" =! result

[<Fact>]
let ``Should analyze group of documents and define proportion size`` () =
    let group = [|set["token1"; "token2"]; set["token3"; "token4"]; set["token3"; "token1"]|]
    let tokens = set["token1"; "token2"]
    let result = analyze group 20 tokens
    result.Proportion =! float 3/float 20

[<Fact>]
let ``Should analyze group of documents and define token frequiencies`` () =
    let group = [|set["token1"; "token2"]; set["token3"; "token4"]; set["token3"; "token1"]|]
    let tokens = set["token1"; "token2"]
    let result = analyze group 20 tokens
    let expected = [("token1", 0.75); ("token2", 0.5)]|> Map.ofSeq
    result.TokenFrequiencies =! expected

[<Fact>]
let ``Should convert list of labels and strings to organized model`` () =
    let tokenizer (str:string) = Set.ofList(str.Split(' ')|> Array.toList)
    let docs = [|("label1", "token1 token2"); ("label1", "token3 token1"); ("label2", "token3 token2"); ("label2", "token4 token2")|]
    let tokens = set["token1"; "token2"]
    let result = learn docs tokenizer tokens
    let expected1 = ("label1", {
        Proportion = 0.5 
        TokenFrequiencies = [("token1", 1.0); ("token2", 2.0/3.0)]|> Map.ofSeq
    })
    let expected2 = ("label2", {
        Proportion = 0.5 
        TokenFrequiencies = [("token1", 1.0/3.0); ("token2", 1.0)]|> Map.ofSeq
    })
    result =! [|expected1; expected2|]

[<Fact>]
let ``Should train model and get results for different words`` () =
    let tokenizer (str:string) = Set.ofList(str.Split(' ')|> Array.toList)
    let docs = [|("label1", "token1 token2"); ("label1", "token3 token1"); ("label2", "token3 token2"); ("label2", "token4 token2")|]
    let tokens = set["token1"; "token2"]
    let classifier = train docs tokenizer tokens
    classifier "token1" =! "label1"
    classifier "token2" =! "label2"

[<Fact>]
let ``Should validate model`` () =
    let docs = [|("label1", "token1 token2"); ("label1", "token3 token1"); ("label2", "token3 token2"); ("label2", "token4 token2")|]
    let classifier (txt:string) = "label1"    
    let result = validate docs classifier
    result =! 0.5


