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

let euclideanDistance (pixels1, pixels2) = 
    Array.zip pixels1 pixels2
    |> Array.map (fun(x,y) -> pown (x-y) 2)
    |> Array.sum

let img1 = trainingData.[0].Pixels
let img2 = trainingData.[1].Pixels

let d1 (pixels1, pixels2) = 
    Array.zip pixels1 pixels2
    |> Array.map (fun(x,y) -> (x-y)*(x-y))
    |> Array.sum

let d2 (pixels1, pixels2) = 
    (pixels1, pixels2)
    ||> Array.map2 (fun x y -> (x-y)*(x-y))
    |> Array.sum

printfn "Initial function"
#time "on"
for i in 1..5000 do
    let dist = euclideanDistance (img1, img2)
    ignore()
#time "off"

printfn "Function without pow"
#time "on"
for i in 1..5000 do
    let dist = d1 (img1, img2)
    ignore()
#time "off"

printfn "Function without zip"
#time "on"
for i in 1..5000 do
    let dist = d2 (img1, img2)
    ignore()
#time "off"


