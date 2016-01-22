module SpamFilterTests

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
