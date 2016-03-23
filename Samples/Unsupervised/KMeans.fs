namespace Unsupervized

module Distance =
    type Observation = float[]

    let distance(obs1:Observation)(obs2:Observation) =
        (obs1, obs2)
        ||> Seq.map2(fun u1 u2 -> pown(u1-u2) 2)
        |> Seq.sum

    let centroidOf (features : int) (cluster: Observation seq) =
        Array.init features (fun f ->
            cluster
            |> Seq.averageBy (fun user -> user.[f]))

module KMeans =
    
    let pickFrom size k =
        let rng = System.Random()
        let rec pick (set:int Set) =
            let candidate = rng.Next(size)
            let set = set.Add candidate
            if set.Count = k then set
            else pick set
        pick Set.empty |> Set.toArray

    let initialize observations k =
        let size = Array.length observations
        let centroids = 
            pickFrom size k
            |> Array.mapi (fun i index -> i+1, observations.[index])
        let assignments = 
            observations
            |> Array.map (fun x -> 0, x)
        (assignments, centroids)

    let clusterize distance centroidOf observations k =
        let rec search (assignments, centroids) = 
            let classifier observation =
                centroids
                |> Array.minBy(fun (_, centroid) -> distance observation centroid)
                |> fst

            let assignments' =
                assignments
                |> Array.map(fun (_, observation) -> 
                    let closestCentroidId = classifier observation
                    (closestCentroidId, observation))

            let changed = 
                (assignments, assignments')
                ||> Seq.zip
                |> Seq.exists(fun ((oldClusterId, _), (newClusterId, _)) -> not (oldClusterId = newClusterId))
            
            if changed
            then 
                let centroids' = 
                    assignments'
                    |> Seq.groupBy fst
                    |> Seq.map(fun (clusterId, group) -> 
                        clusterId, group|>Seq.map snd|> centroidOf)
                    |> Seq.toArray
                search(assignments', centroids')
            else centroids, classifier

        let initialValues = initialize observations k
        search initialValues


