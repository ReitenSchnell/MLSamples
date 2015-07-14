open System.IO

type DataPoint = {Label : string; Pixels:int[]}

let toDataPoint (csvData:string) = 
    let columns = csvData.Split(',')
    let label = columns.[0]
    let pixels = columns.[1..] |> Array.map int
    {Label = label; Pixels = pixels}

let reader path = 
    let data = File.ReadAllLines path
    data.[1..]
    |> Array.map toDataPoint

let trainingPath =  @"C:\Data\Repos\Data\trainingsample.csv"

let trainingData = reader trainingPath

let manhattanDistance (pixels1, pixels2) = 
    Array.zip pixels1 pixels2
    |> Array.map (fun(x,y) -> abs(x-y))
    |> Array.sum

let train (trainingSet:DataPoint[]) = 
    let classify(pixels:int[]) =
        trainingSet
        |> Array.minBy (fun x -> manhattanDistance x.Pixels pixels)
        |> fun x -> x.Label
    classify

let classifier = train trainingData

