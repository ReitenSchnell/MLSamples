module NaiveBayesWordOperationsTests

open Xunit
open Swensen.Unquote
open NaiveBayes.WordOperations
open System

[<Fact>]
let ``Should tokenize string by words and decapitalize them`` () =
    let text = "Foo boo foo Boo text"
    let result = wordsTokenizer text
    result =! set ["foo"; "boo"; "text"]

[<Fact>]
let ``Should tokenize string by words and leave them fully cased`` () =
    let text = "Foo boo boo foo Boo TEXT text"
    let result = casedTokenizer text
    result =! set ["foo"; "boo"; "text"; "TEXT"; "Foo"; "Boo"]

[<Fact>]
let ``Should create vocabulary`` () =
    let corpus = [|"token1 token2"; "token3 token1"; "token3 token2"; "token4 token2"|] |> Array.ofSeq
    let tokenizer (str:string) = Set.ofList(str.Split(' ')|> Array.toList)
    let result = vocabulary tokenizer corpus
    result =! set ["token1"; "token2"; "token3"; "token4"]

[<Fact>]
let ``Should collect all tokens`` () =
    let docs = [|("label1", "token1 token2"); ("label1", "token3 token1"); ("label2", "token3 token2"); ("label2", "token4 token2")|]
    let tokenizer (str:string) = Set.ofList(str.Split(' ')|> Array.toList)
    let result = allTokens docs
    result =! set ["token1"; "token2"; "token3"; "token4"]

[<Fact>]
let ``Should collect cased tokens`` () =
    let docs = [|("label1", "Token1 token2"); ("label1", "Token3 token1"); ("label2", "token3 token2"); ("label2", "token4 token2")|]
    let tokenizer (str:string) = Set.ofList(str.Split(' ')|> Array.toList)
    let result = casedTokens docs
    result =! set ["token1"; "token2"; "token3"; "token4"; "Token1"; "Token3"]

