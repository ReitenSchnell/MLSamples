#I @"..\packages\"
#r @"FSharp.Data.2.2.5\lib\net40\FSharp.Data.dll"
open FSharp.Data

type Titanic = CsvProvider<"titanic.csv">
type Passenger = Titanic.Row

let dataset = Titanic.GetSample()

let entropy extract data =
    let size = data |> Seq.length
    data
    |> Seq.map (fun obs -> extract obs)
    |> Seq.countBy id
    |> Seq.map (fun (_, count) -> float count/float size)
    |> Seq.sumBy (fun f -> if f > 0. then -f*log f else 0.)

let hasData extractFeature = extractFeature >> Option.isSome

let splitEntropy extractLabel extractFeature data =
    let dataWithValues =
        data
        |> Seq.filter(extractFeature |> hasData)
    let size = dataWithValues |> Seq.length
    dataWithValues
    |> Seq.groupBy(extractFeature)
    |> Seq.sumBy(fun (_, group) ->
        let groupSize = group|> Seq.length
        let probaGroup = float groupSize / float size
        let groupEntropy = group |> entropy extractLabel
        probaGroup * groupEntropy)

let survived (p:Passenger) = p.Survived
let gender (p:Passenger) = Some(p.Sex)
let port(p:Passenger) = 
    if p.Embarked = "" then None
    else Some(p.Embarked)  
let age (p:Passenger) =
    if p.Age < 12.0
    then Some("Yonger")
    else Some("Older")
let pclass (p:Passenger) = Some(p.Pclass |> string)

let byGender = dataset.Rows |> Seq.groupBy gender
for (groupName, group) in byGender do 
    printfn "Group: %s" groupName.Value
    let h = group |> entropy survived
    printfn "Base entropy: %.3f" h

    group |> splitEntropy survived gender |> printfn "Gender: %.3f"
    group |> splitEntropy survived age |> printfn "Age: %.3f"
    group |> splitEntropy survived pclass |> printfn "Class: %.3f"
    group |> splitEntropy survived port |> printfn "Port: %.3f"

let features = 
    ["Gender", gender
     "Class", pclass]
features
|> List.map (fun(name, feat) ->
    dataset.Rows
    |> splitEntropy survived feat |> printfn "%s: %.3f" name)

let ages = dataset.Rows |> Seq.map(fun obs -> obs.Age) |> Seq.distinct
let best =
    ages
    |> Seq.minBy (fun age ->
        let age (p:Passenger) = 
            if p.Age < age then Some("Yonger") else Some("Older")
        dataset.Rows |> splitEntropy survived age)
printfn "Best age split: %.3f"