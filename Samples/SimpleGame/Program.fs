﻿namespace Game

open System
open System.Threading
open Game

module Program =

    let size = {Width = 40; Height = 20}
    let player = {Position = {Top = 10; Left = 20}; Direction = North}
    let rng = Random()
    let board = 
        [   for top in 0..size.Height - 1 do
                for left in 0..size.Width - 1 do
                    if rng.NextDouble() > 0.5
                    then
                        let pos = {Top = top; Left = left}
                        let cell = if rng.NextDouble() > 0.5 then Trap else Treasure
                        yield pos, cell]
        |> Map.ofList
    let score = 0
    let initialGameState = { Board = board; Hero = player; Score = score}
    let choices = [|Straight; Left; Right|]
    let decide () = choices.[rng.Next(3)]
    
    [<EntryPoint>]
    let main argv = 
        let rec loop (state:GameState) =
            let decision = decide()
            let player = state.Hero |> applyDecision size decision
            let board = updateBoard state.Board player
            let gain = computeGain state.Board player
            let score = state.Score + gain
            printfn "%i" score
            let updated = {Board = board; Hero = player; Score = score}
            Thread.Sleep 20
            loop(updated)
        let _ = loop(initialGameState)
        0 