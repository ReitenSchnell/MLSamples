let set1 = set[1;2;3]
let set2 = set[1;3;3;5]
let intersection = Set.intersect set1 set2
let union = Set.union set1 set2
let diff1 = Set.difference set1 set2
let diff2 = Set.difference set2 set1

let arr1 = [|for x in 1..10 -> x|]

let seq1 = seq{for x in 1..10 -> x}
let seq2 = 
    seq1 
    |> Seq.map(fun x -> 
        printfn "mapping %i" x
        2*x) 
let firstThree = 
    seq2
    |> Seq.take 3
    |> Seq.sum

let infinite = Seq.initInfinite (fun x-> if x%2 = 0 then 1 else -1)
let test = infinite |> Seq.take 100000 |> Seq.sum

let arr2 = [|1;2;2;3;3;3;4;4;4|]
let countSeq = arr2|>Seq.countBy(fun x->x)|>Seq.toArray

    