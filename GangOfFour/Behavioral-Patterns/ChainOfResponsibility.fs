(***************************************************************** 
 * Chain of Responsability (page 223)
 *   
 * Design Patterns: Elements of Reusable Object-Oriented Software
 *   by Gamma, Helm, Johnson, & Vlissides
 * Sample Code Ported to F#
 *
 * - Initally Ported by Jay Tuley (Jun 2020)
 *
 *****************************************************************)
namespace GangOfFour.ChainOfResponsibility

type Topic = NoHelp | TopicCategory of int

[<AbstractClass>]
type HelpHandler(?optSuccessor:HelpHandler, ?optTopic: Topic) =
    let mutable topic = defaultArg optTopic NoHelp
    let mutable successor = optSuccessor
    member _.HasHelp with get () = match topic with NoHelp -> false | _ -> true
    member _.SetHandler(s, t) =
        successor <- Some s
        topic <- t
    abstract HandleHelp: unit -> unit
    default _.HandleHelp () = successor |> Option.iter (fun s -> s.HandleHelp())

[<AbstractClass>]
type Widget =
    inherit HelpHandler
    new (?optTopic: Topic) = 
        { inherit HelpHandler(optTopic=defaultArg optTopic NoHelp) }
    new (parent:Widget, ?optTopic: Topic) =
        { inherit HelpHandler(parent, defaultArg optTopic NoHelp) }

type Button (d:Widget, ?optTopic: Topic) =
    inherit Widget(d, defaultArg optTopic NoHelp)
    override this.HandleHelp () =
        if this.HasHelp then
            () //offer help on the button here
        else
            base.HandleHelp()

type Dialog (h: HelpHandler, ?optTopic: Topic) as this=
    inherit Widget(defaultArg optTopic NoHelp)
    do 
        this.SetHandler(h, defaultArg optTopic NoHelp)
    override this.HandleHelp () =
        if this.HasHelp then
            () //offer help on the dialog here
        else
            base.HandleHelp()

type Application(topic:Topic) =
    inherit HelpHandler(optTopic=topic)
    override this.HandleHelp() =
        () //offer list of help topics

//Example Code
module Example =
    let main _ =
        let printTopic = TopicCategory(1)
        let paperOrientationTopic = TopicCategory(2)
        let appTopic = TopicCategory(3)

        let app = Application(appTopic)
        let dialog = Dialog(app, appTopic)
        let button = Button(dialog, paperOrientationTopic)

        button.HandleHelp()
