﻿#I @"..\packages\"
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

let tree = growTree dataset.Rows label (features |> Map.ofList)

dataset.Rows
|> Seq.averageBy(fun p -> if p.Survived = decide tree p then 1. else 0.)

display 1 tree

