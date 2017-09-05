open System
open System.IO

#I "../packages/NuGet.Core/lib/net40-client"
#r "NuGet.Core"
#r "../packages/IntelliFactory.Core/lib/net45/IntelliFactory.Core.dll"
#r "../packages/IntelliFactory.Build/lib/net45/IntelliFactory.Build.dll"
open IntelliFactory.Build

let bt = BuildTool().PackageId("WebSharper.PhoneJS").VersionFrom("WebSharper")
let version = PackageVersion.Full.Find(bt).ToString()
File.WriteAllText(__SOURCE_DIRECTORY__ + "/version.txt", version)

let knockoutVersion = bt.NuGetResolver.FindLatestVersion("WebSharper.Knockout", true).Value.ToString()
File.WriteAllText(__SOURCE_DIRECTORY__ + "/knockout-version.txt", knockoutVersion)

let tlibVersion = bt.NuGetResolver.FindLatestVersion("WebSharper.TypeScript.Lib", true).Value.ToString()
File.WriteAllText(__SOURCE_DIRECTORY__ + "/tlib-version.txt", tlibVersion)
