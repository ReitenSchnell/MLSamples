#I @"..\packages\"
#r @"Accord.3.0.2\lib\net45\Accord.dll"
#r @"Accord.MachineLearning.3.0.2\lib\net45\Accord.MachineLearning.dll"
#r @"Accord.Math.3.0.2\lib\net45\Accord.Math.dll"
#r @"Accord.Statistics.3.0.2\lib\net45\Accord.Statistics.dll"

open Accord.Statistics.Models.Regression
open Accord.Statistics.Models.Regression.Fitting

fsi.ShowDeclarationValues <- false

let readLogistic fileName =
    let path = fileName
    path
    |> System.IO.File.ReadAllLines
    |> fun lines -> lines.[1..]
    |> Array.map (fun line -> 
        let parsed = line.Split ',' |> Array.map float
        parsed.[0], parsed.[1..])

let training = readLogistic @"C:\Repository\Data\Samples\trainingsample.csv"
let validation = readLogistic @"C:\Repository\Data\Samples\validationsample.csv"

let labeler x =
    match x with
    | 4. -> 0.
    | 9. -> 1.
    | _ -> failwith "unexpected label"

let fours = training |> Array.filter (fun (label, _) -> label = 4.)
let nines = training |> Array.filter (fun (label, _) -> label = 9.)

let labels, images =
    Array.append fours nines
    |> Array.map(fun (label, image) -> labeler label, image)
    |> Array.unzip

let features = 28 * 28
let model = LogisticRegression(features)

let trainLogistic(model) =
    let learner = LogisticGradientDescent(model)
    let minDelta = 0.001
    let rec improve () =
        let delta = learner.Run(images, labels)
        printfn "%.4f" delta
        if delta > minDelta
        then improve()
        else ignore()
    improve()

trainLogistic model |> ignore

let accuracy () =
    validation
    |> Array.filter(fun (label,_) -> label = 4. || label = 9.)
    |> Array.map(fun(label, image) -> labeler label, image)
    |> Array.map(fun(label,image) ->
        let predicted = if model.Compute(image) > 0.5 then 1. else 0.
        let real = label
        if predicted = real then 1. else 0.)
    |> Array.average

let acc = accuracy()
 