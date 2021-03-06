﻿#I @"..\packages\"
#load @"FSharp.Charting.0.90.13\FSharp.Charting.fsx"
#load "KMeans.fs"

open System
open System.IO
open FSharp.Charting
open KMeans.Algorithm
open KMeans.Helpers

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

let features = headers.Length

let observations1 = 
    observations
    |> Array.map(Array.map float)
    |> Array.filter (fun x -> Array.sum x > 0.)

let (clusters1, classifier1) =
    let clustering = clusterize distance (centroidOf features)
    let k = 5
    clustering observations1 k

clusters1
|> Seq.iter (fun (id, profile) -> 
    printfn "Cluster %i" id
    profile
    |> Array.iteri (fun i value -> printfn "%16s %.1f" headers.[i] value))

observations1
|> Seq.countBy (fun obs -> classifier1 obs)
|> Seq.iter (fun (clusterId, count) ->
    printfn "Cluster %i has %i elements" clusterId count)

Chart.Combine [
    for (id, profile) in clusters1 ->
        profile
        |> Seq.mapi (fun i value -> headers.[i], value)
        |> Chart.Bar
    ]
    |> fun chart -> chart.WithXAxis(LabelStyle = labels)

let observations2 = 
    observations
    |> Array.map(Array.map float)
    |> Array.filter (fun x -> Array.sum x > 0.)
    |> Array.map rowNormalizer

let (clusters2, classifier2) =
    let clustering = clusterize distance (centroidOf features)
    let k = 5
    clustering observations2 k

observations2
|> Seq.countBy (fun obs -> classifier2 obs)
|> Seq.iter (fun (clusterId, count) ->
    printfn "Cluster %i has %i elements" clusterId count)

Chart.Combine [
    for (id, profile) in clusters2 ->
        profile
        |> Seq.mapi (fun i value -> headers.[i], value)
        |> Chart.Bar
    ]
    |> fun chart -> chart.WithXAxis(LabelStyle = labels)

let k_ruleOfThumb = ruleofThumb (observations2.Length)

let data = 
    [1..25]
    |> Seq.map(fun k ->
        let value = 
            [for _ in 1..10 ->
                let (clusters,classifier) =
                    let clustering = clusterize distance (centroidOf features)
                    clustering observations2 k
                AIC observations2 (clusters |> Seq.map snd) ]
            |> List.average
        k, value)
    |> Seq.toArray

Chart.Line data

let (bestClusters, bestClassifier) =
    let clustering = clusterize distance (centroidOf features)
    let k = 10
    seq {
        for _ in 1..20 ->
            clustering observations2 k
    }
    |> Seq.minBy (fun(cs, f) -> RSS observations2 (cs |> Seq.map snd))

bestClusters
|> Seq.iter(fun (id, profile) ->
    printfn "Cluster %i" id
    profile
    |> Array.iteri(fun i value -> if value>0.2 then printfn "%16s %.1f" headers.[i] value))