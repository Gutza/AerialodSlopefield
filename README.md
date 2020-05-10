# AerialodSlopefield
Slopefield data generator for Aerialod

## What you need
* This code
* [.NET Core 3.1 SDK](https://dotnet.microsoft.com/download) – make sure to download the SDK for .NET Core, not the Runtime, and neither .NET Framework
* [Aerialod](https://ephtracy.github.io/index.html?page=aerialod)

## How to use it
1. Edit file `AerialodSlopefield/RenderFunction.cs` and alter the function as you wish
1. Open a console in folder `AerialodSlopefield` and execute the following: `dotnet run -- -f test.asc`
1. Start Aerialod and open or drag file `AerialodSlopefield/test.asc` in that window
1. Experiment and enjoy

You can find out about more options by executing `dotnet run -- --help`.
