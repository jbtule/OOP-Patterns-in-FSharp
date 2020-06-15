(***************************************************************** 
 * Facade (page 185)
 *  
 * Design Patterns: Elements of Reusable Object-Oriented Software
 *   by Gamma, Helm, Johnson, & Vlissides
 * Sample Code Ported to F#
 *
 * - Initally Ported by Jay Tuley (Jun 2020)
 *
 *****************************************************************)
module GangOfFour.Facade

open System.IO

///Sample code Helpers
let private notImpl msg = raise <| System.NotImplementedException(msg)
type NotFullyImplementedForExampleAttribute = AbstractClassAttribute

//Domain Code
module internal rec SubSystem =
    type Token = class end
    [<NotFullyImplementedForExample>]
    type Scanner(input:Stream) =
        abstract Scan: unit -> Token

    [<NotFullyImplementedForExample>]
    type ProgramNode () = 
        abstract SourcePosition: line:int * index:int with get
        //...
        abstract Add: ProgramNode -> unit
        abstract Remove: ProgramNode -> unit
        //...
        abstract Traverse:CodeGenerator-> unit
    
    [<NotFullyImplementedForExample>]
    type ExpressionNode () = 
        inherit ProgramNode ()
        let _children = ResizeArray<ProgramNode>()
        override this.Traverse(cg) =
            cg.Visit(this)
            for c in _children do
                c.Traverse(cg)
    
    [<NotFullyImplementedForExample>]
    type StatementNode () =
        inherit ProgramNode ()


    [<NotFullyImplementedForExample>]
    type ProgramNodeBuilder () =
        abstract NewVariable: variableName:string -> ProgramNode
        abstract NewAssignment: variable:ProgramNode * expression:ProgramNode -> ProgramNode
        abstract NewReturnstatement: value:ProgramNode -> ProgramNode
        abstract NewCondition: condition:ProgramNode * truePart:ProgramNode * falsePart: ProgramNode -> ProgramNode
        // ...
        abstract RootNode: ProgramNode with get

    [<NotFullyImplementedForExample>]
    type Parser() =
        abstract Parse:Scanner * ProgramNodeBuilder -> unit

    [<NotFullyImplementedForExample>]
    type CodeGenerator (output:Stream) =
        abstract Visit: ExpressionNode -> unit
        abstract Visit: StatementNode -> unit

    [<NotFullyImplementedForExample>]
    type RISCCodeGenerator (output:Stream) =   
        inherit CodeGenerator(output)

    [<NotFullyImplementedForExample>]
    type StackMachineCodeGenerator (output:Stream) =   
        inherit CodeGenerator(output)



[<AutoOpen>]
module TopLevel =
    open SubSystem
    ///Facade
    type Compiler () = 
        member _.Compile(input:Stream, output:Stream) =
            let scanner:Scanner = notImpl "Init Scanner with input"
            let builder:ProgramNodeBuilder  = notImpl "Init Program Builder"
            let parser:Parser = notImpl "Init Parser"
            
            parser.Parse(scanner, builder)

            let generator:RISCCodeGenerator = notImpl "Init Code Generator"
            let parseTree = builder.RootNode
            parseTree.Traverse(generator)
