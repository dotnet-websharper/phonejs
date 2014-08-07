#r "../packages/WebSharper.TypeScript/tools/net40/IntelliFactory.WebSharper.Core.dll"
#r "../packages/WebSharper/tools/net40/IntelliFactory.WebSharper.JQuery.dll"
#r "../packages/WebSharper.TypeScript/tools/net40/IntelliFactory.WebSharper.TypeScript.dll"
//#r "C:/dev/websharper.typescript/build/Release/IntelliFactory.WebSharper.TypeScript.dll"
#load "utility.fsx"

open System
open System.IO
module C = IntelliFactory.WebSharper.TypeScript.Compiler
module U = Utility
type JQuery = IntelliFactory.WebSharper.JQuery.Resources.JQuery

let dts = U.loc ["typings/dx.phonejs.d.ts"]
let lib = U.loc ["packages/WebSharper.TypeScript.Lib/lib/net40/IntelliFactory.WebSharper.TypeScript.Lib.dll"]
let snk = U.loc [Environment.GetEnvironmentVariable("INTELLIFACTORY"); "keys/IntelliFactory.snk"]

let fsCore =
    U.loc [
        Environment.GetEnvironmentVariable("ProgramFiles(x86)")
        "Reference Assemblies/Microsoft/FSharp/.NETFramework/v4.0/4.3.0.0/FSharp.Core.dll"
    ]

let opts =
    {
        C.Options.Create("IntelliFactory.WebSharper.PhoneJS", [dts]) with
            AssemblyVersion = Some (Version "2.5.0.0")
//            Renaming = C.Renaming.RemovePrefix ""
            References = [C.ReferenceAssembly.File lib; C.ReferenceAssembly.File fsCore]
            StrongNameKeyFile = Some snk
            Verbosity = C.Level.Verbose
            EmbeddedResources =
                [
                    C.EmbeddedResource.FromFile("js/globalize.js")
                    C.EmbeddedResource.FromFile("js/dx.phonejs.js")
                    C.EmbeddedResource.FromFile("css/dx.android.holo-dark.css")
                    C.EmbeddedResource.FromFile("css/dx.android.holo-light.css")
                    C.EmbeddedResource.FromFile("css/dx.common.css")
                    C.EmbeddedResource.FromFile("css/dx.generic.light.css")
                    C.EmbeddedResource.FromFile("css/dx.ios.default.css")
                    C.EmbeddedResource.FromFile("css/dx.tizen.white.css")
                    C.EmbeddedResource.FromFile("css/dx.win8.black.css")
                    C.EmbeddedResource.FromFile("css/dx.win8.white.css")
                 ]
            WebSharperResources =
                [
                    C.WebSharperResource.Create("Globalize", "globalize.js")
                    C.WebSharperResource.Create("PhoneJS", "dx.phonejs.js").Require<JQuery>()
                    C.WebSharperResource.Create("CommonStyle", "dx.common.css")
                    C.WebSharperResource.CreateOptional("AndroidHoloDark", "dx.android.holo-dark.css")
                    C.WebSharperResource.CreateOptional("AndroidHoloLight", "dx.android.holo-light.css")
                    C.WebSharperResource.CreateOptional("GenericStyle", "dx.generic.light.css")
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

match result.CompiledAssembly with
| None -> ()
| Some asm ->
    let out = U.loc ["build/IntelliFactory.WebSharper.PhoneJS.dll"]
    let dir = DirectoryInfo(Path.GetDirectoryName(out))
    if not dir.Exists then
        dir.Create()
    printfn "Writing %s" out
    File.WriteAllBytes(out, asm.GetBytes())

let (|I|_|) (x: string) =
    match x with
    | null | "" -> None
    | n ->
        match Int32.TryParse(n) with
        | true, r -> Some r
        | _ -> None

let version =
    match Environment.GetEnvironmentVariable("BUILD_NUMBER") with
    | I k -> Version(2, 5, k, 0).ToString()
    | _ -> Version(2, 5, 0, 0).ToString()

let ok =
    match Environment.GetEnvironmentVariable("NuGetPackageOutputPath") with
    | null | "" ->
        U.nuget (sprintf "pack -out build/ -version %s PhoneJS.nuspec" version)
    | path ->
        U.nuget (sprintf "pack -out %s -version %s PhoneJS.nuspec" path version)

printfn "pack: %b" ok
if not ok then exit 1 else exit 0