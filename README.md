# AerialodSlopefield
Slopefield data generator for Aerialod

## What is this?
See the main [AerialodSlopefield website](https://gutza.github.io/AerialodSlopefieldWebsite/).

## What you need
* [This code](https://github.com/Gutza/AerialodSlopefield/releases);
* [.NET Core 3.1 SDK](https://docs.microsoft.com/en-us/dotnet/core/install/sdk) – make sure to download the SDK for .NET Core, not the Runtime, and neither .NET Framework;
* [Aerialod](https://ephtracy.github.io/index.html?page=aerialod)

## How to use it
1. Open a console in folder `AerialodSlopefield` and execute the following: `dotnet run -- -f test.asc`
1. Start Aerialod and open or drag file `AerialodSlopefield/test.asc` in that window
1. Experiment and enjoy
1. Edit file `AerialodSlopefield/RenderFunction.cs`, alter the function as you wish, and start over from the top
1. Execute `dotnet run -- --help` to learn about the various options, and start over from the top

