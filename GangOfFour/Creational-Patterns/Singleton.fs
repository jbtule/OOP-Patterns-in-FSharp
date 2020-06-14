module GangOfFour.Singleton

open GangOfFour.AbstractFactory

///Singleton Lazy Store
let private instance =
    lazy 
        let mazeStyle = System.Environment.GetEnvironmentVariable("MAZESTYLE") 
        let fac:MazeFactory =
            match mazeStyle with
            | "bombed" -> upcast BombMazeFactory()
            | "enchanted" -> upcast EnchantedMazeFactory()
            | _ -> MazeFactory()
        fac

//Singleton added to Maze Factory class
type MazeFactory with
    static member Instance () = instance.Force()