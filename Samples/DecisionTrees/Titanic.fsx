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