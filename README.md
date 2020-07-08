# OOP Patterns in FSharp
 Examples of OOP Patterns implmenented in F#

## Reasoning

So, the purpose of this repo is not to advocate the use of F# as a primarily Object Oriented langauge, because the benefits of Functional Program are just too mumerous. However, if you need/wanted to do serious Object Oriented Programing, it is not only possible with F#, but thanks to it's type inference, lack of nulls, immutable by default, and simple declariations, the result is some very clear and consise object oriented code (quite possibly more so than your OO first langauge of choice – IMO better than C#).

This view point is not common, rather more typically I've seen is represented by this example by from [HackerNews](https://news.ycombinator.com/item?id=23505333):

> To get all the benefits of F#, you must adopt the whole paradigm. While F# allows C#-like OOP, and while this can be an initial stepping stone in the path towards F#, you must go all the way. If you simply do OOP, the trade-offs aren't worth it, IMO, as F# is a functional-first language, and the OOP is mostly provided for interop with the rest of .NET.

That has not been anywhere near my experience, such that my feeling is this view point may be more about C# interop than OOP. But to be fair, I definately favor functional programing, and typically only do Object Oriented Style in F# to interact with frameworks patterns. So to better test this hypotheses, I thought I'd explore porting all the sample code in old standard Gang of Four Design Patterns book – [Design Patterns: Elements of Reusable Object-Oriented Software by Erich Gamma, Richard Helm, Ralph Johnson, John Vlissides](https://books.google.com/books/about/Design_Patterns.html?id=6oHuKQe3TjQC). 

## Methodology/Sections/Progress

### Gang Of Four Patterns

*I used F# default features for code correctness in full force and didn't concern with adding extra code for C# easy consumption. I used option types instead of allowing null (unless mimicing a 3rd party framework, not the pattern), I made the bare minimum of fields mutable, I used functional style inside methods while externally presenting OOP*

| Creation | Structural | Behavioral |
|-|-|-|
| :heavy_check_mark: [Abstract Factory](GangOfFour/Creational-Patterns/AbstractFactory.fs) | :heavy_check_mark: [Adapter](GangOfFour/Structural-Patterns/Adapter.fs) | :heavy_check_mark: [Chain of Responsibility](GangOfFour/Behavioral-Patterns/ChainOfResponsibility.fs) |
| :heavy_check_mark: [Builder](GangOfFour/Structural-Patterns/Builder.fs) | :heavy_check_mark: [Bridge](GangOfFour/Structural-Patterns/Bridge.fs) | :heavy_check_mark: [Command](GangOfFour/Behavioral-Patterns/Command.fs) |
| :heavy_check_mark: [Factory Method](GangOfFour/Structural-Patterns/FactoryMethod.fs) | :heavy_check_mark: [Composite](GangOfFour/Structural-Patterns/Composite.fs) | :heavy_check_mark: [Interpreter](GangOfFour/Behavioral-Patterns/Interpreter.fs) |
| :heavy_check_mark: [Prototype](GangOfFour/Structural-Patterns/Prototype.fs) | :heavy_check_mark: [Decorator](GangOfFour/Structural-Patterns/Decorator.fs) | :heavy_check_mark: [Iterator](GangOfFour/Behavioral-Patterns/Iterator.fs) |
| :heavy_check_mark: [Singleton](GangOfFour/Structural-Patterns/Singleton.fs) | :heavy_check_mark: [Facade](GangOfFour/Structural-Patterns/Facade.fs) | :heavy_check_mark: [Mediator](GangOfFour/Behavioral-Patterns/Mediator.fs) |
|  | :heavy_check_mark: [Flyweight](GangOfFour/Structural-Patterns/Flyweight.fs) | :construction: Memento |
|  | :heavy_check_mark: [Proxy](GangOfFour/Structural-Patterns/Proxy.fs) | :construction: Observer |
|  |  | :construction: State |
|  |  | :construction: Strategy |
|  |  | :heavy_check_mark: [Template Method](GangOfFour/Behavioral-Patterns/TemplateMethod.fs) |
|  |  | :heavy_check_mark:  [Vistor](GangOfFour/Behavioral-Patterns/Vistor.fs) |

### [MVC Entity Framework Sample Code](ef-mvc) [[C# Tutorial](https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/?view=aspnetcore-3.1)]

|             | Chapters       |             |
|-------------|----------------|-------------|
| :heavy_check_mark: Get Started | Complex Data   | Inheritance |
| :heavy_check_mark: CRUD        | Related Data   | Advanced    |
| Sort Filter | Update Related |             |
| Migrations  | Concurency     |             |

## Notes & Caveats

### Gang Of Four Patterns

While I didn't run into any OOP issues implementing these patterns so far, I'm noting the following:

  * Operator overloading sucks in F#, it seems like it should be nice with statically resolved types, but it ended up being a headache. Operator overloading is not necessary for OOP, lot of people think it's an antipattern, but it was used in some samples, so when I used it I kept it simple as possible. Operator overloading isn't great for C# either BTW.
  * No protected methods. This is not a deal breaker for OOP programming, or the Gang of Four sample code, but as a side note, it is a kind of weird hill to die on for F# ([mentioned in the history of F#](https://dl.acm.org/doi/pdf/10.1145/3386325)), because now interop with C# frameworks that do metaprograming reflect/emit requiring writing a base class with protected virtual members (unfortunately can be a thing) is quite literally impossible. Not a big deal design wise, the F# ideology of forcing a public/internal choice, is probably better in fairness.
  * I used a nightly build of [FSharp.Interop.NullOptAble](https://github.com/ekonbenefits/FSharp.Interop.NullOptAble) in the Mediator pattern to keep the domain types from allowing nullness (the. F# default), while keeping the code as simple since the original sample code had a lot of unguarded not null assumptions.
  
### MVC Entity Framework Sample Code

Ran into more issues with the sample code at the start due to C# interoperablity

  * DbContext, I'm not sure if there is something that was keeping `DbSet` from being injected or if my first attempt at using `member val` prevented was initializtion was overwriting. Either way my 2nd attempt was to initialize with `this.Set<Model>()` which caused a recursive initialization error. So I ended up using lazy backing fields for the properties and that worked just fine.
  * Auto generated Linq expressions from param arrays don't work in F#. I had to make my own identity method that would allow the compiler to auto generate one expression and then use it on each element of an array literal. Using new F# open static type methods, made it look cleaner than it sounds.
  * Entity Framework overloads of Include and ThenInclude will need some type hints. `Include` lamba's results will need to be upcasted to IEnumerable<> to get the right overload, and then you have to add a type hint to the argument of the subsequent `ThenInclude`.
  * All the entities are allownull, but in usage, I wasn't sure if I wanted to null check or transform to option types. I did the later, and found an unhandled NRE based on user input thanks to type checking.

  
## Extensions

  * I'd like to port some more sample code from other source material more representive of C# specific patterns, so that I can have some examples that focus more on C# interop. I do a lot of C# interop in my day job, to the point that I am the author of 3 out of 6 **FSharp.Interop.*** libraries on Github, and feel like there is a lot of improvement that can be made for interop.
  
## Todo

  * Finish Gang of Four Patterns
  * Convert to dotnet interactive notebooks when stable
  * Find some C# specifc patterns to port.
  
## Contributing

  * I'm happy to get critiques, suggestions, discussions, additional examples, etc in issues and pull requests.
