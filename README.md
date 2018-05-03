
NiL.JS
======
    
Open source ECMAScript 6.0 (ES2015) (JavaScript) engine.
Licensed under BSD 3-Clause License.

This build is created and bundled by Jason Sobell of QuestMetrics Pty Ltd to provide a single bundled solution for the frameworks and .NET Standard.
Includes corrected Date() functionality based on .NET DateTime objects.

All credits for development of the core system go to the original author, [NiL](https://github.com/nilproject/)


```C#
 var context = new Context(); 
 context.Eval(@"console.log( 
 `This is a JavaScript engine written in C#. 
 This engine can perform js code on .NET 4,4.5,4.71 and .NET Standard 1.3+`)"); 
 ```
> [What is Isolated Global Context?](https://github.com/nilproject/NiL.JS/blob/version-2.5/Examples/7.%20Few%20words%20about%20Global%20Context/What%20it%20is.cs)  
> [Other Examples](https://github.com/nilproject/NiL.JS/tree/version-2.5/Examples)  

> [NuGet Package for .NET 4, .NET 4.5 and UWP](https://www.nuget.org/packages/NiL.JS)  
> [NuGet Package for .NET Core](https://www.nuget.org/packages/NiL.JS.NetCore/)  
