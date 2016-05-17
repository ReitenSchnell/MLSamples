#I @"..\packages\"
#r @"Accord.3.0.2\lib\net45\Accord.dll"
#r @"Accord.MachineLearning.3.0.2\lib\net45\Accord.MachineLearning.dll"
#r @"Accord.Math.3.0.2\lib\net45\Accord.Math.dll"
#r @"Accord.Statistics.3.0.2\lib\net45\Accord.Statistics.dll"

open Accord.MachineLearning.VectorMachines
open Accord.MachineLearning.VectorMachines.Learning
open Accord.Statistics.Kernels

let readSvm fileName =
    let path = fileName
    path
    |> System.IO.File.ReadAllLines
    |> fun lines -> lines.[1..]
    |> Array.map (fun line -> 
        let parsed = line.Split ',' 
        parsed.[0] |> int, parsed.[1..] |> Array.map float)

let labels, images = readSvm @"C:\Repository\Data\Samples\trainingsample.csv" |> Array.unzip

let features = 28*28
let classes = 10

let algorithm = 
    fun(svm:KernelSupportVectorMachine)(classInputs:float[][])(classOutputs:int[])(i:int)(j:int) ->
        let strategy = SequentialMinimalOptimization(svm, classInputs, classOutputs)
        strategy :> ISupportVectorMachineLearning

let kernel = Linear()
let svm = new MulticlassSupportVectorMachine(features, kernel, classes)
let learner = MulticlassSupportVectorLearning(svm, images, labels)
let config = SupportVectorMachineLearningConfigurationFunction(algorithm)
learner.Algorithm <- config

let error = learner.Run()

let validation = readSvm @"C:\Repository\Data\Samples\validationsample.csv" 
validation
|> Array.averageBy(fun(l,i) -> if svm.Compute i = l then 1. else 0.)

