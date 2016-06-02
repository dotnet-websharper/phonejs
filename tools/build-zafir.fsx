#r "../packages/Zafir.TypeScript/tools/net40/WebSharper.Core.dll"
#r "../packages/Zafir/lib/net40/WebSharper.JQuery.dll"
#r "../packages/Zafir.TypeScript/tools/net40/WebSharper.TypeScript.dll"
#r "../packages/Zafir.Knockout/lib/net40/Zafir.Knockout.dll"
//#r "C:/dev/websharper.typescript/build/Release/WebSharper.TypeScript.dll"
#I "../packages/NuGet.Core/lib/net40-client"
#r "NuGet.Core"
#r "../packages/IntelliFactory.Core/lib/net45/IntelliFactory.Core.dll"
#r "../packages/IntelliFactory.Build/lib/net45/IntelliFactory.Build.dll"
#load "utility.fsx"

open System
open System.IO
module C = WebSharper.TypeScript.Compiler
module U = Utility
type JQuery = WebSharper.JQuery.Resources.JQuery
type Knockout = Zafir.Knockout.Resources.Knockout

open IntelliFactory.Build

let bt =
    BuildTool().PackageId("Zafir.PhoneJS").VersionFrom("Zafir")
        .WithFramework(fun fw -> fw.Net40)
        .WithFSharpVersion(FSharpVersion.FSharp30)

let asmVersion =
    let v = PackageVersion.Full.Find(bt)
    sprintf "%i.%i.0.0" v.Major v.Minor

let dts = U.loc ["typings/dx.phonejs.d.ts"]
let lib = U.loc ["packages/Zafir.TypeScript.Lib/lib/net40/WebSharper.TypeScript.Lib.dll"]
let snk = U.loc [Environment.GetEnvironmentVariable("INTELLIFACTORY"); "keys/IntelliFactory.snk"]

let fsCore =
    U.loc [
        Environment.GetEnvironmentVariable("ProgramFiles(x86)")
        "Reference Assemblies/Microsoft/FSharp/.NETFramework/v4.0/4.3.0.0/FSharp.Core.dll"
    ]

type GlobalizeResource() =
    inherit WebSharper.Core.Resources.BaseResource("//cdnjs.cloudflare.com/ajax/libs/globalize/0.1.1/globalize.min.js")    

let opts =
    {
        C.Options.Create("Zafir.PhoneJS", [dts]) with
            AssemblyVersion = Some (Version asmVersion)
            Renaming = C.Renaming.RemovePrefix ""
            References = [C.ReferenceAssembly.File lib; C.ReferenceAssembly.File fsCore]
            StrongNameKeyFile = Some snk
            Verbosity = C.Level.Verbose
            EmbeddedResources =
                [
                    C.EmbeddedResource.FromFile("js/globalize.js")
                    C.EmbeddedResource.FromFile("js/dx.phonejs.js")
                    C.EmbeddedResource.FromFile("css/dx.common.css")
                    C.EmbeddedResource.FromFile("css/dx.generic.light.css")
                    C.EmbeddedResource.FromFile("css/dx.android.holo-dark.css")
                    C.EmbeddedResource.FromFile("css/dx.android.holo-light.css")
                    C.EmbeddedResource.FromFile("css/dx.ios.default.css")
                    C.EmbeddedResource.FromFile("css/dx.tizen.white.css")
                    C.EmbeddedResource.FromFile("css/dx.win8.black.css")
                    C.EmbeddedResource.FromFile("css/dx.win8.white.css")
                ]
            WebSharperResources =
                [
                    C.WebSharperResource.Create("Globalize", "globalize.js")
                    C.WebSharperResource.Create("PhoneJS", "dx.phonejs.js").Require<JQuery>().Require<Knockout>()
                    C.WebSharperResource.Create("CommonStyle", "dx.common.css")
                    C.WebSharperResource.CreateOptional("GenericStyle", "dx.generic.light.css")
                    C.WebSharperResource.CreateOptional("AndroidHoloDark", "dx.android.holo-dark.css")
                    C.WebSharperResource.CreateOptional("AndroidHoloLight", "dx.android.holo-light.css")
                    C.WebSharperResource.CreateOptional("IOSDefault", "dx.ios.default.css")
                    C.WebSharperResource.CreateOptional("TizenWhite", "dx.tizen.white.css")
                    C.WebSharperResource.CreateOptional("Win8Black", "dx.win8.black.css")
                    C.WebSharperResource.CreateOptional("Win8White", "dx.win8.white.css")
                ]
    }

let result =
    C.Compile opts

for msg in result.Messages do
    printfn "%O" msg

let knockoutVersion = bt.NuGetResolver.FindLatestVersion("Zafir.Knockout", true).Value.Version.ToString()
let tlibVerson = bt.NuGetResolver.FindLatestVersion("Zafir.TypeScript.Lib", true).Value.Version.ToString()

match result.CompiledAssembly with
| None -> ()
| Some asm ->
    let out = U.loc ["build/WebSharper.PhoneJS.dll"]
    let dir = DirectoryInfo(Path.GetDirectoryName(out))
    if not dir.Exists then
        dir.Create()
    printfn "Writing %s" out
    File.WriteAllBytes(out, asm.GetBytes())

    bt.Solution [
        bt.NuGet.CreatePackage()
            .Configure(fun c ->
                { c with
                    Authors = ["IntelliFactory"]
                    Title = Some "Zafir.PhoneJS 13.2.9"
                    LicenseUrl = Some "http://websharper.com/licensing"
                    ProjectUrl = Some "http://websharper.com"
                    Description = "Zafir bindings for PhoneJS (13.2.9)"
                    RequiresLicenseAcceptance = true })
            .AddDependency("Zafir.TypeScript.Lib", tlibVerson, forceFoundVersion = true)
            .AddDependency("Zafir.Knockout", knockoutVersion, forceFoundVersion = true)
            .AddFile("build/WebSharper.PhoneJS.dll", "lib/net40/WebSharper.PhoneJS.dll")
            .AddFile("README.md", "docs/README.md")
    ]
    |> bt.Dispatch
