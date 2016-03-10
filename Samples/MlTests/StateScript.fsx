#load "StateMachine.fs"
open Tests.StateMachine

let (|+>) fsm f =
    printfn "Current state: %A" fsm.CurrentState
    f fsm

let fsm =  
    initFSM Idle
    |> registerResetEvent DoorOpened
    |> registerCommandChannel (printfn "Execute command %A")
    |> registertTransition Idle DoorClosed Active
    |> registertTransition Active DrawerOpened WaitingForLight
    |> registertTransition Active LightOn WaitingForDrawer
    |> registertTransition WaitingForLight LightOn UnlockedPanel
    |> registertTransition WaitingForDrawer DrawerOpened UnlockedPanel
    |> registertTransition UnlockedPanel PanelClosed Idle
    |> registerCommand Active UnlockDoor
    |> registerCommand Active LockPanel
    |> registerCommand UnlockedPanel UnlockPanel
    |> registerCommand UnlockedPanel LockDoor

fsm
|+> handleEvent DoorClosed
|+> handleEvent DrawerOpened
|+> handleEvent LightOn
|+> handleEvent PanelClosed
|+> (printfn "Result:%A") 
    
