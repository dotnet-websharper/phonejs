open System
open System.IO

#load "utility.fsx"

try Directory.Delete(Utility.loc ["packages"], true) with _ -> ()
let ok =
    Utility.nuget "install Zafir -pre -o packages -excludeVersion -nocache"
    && Utility.nuget "install Zafir.TypeScript -pre -o packages -excludeVersion -nocache"
    && Utility.nuget "install Zafir.TypeScript.Lib -pre -o packages -excludeVersion -nocache"
    && Utility.nuget "install Zafir.Knockout -pre -o packages -excludeVersion -nocache"
    && Utility.nuget "install IntelliFactory.Build -pre -o packages -excludeVersion -nocache"

#I "../packages/NuGet.Core/lib/net40-client"
#r "NuGet.Core"
#r "../packages/IntelliFactory.Core/lib/net45/IntelliFactory.Core.dll"
#r "../packages/IntelliFactory.Build/lib/net45/IntelliFactory.Build.dll"
open IntelliFactory.Build

let bt = BuildTool().PackageId("Zafir.PhoneJS").VersionFrom("Zafir")
let version = PackageVersion.Full.Find(bt).ToString()
File.WriteAllText(__SOURCE_DIRECTORY__ + "/version.txt", version)

let knockoutVersion = bt.NuGetResolver.FindLatestVersion("Zafir.Knockout", true).Value.ToString()
File.WriteAllText(__SOURCE_DIRECTORY__ + "/knockout-version.txt", knockoutVersion)

let tlibVerson = bt.NuGetResolver.FindLatestVersion("Zafir.TypeScript.Lib", true).Value.ToString()
File.WriteAllText(__SOURCE_DIRECTORY__ + "/tlib-version.txt", tlibVersion)

printfn "configure: %b" ok
if not ok then exit 1 else exit 0
