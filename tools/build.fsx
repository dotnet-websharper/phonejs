#r "../packages/WebSharper.TypeScript/tools/net40/IntelliFactory.WebSharper.Core.dll"
#r "../packages/WebSharper/tools/net40/IntelliFactory.WebSharper.JQuery.dll"
#r "../packages/WebSharper.TypeScript/tools/net40/IntelliFactory.WebSharper.TypeScript.dll"
#r "../packages/WebSharper.Knockout/lib/net40/IntelliFactory.WebSharper.Knockout.dll"
//#r "C:/dev/websharper.typescript/build/Release/IntelliFactory.WebSharper.TypeScript.dll"
#load "utility.fsx"

open System
open System.IO
module C = IntelliFactory.WebSharper.TypeScript.Compiler
module U = Utility
type JQuery = IntelliFactory.WebSharper.JQuery.Resources.JQuery
type Knockout = IntelliFactory.WebSharper.Knockout.Resources.Knockout

let dts = U.loc ["typings/dx.phonejs.d.ts"]
let lib = U.loc ["packages/WebSharper.TypeScript.Lib/lib/net40/IntelliFactory.WebSharper.TypeScript.Lib.dll"]
//let ko  = U.loc ["packages/WebSharper.Knockout/lib/net40/IntelliFactory.WebSharper.Knockout.dll"]
let snk = U.loc [Environment.GetEnvironmentVariable("INTELLIFACTORY"); "keys/IntelliFactory.snk"]

let fsCore =
    U.loc [
        Environment.GetEnvironmentVariable("ProgramFiles(x86)")
        "Reference Assemblies/Microsoft/FSharp/.NETFramework/v4.0/4.3.0.0/FSharp.Core.dll"
    ]

type GlobalizeResource() =
    inherit IntelliFactory.WebSharper.Core.Resources.BaseResource("//cdnjs.cloudflare.com/ajax/libs/globalize/0.1.1/globalize.min.js")    

let opts =
    {
        C.Options.Create("IntelliFactory.WebSharper.PhoneJS", [dts]) with
            AssemblyVersion = Some (Version "2.5.0.0")
            Renaming = C.Renaming.RemovePrefix ""
            References = [C.ReferenceAssembly.File lib; C.ReferenceAssembly.File fsCore]
            StrongNameKeyFile = Some snk
            Verbosity = C.Level.Verbose
            WebSharperResources =
                [
                    C.WebSharperResource.Create("Globalize", "//cdnjs.cloudflare.com/ajax/libs/globalize/0.1.1/globalize.min.js")
                    C.WebSharperResource.Create("PhoneJS", "//cdn3.devexpress.com/jslib/13.2.9/js/dx.phonejs.js").Require<JQuery>().Require<Knockout>()
                    C.WebSharperResource.Create("CommonStyle", "//cdn3.devexpress.com/jslib/13.2.9/css/dx.common.css")
                    C.WebSharperResource.CreateOptional("GenericStyle", "//cdn3.devexpress.com/jslib/13.2.9/css/dx.generic.light.css")
                    C.WebSharperResource.CreateOptional("AndroidHoloDark", "//cdn3.devexpress.com/jslib/13.2.9/css/dx.android.holo-dark.css")
                    C.WebSharperResource.CreateOptional("AndroidHoloLight", "//cdn3.devexpress.com/jslib/13.2.9/css/dx.android.holo-light.css")
                    C.WebSharperResource.CreateOptional("IOSDefault", "//cdn3.devexpress.com/jslib/13.2.9/css/dx.ios.default.css")
                    C.WebSharperResource.CreateOptional("TizenWhite", "//cdn3.devexpress.com/jslib/13.2.9/css/dx.tizen.white.css")
                    C.WebSharperResource.CreateOptional("Win8Black", "//cdn3.devexpress.com/jslib/13.2.9/css/dx.win8.black.css")
                    C.WebSharperResource.CreateOptional("Win8White", "//cdn3.devexpress.com/jslib/13.2.9/css/dx.win8.white.css")
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
