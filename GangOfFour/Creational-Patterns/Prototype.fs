(***************************************************************** 
 * Prototype (page 117)
 *  
 * Design Patterns: Elements of Reusable Object-Oriented Software
 *   by Gamma, Helm, Johnson, & Vlissides
 * Sample Code Ported to F#
 *
 * - Initally Ported by Jay Tuley (Jun 2020)
 *
 *****************************************************************)
namespace GangOfFour.Prototype

open GangOfFour.AbstractFactory

///Copy Extensions - So we don't need to clutter the abstract factory example
module CopyExtensions =
    type Reflect = System.Activator
    type Wall with
        member this.CopyWall() : Wall =
            downcast Reflect.CreateInstance(this.GetType())
    type Room with
        member this.CopyRoom(n:int) : Room =
            downcast Reflect.CreateInstance(this.GetType(),n)
        static member Prototype<'T when 'T :> Room> () : 'T = 
            downcast Reflect.CreateInstance(typeof<'T>,0)
    type Door with
        member this.CopyDoor(r1:Room, r2:Room) : Door =
            downcast Reflect.CreateInstance(this.GetType(),r1,r2)
        static member Prototype<'T when 'T :> Door> () : 'T = 
            downcast Reflect.CreateInstance(typeof<'T>,Room.Prototype<Room>(), Room.Prototype<Room>())   
    type Maze with
        member this.CopyMaze() : Maze =
            downcast Reflect.CreateInstance(this.GetType())

open CopyExtensions
///General Prototype Factory
type MazePrototypeFactory (maze:Maze, wall:Wall, room:Room, door:Door) =
    inherit MazeFactory()
    override _.MakeWall() = wall.CopyWall()
    override _.MakeDoor(r1,r2) = door.CopyDoor(r1,r2)
    override _.MakeRoom(n) = room.CopyRoom(n)
    override _.MakeMaze() = maze.CopyMaze()

///Prototype Instances
module MazeFactories =
    let simpleMazeFactor = MazePrototypeFactory(Maze(), Wall(), Room.Prototype<Room>(), Door.Prototype<Door>())
    let bombedMazeFactor = MazePrototypeFactory(Maze(), BombedWall(), Room.Prototype<RoomWithABomb>(), Door.Prototype<Door>())