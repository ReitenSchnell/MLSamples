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

