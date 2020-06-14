module GangOfFour.Singleton

open GangOfFour.AbstractFactory

let private instance =
    lazy 
        let mazeStyle = System.Environment.GetEnvironmentVariable("MAZESTYLE") 
        let fac:MazeFactory =
            match mazeStyle with
            | "bombed" -> upcast BombMazeFactory()
            | "enchanted" -> upcast EnchantedMazeFactory()
            | _ -> MazeFactory()
        fac

type MazeFactory with
    static member Instance () = instance.Force()