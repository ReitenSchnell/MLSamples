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

let A = vector[1.;2.;3.]
let B = matrix[[1.;2.]; [3.;4.]; [5.;6.]]
let C = A*A
let D = A*B
let E = A*B.Column(1)

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
