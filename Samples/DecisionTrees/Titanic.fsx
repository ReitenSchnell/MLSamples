#I @"..\packages\"
#r @"FSharp.Data.2.2.5\lib\net40\FSharp.Data.dll"
open FSharp.Data

type Titanic = CsvProvider<"titanic.csv">
type Passenger = Titanic.Row

let dataset = Titanic.GetSample()

dataset.Rows
|> Seq.countBy(fun passenger -> passenger.Survived)
|> Seq.iter (printfn "%A")

dataset.Rows
|> Seq.averageBy(fun passenger -> if passenger.Survived then 1.0 else 0.0)
|> printfn "Chances to survive: %.3f"

let survivalRate (passengers:Passenger seq) =
    let total = passengers |> Seq.length
    let survivors = 
        passengers
        |> Seq.filter(fun p -> p.Survived)
        |> Seq.length
    100.0 * (float survivors/float total)

let bySex =
    dataset.Rows
    |> Seq.groupBy(fun p -> p.Sex)

bySex|> Seq.iter(fun (s,g) -> printfn "Sex %A: %f" s (survivalRate g))

let byClass = 
    dataset.Rows
    |> Seq.groupBy(fun p -> p.Pclass)

byClass
|> Seq.iter(fun (s,g) ->
    printfn "Class %A: %f" s (survivalRate g))

let mostFrequentLabelIn group =
    group
    |> Seq.countBy snd
    |> Seq.maxBy snd
    |> fst

let learn sample extractFeature extractLabel =
    let groups =
        sample
        |> Seq.map (fun obs -> extractFeature obs, extractLabel obs)
        |> Seq.groupBy fst
        |> Seq.map (fun (feat, group) -> feat, mostFrequentLabelIn group)

    let classifier obs =
        let featureValue = extractFeature obs
        groups
        |> Seq.find (fun (f,_) -> f = featureValue)
        |> snd

    classifier

let survived (p:Passenger) = p.Survived
let sex (p:Passenger) = p.Sex
let sexClassifier = learn (dataset.Rows) sex survived

printfn "Stump based on passenger sex"
dataset.Rows
|> Seq.averageBy(fun p -> if p.Survived = sexClassifier p then 1.0 else 0.0)

let classClassifier = learn (dataset.Rows) (fun p -> p.Pclass) survived
printfn "Stump based on passenger class"
dataset.Rows
|> Seq.averageBy(fun p -> if p.Survived = classClassifier p then 1.0 else 0.0)

let survivalByPricePaid = 
    dataset.Rows
    |> Seq.groupBy(fun p -> p.Fare)
    |> Seq.iter (fun (price, passengers) -> 
        printf "%6.2F: %6.2f" price (survivalRate passengers))

let averageFare =
    dataset.Rows
    |> Seq.averageBy(fun p -> p.Fare)

let fareLevel (p:Passenger) =
    if p.Fare < averageFare
    then "Cheap"
    else "Expensive"

printfn "Stump: classify by fare level"
let fareClassifier = learn (dataset.Rows) fareLevel survived
dataset.Rows
|> Seq.averageBy(fun p -> if p.Survived = fareClassifier p then 1.0 else 0.0)

let survivalByPortOfOrigin = 
    dataset.Rows
    |> Seq.groupBy(fun p -> p.Embarked)
    |> Seq.iter (fun (port, passengers) -> 
        printfn "%s: %f" port (survivalRate passengers))

let hasData extractFeature = extractFeature >> Option.isSome

let betterLearn sample extractFeature extractLabel =
    let branches =
        sample
        |> Seq.filter(extractFeature |> hasData)
        |> Seq.map(fun obs -> extractFeature obs |> Option.get, extractLabel obs)
        |> Seq.groupBy fst
        |> Seq.map (fun(feat, group) -> feat, mostFrequentLabelIn group)
        |> Map.ofSeq
    let labelForMissingValues =
        sample
        |> Seq.countBy extractLabel
        |> Seq.maxBy snd
        |> fst
    let classifier obs =
        let featureValue = extractFeature obs
        match featureValue with
        | None -> labelForMissingValues
        | Some(value) -> 
            match (branches.TryFind value) with
            | None -> labelForMissingValues
            | Some(predictedLabel) -> predictedLabel
    classifier
    
let port(p:Passenger) = 
    if p.Embarked = "" then None
    else Some(p.Embarked)        

let updatedClassifier = betterLearn dataset.Rows port survived 
dataset.Rows
|> Seq.averageBy(fun p -> if p.Survived = updatedClassifier p then 1.0 else 0.0)

let entropy extract data =
    let size = data |> Seq.length
    data
    |> Seq.map (fun obs -> extract obs)
    |> Seq.countBy id
    |> Seq.map (fun (_, count) -> float count/float size)
    |> Seq.sumBy (fun f -> if f > 0. then -f*log f else 0.)

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

let age (p:Passenger) =
    if p.Age < 12.0
    then Some("Yonger")
    else Some("Older")

let gender (p:Passenger) = Some(p.Sex)

let h = dataset.Rows |> entropy survived
dataset.Rows |> splitEntropy survived age |> printfn "By Age: %3f"
dataset.Rows |> splitEntropy survived port |> printfn "By Port: %3f"
dataset.Rows |> splitEntropy survived gender |> printfn "By Gender: %3f"
 