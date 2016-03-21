﻿#I @"..\packages\"
#load @"FSharp.Charting.0.90.13\FSharp.Charting.fsx"

open System
open System.IO
open FSharp.Charting

let folder = __SOURCE_DIRECTORY__
let file = "userprofiles-toptags.txt"

let headers, observations =
    let raw = folder + "/" + file |> File.ReadAllLines
    let headers = (raw.[0].Split ',').[1..]
    let observations = 
        raw.[1..]
        |> Array.map(fun line -> (line.Split ',').[1..])
        |> Array.map(Array.map float)
    headers, observations

printfn "%16s %8s %8s %8s" "Tag Name" "Avg" "Min" "Max"
headers
|> Array.iteri (fun i name ->
    let col = observations |> Array.map (fun obs -> obs.[i])
    let avg = col |> Array.average
    let min = col |> Array.min
    let max = col |> Array.max
    printfn "%16s %8.1f %8.1f %8.1f" name avg min max)

let labels = ChartTypes.LabelStyle(Interval = 0.25)

headers
|> Seq.mapi (fun i name ->
    name,
    observations |> Seq.averageBy (fun obs -> obs.[i]))
|> Chart.Bar
|> fun chart -> chart.WithXAxis(LabelStyle = labels)

