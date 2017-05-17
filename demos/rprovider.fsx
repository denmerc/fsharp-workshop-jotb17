(*
R Type Provider

R is a language by statisticians, for statisticians.

As a result, R contains a lot of useful packages for ML
tasks. The R Type Provider gives access to all of the R
ecosystem, within F#.

Note: for the TP to work, you need R installed.
*)

#I "packages/R.NET.Community/lib/net40/"
#I "packages/R.NET.Community.FSharp/lib/net40/"
#I "packages/RProvider/lib/net40/"

#r "RDotNet.dll"
#r "RDotNet.FSharp.dll"
#r "RProvider.dll"
#r "RProvider.Runtime.dll"

open RProvider
open RProvider.``base``
open RProvider.utils
open RProvider.stats
open RProvider.graphics
open RProvider.ggplot2

fsi.AddPrinter(fun (synexpr:RDotNet.SymbolicExpression) -> synexpr.Print())

let rng = System.Random()

[ for i in 1 .. 100 -> rng.NextDouble() ]
|> R.plot

#r "packages/FSharp.Data/lib/net40/FSharp.Data.dll"
open FSharp.Data

type Wines = 
    CsvProvider<
        Sample = "winequality-red.csv",
        Separators = ";",
        Schema = "float,float,float,float,float,float,float,float,float,float,float,float">

type Wine = Wines.Row

let sample = Wines.GetSample().Rows

let dataframe = 
    [
        "Alcohol", sample |> Seq.map (fun wine -> wine.Alcohol)  
        "Chlorides", sample |> Seq.map (fun wine -> wine.Chlorides)
        "Density", sample |> Seq.map (fun wine -> wine.Density)
        "Ph", sample |> Seq.map (fun wine -> wine.PH)
        "Quality", sample |> Seq.map (fun wine -> wine.Quality)
    ]
    |> namedParams 
    |> R.data_frame

R.plot dataframe

let result = R.lm("Quality ~ Alcohol + Chlorides + Density + Ph", dataframe)

R.summary(result)
