(***************************************************************** 
 * Singleton (page 127)
 *  
 * Design Patterns: Elements of Reusable Object-Oriented Software
 *   by Gamma, Helm, Johnson, & Vlissides
 * Sample Code Ported to F#
 *
 * - Initally Ported by Jay Tuley (Jun 2020)
 *
 *****************************************************************)
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