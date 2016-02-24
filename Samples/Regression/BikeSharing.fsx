#I @"..\packages\"
#r @"FSharp.Data.2.2.5\lib\net40\FSharp.Data.dll"
#r @"FSharp.Data.2.2.5\lib\net40\FSharp.Data.DesignTime.dll"
#load @"FSharp.Charting.0.90.13\FSharp.Charting.fsx"

open FSharp.Data
open FSharp.Charting

type Data = CsvProvider<"day.csv">
let dataset = Data.Load("day.csv")
let data = dataset.Rows

type Obs = Data.Row

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

let baseline =
    let avg = data|>Seq.averageBy (fun x -> float x.Cnt)
    data |> Seq.averageBy (fun x -> abs(avg - float x.Cnt))

let model (theta0, theta1) (obs:Obs) = 
    theta0 + theta1 * (float obs.Instant)

let model1 = model (4504.0, 0.0)
let model2 = model (6000.0, -4.5)

Chart.Combine[
    Chart.Line cnts
    Chart.Line [for obs in data -> model1 obs]
    Chart.Line [for obs in data -> model2 obs]
]

type Model = Obs -> float

let cost (data:Obs seq)(m:Model) =
    data
    |> Seq.sumBy (fun x -> pown (float x.Cnt - m x) 2 )
    |> sqrt

let overallcost = cost data
overallcost model1 |> printfn "Cost model1: %.0f" 
overallcost model2 |> printfn "Cost model2: %.0f"

let update alpha (theta0, theta1) (obs:Obs) =
    let y = float obs.Cnt
    let x = float obs.Instant
    let theta0' = theta0 - 2.*alpha*1.*(theta0 + theta1*x - y) 
    let theta1' = theta1 - 2.*alpha*x*(theta0 + theta1*x - y) 
    theta0', theta1'
    
let obs100 = data |> Seq.nth 100
let testUpdate = update 0.00001 (0., 0.) obs100
cost [obs100] (model (0.0, 0.0))
cost [obs100] (model testUpdate)

let stochastic rate (theta0 ,theta1) =
    data
    |> Seq.fold (fun (t0, t1) obs ->
        printfn "%.4f, %.4f" t0 t1
        update rate (t0, t1) obs) (theta0, theta1)

let tune_rate = 
    [for r in 1..20 ->
        (pown 0.1 r), stochastic (pown 0.1 r) (0., 0.) |> model |> overallcost]

let rate = pown 0.1 8
let model3 = model (stochastic rate (0.0, 0.0))

Chart.Combine[
    Chart.Line cnts
    Chart.Line [for obs in data -> model3 obs]
]

let hiRate = 10. * rate
let error_eval = 
    data
    |> Seq.scan(fun(t0, t1) obs -> update hiRate (t0, t1) obs) (0.0, 0.0)
    |> Seq.map(model >> overallcost)
    |> Chart.Line