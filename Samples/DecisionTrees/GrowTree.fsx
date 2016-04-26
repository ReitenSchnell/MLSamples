#I @"..\packages\"
#r @"FSharp.Data.2.2.5\lib\net40\FSharp.Data.dll"
#load "Tree.fs"

open FSharp.Data
open System
open Titanic.Tree

type Titanic = CsvProvider<"titanic.csv">
type Passenger = Titanic.Row

let dataset = Titanic.GetSample()

let label(p:Passenger) = p.Survived
let features = [
    "Gender", fun(p:Passenger) -> p.Sex |> Some
    "Class", fun(p:Passenger) -> p.Pclass |> string |> Some
    "Age", fun(p:Passenger) -> if p.Age < 7.0 then Some("Yonger") else Some("Older")]

let filters = [entropyGainFilter; leafSizeFilter 10]

let tree = growTree filters dataset.Rows label (features |> Map.ofList)

dataset.Rows
|> Seq.averageBy(fun p -> if p.Survived = decide tree p then 1. else 0.)

//display 1 tree

let folds = dataset.Rows |> Seq.toArray |> kfold 10

let accuracy tree (sample : Passenger seq) =
    sample
    |> Seq.averageBy(fun p ->
        if p.Survived = decide tree p then 1.0 else 0.0)

let evaluateFolds =
    let filters = [entropyGainFilter; leafSizeFilter 10]
    let features = features |> Map.ofList
    [for (training, validation) in folds ->
        let tree = growTree filters training label features
        let accuracyTraining = accuracy tree training
        let accuracyValidation = accuracy tree validation
        printfn "Training:%.3f, Validation:%.3f" accuracyTraining accuracyValidation
        accuracyTraining, accuracyValidation]
     