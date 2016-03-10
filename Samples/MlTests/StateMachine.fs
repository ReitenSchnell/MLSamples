namespace Tests

module StateMachine =

    type FSM<'TState, 'TEvent, 'TCommand when 'TEvent:comparison and 'TState:comparison> =
            {Transitions:Map<'TState, Map<'TEvent, 'TState>>;
            CurrentState:'TState;
            InitState:'TState;
            Commands: Map<'TState, 'TCommand list>;
            ResetEvents: 'TEvent list;
            CommandChannel: 'TCommand -> unit}

    let initFSM initState =
        {
            InitState = initState
            CurrentState = initState
            Transitions = Map.empty
            Commands = Map.empty
            ResetEvents = []
            CommandChannel = (fun _ -> ())
        }

    let registertTransition currentState event nextState fsm =
        match fsm.Transitions |> Map.tryFind currentState with
        | None -> {fsm with Transitions = fsm.Transitions |> Map.add currentState (Map.empty |> Map.add event nextState)}
        | Some m -> {fsm with Transitions = fsm.Transitions |> Map.add currentState (m|> Map.add event nextState)} 

    let registerCommand state command fsm =
        match fsm.Commands |> Map.tryFind state with
        | None -> {fsm with Commands = fsm.Commands |> Map.add state [command]}
        | Some commands -> {fsm with Commands = fsm.Commands |> Map.add state (command :: commands)}

    let registerResetEvent event fsm = {fsm with ResetEvents = (event :: fsm.ResetEvents)}

    let registerCommandChannel f fsm = {fsm with CommandChannel = f}

    let handleEvent e fsm =
        printfn "Reacting to event %A" e
        let transitionTo state fsm =
            match fsm.Commands |> Map.tryFind state with
            | None -> ()
            | Some commands -> commands |> List.iter fsm.CommandChannel
            {fsm with CurrentState = state}

        match fsm.ResetEvents |> List.tryFind (fun re -> re = e) with
        | Some _ -> transitionTo fsm.InitState fsm
        | None -> 
            fsm.Transitions
            |> Map.tryFind fsm.CurrentState
            |> Option.bind (Map.tryFind e)
            |> Option.bind (fun nextState -> Some(transitionTo nextState fsm))
            |> function
                |None -> fsm
                |Some fsm' -> fsm'
    

    type Event =
        | DoorClosed
        | DrawerOpened
        | LightOn
        | DoorOpened
        | PanelClosed

    type Command =
        | UnlockPanel
        | LockPanel
        | LockDoor
        | UnlockDoor

    type State =
        | Idle
        | Active
        | WaitingForLight
        | WaitingForDrawer
        | UnlockedPanel
    



     
 
  

