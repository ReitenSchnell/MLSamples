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
