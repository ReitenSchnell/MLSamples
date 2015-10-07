open System.IO

type DataPoint = {Label : string; Pixels:int[]}
type Distance = int[]*int[] -> int

let toDataPoint (csvData:string) = 
    let columns = csvData.Split(',')
    let label = columns.[0]
    let pixels = columns.[1..] |> Array.map int
    {Label = label; Pixels = pixels}

let reader path = 
    let data = File.ReadAllLines path
    data.[1..]
    |> Array.map toDataPoint

let trainingPath =  @"C:\Repository\Data\Samples\trainingsample.csv"

let trainingData = reader trainingPath

let manhattanDistance (pixels1, pixels2) = 
    Array.zip pixels1 pixels2
    |> Array.map (fun(x,y) -> abs(x-y))
    |> Array.sum

let euclideanDistance (pixels1, pixels2) = 
    Array.zip pixels1 pixels2
    |> Array.map (fun(x,y) -> pown (x-y) 2)
    |> Array.sum

let train (trainingSet:DataPoint[]) (dist:Distance) = 
    let classify(pixels:int[]) =
        trainingSet
        |> Array.minBy (fun x -> dist (x.Pixels, pixels))
        |> fun x -> x.Label
    classify

let classifier = train trainingData

let validationPath =  @"C:\Repository\Data\Samples\validationsample.csv"
let validationData = reader validationPath

let predictedCorrectlyCount (dist:Distance) = 
    validationData |>
    Array.averageBy (fun x -> if classifier dist x.Pixels = x.Label then 1.0 else 0.0)

let manhattanScore = predictedCorrectlyCount manhattanDistance
let euclideanScore = predictedCorrectlyCount euclideanDistance
