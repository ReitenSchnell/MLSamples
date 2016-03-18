#I @"..\packages\"
#r @"FSharp.Data.2.2.5\lib\net40\FSharp.Data.dll"
#load @"FSharp.Charting.0.90.13\FSharp.Charting.fsx"
#r @"MathNet.Numerics.Signed.3.11.0\lib\net40\MathNet.Numerics.dll"
#r @"MathNet.Numerics.FSharp.Signed.3.11.0\lib\net40\MathNet.Numerics.FSharp.dll"

open FSharp.Data
open FSharp.Charting
open MathNet
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.Double

type Data = CsvProvider<"day.csv">
let dataset = Data.Load("day.csv")
let data = dataset.Rows

type Vec = Vector<float>
type Mat = Matrix<float>

let cost(theta:Vec)(Y:Vec)(X:Mat) =
    let ps = Y - (theta*X.Transpose())
    ps*ps|> sqrt

let predict(theta:Vec)(v:Vec) = theta*v

let X = matrix[for obs in data -> [1.; float obs.Instant]]
let Y = vector[for obs in data -> float obs.Cnt]
let theta = vector[6000.; -4.5]

predict theta (X.Row(0))
cost theta Y X

let estimate (Y:Vec)(X:Mat)=
    (X.Transpose()*X).Inverse()*X.Transpose()*Y

let estimatedTheta = estimate Y X
let estimatedCost = cost estimatedTheta Y X

let seed = 314159
let rng = System.Random(seed)

let shuffle(arr:'a[]) =
    let arr = Array.copy arr
    let l = arr.Length
    for i in (l-1) .. -1 .. 1 do
        let temp = arr.[i]
        let j = rng.Next(0, i+1)
        arr.[j] <- arr.[i]
        arr.[i] <- temp
    arr

let training, validation =
    let shuffled = 
        data
        |>Seq.toArray
        |>shuffle
    let size = 0.7*float(Array.length shuffled) |> int
    shuffled.[..size], shuffled.[size+1..]

type Obs = Data.Row
type Model = Obs -> float
type Featurizer = Obs -> float list

let predictor (f:Featurizer)(theta:Vec) =
    f >> vector >> (*) theta

let evaluate(model:Model)(data:Obs seq) =
    data
    |> Seq.averageBy (fun obs -> abs(model obs - float obs.Cnt))

let model (f:Featurizer)(data:Obs seq) =
    let Yt, Xt =
        data
        |> Seq.toList
        |> List.map(fun obs -> float obs.Cnt, f obs)
        |> List.unzip
    let theta = estimate(vector Yt)(matrix Xt)
    let predict = predictor f theta
    theta, predict

let featurizer0 (obs:Obs) =
    [1.;
    float obs.Instant]

let (theta0, model0) = model featurizer0 training

evaluate model0 training |> printfn "Training: %.0f"
evaluate model0 validation |> printfn "Validation: %.0f"

let featurizer1 (obs:Obs) =
    [1.;
    float obs.Instant;
    float obs.Atemp;
    float obs.Hum;
    float obs.Temp;
    float obs.Windspeed]

let (theta1, model1) = model featurizer1 training

evaluate model1 training |> printfn "Training: %.0f"
evaluate model1 validation |> printfn "Validation: %.0f"

let featurizer2 (obs:Obs) =
    [1.;
    float obs.Instant;
    float obs.Atemp;
    float obs.Hum;
    float obs.Temp;
    float obs.Windspeed;
    (if obs.Weekday = 0 then 0.0 else 1.0);
    (if obs.Weekday = 1 then 0.0 else 1.0);
    (if obs.Weekday = 2 then 0.0 else 1.0);
    (if obs.Weekday = 3 then 0.0 else 1.0);
    (if obs.Weekday = 4 then 0.0 else 1.0);
    (if obs.Weekday = 5 then 0.0 else 1.0);]

let (theta2, model2) = model featurizer2 training
evaluate model2 training |> printfn "Training: %.0f"
evaluate model2 validation |> printfn "Validation: %.0f"

let featurizer3 (obs:Obs) =
    [1.;
    float obs.Temp;
    obs.Temp*obs.Temp |> float
    ]

let (theta3, model3) = model featurizer3 data

Chart.Combine[
    Chart.Point [for obs in data -> obs.Temp, obs.Cnt]
    Chart.Line [for obs in data -> obs.Temp, model3 obs]]

let featurizer4 (obs:Obs) =
    [1.;
    float obs.Instant;
    float obs.Atemp;
    float obs.Hum;
    float obs.Temp;
    float obs.Windspeed;
    obs.Temp * obs.Temp |> float;
    (if obs.Weekday = 0 then 0.0 else 1.0);
    (if obs.Weekday = 1 then 0.0 else 1.0);
    (if obs.Weekday = 2 then 0.0 else 1.0);
    (if obs.Weekday = 3 then 0.0 else 1.0);
    (if obs.Weekday = 4 then 0.0 else 1.0);
    (if obs.Weekday = 5 then 0.0 else 1.0);]

let (theta4, model4) = model featurizer4 data
evaluate model4 training |> printfn "Training: %.0f"
evaluate model4 validation |> printfn "Validation: %.0f"

Chart.Combine[
    Chart.Line [for obs in data -> float obs.Cnt]
    Chart.Line [for obs in data -> model4 obs]]
