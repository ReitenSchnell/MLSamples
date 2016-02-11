#I @"..\packages\"
#r @"FSharp.Data.2.2.5\lib\net40\FSharp.Data.dll"
#r @"FSharp.Data.2.2.5\lib\net40\FSharp.Data.DesignTime.dll"
#load @"FSharp.Charting.0.90.13\FSharp.Charting.fsx"

open FSharp.Data
open FSharp.Charting

type Data = CsvProvider<"day.csv">
let dataset = Data.Load("day.csv")
let data = dataset.Rows
let all = Chart.Line [for obs in data -> obs.Cnt]

let windowedExample = 
    [1..10]
    |> Seq.windowed 3
    |> Seq.toList

let movingAverages n (series: float list) =
    series
    |> Seq.windowed n
    |> Seq.map (fun xs -> xs|>Seq.average)
    |> Seq.toList

let cnts = [for obs in data -> (float)obs.Cnt]

Chart.Combine[
    Chart.Line cnts
    Chart.Line (movingAverages 7 cnts)
    Chart.Line (movingAverages 30 cnts)]
    

