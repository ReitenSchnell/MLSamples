namespace Game

open System
open Game

module Brain =
    type State = Dir * int list
    type Experience = {
        State:State;
        Action:Act;
        Reward:float;
        NextState:State
    }
    type Strategy = {State:State; Action:Act;}
    type Brain = Map<Strategy, float>

    let rng = Random()
    let choices = [|Straight; Left; Right;|]
    let randomDecide() = choices.[rng.Next(choices|>Array.length)]

    let alpha = 0.2
    let learn(brain:Brain)(exp:Experience) =
        let strategy = {State = exp.State; Action = exp.Action}
        match brain.TryFind strategy with
        | Some(value) -> brain.Add(strategy, (1.0 - alpha)*value + alpha*exp.Reward)
        | None -> brain.Add(strategy, alpha*exp.Reward)

    let decide (brain:Brain)(state:State) =
        let knownStrategies =
            choices
            |> Array.map(fun alt -> {State = state; Action = alt})
            |> Array.filter(fun strategy -> brain.ContainsKey strategy)
        match knownStrategies.Length with
        |0 -> randomDecide()
        |_ -> 
            choices
            |> Seq.maxBy(fun alt ->
                let strategy = {State = state; Action = alt;}
                match brain.TryFind strategy with
                |Some(value) -> value
                |None -> 0.0)

    let tileAt (board:Board)(pos:Pos) = board.[pos.Left, pos.Top]

    let offsets = 
        [
            (-1,-1)
            (-1,0)
            (-1,1)
            (0,-1)
            (0,1)
            (1,-1)
            (1,0)
            (1,1)]

    let visibleState (size:Size)(board:Board)(hero:Hero) =  
        let (dir, pos) = hero.Direction, hero.Position
        let visibleCells =
            offsets
            |> List.map(fun(x,y) -> 
                onboard size {Top = pos.Top + x; Left = pos.Left + y} |> tileAt board)
        (dir, visibleCells)
        


