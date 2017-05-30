@ECHO OFF
setlocal
set EnableNuGetPackageRestore=true
if exist packages del /s /q packages
tools\NuGet\NuGet install Zafir -pre -o packages -excludeVersion -nocache
tools\NuGet\NuGet install Zafir.TypeScript -pre -o packages -excludeVersion -nocache
tools\NuGet\NuGet install Zafir.TypeScript.Lib -pre -o packages -excludeVersion -nocache
tools\NuGet\NuGet install Zafir.Knockout -pre -o packages -excludeVersion -nocache
tools\NuGet\NuGet install IntelliFactory.Build -pre -o packages -excludeVersion -nocache
tools\NuGet\NuGet install FSharp.Compiler.Tools -version 4.1.17 -o packages -excludeVersion -nocache

packages\FSharp.Compiler.Tools\tools\fsi.exe --exec tools/configure-zafir.fsx

xcopy /y /q packages\Zafir.TypeScript\tools\net40\FSharp.Core.dll tools
xcopy /y /q packages\Zafir.TypeScript\tools\net40\WebSharper.Core.dll tools
xcopy /y /q packages\Zafir.TypeScript\tools\net40\WebSharper.TypeScript.dll tools
xcopy /y /q packages\Zafir.TypeScript\tools\net40\WebSharper.Core.JavaScript.dll tools
xcopy /y /q packages\Zafir.TypeScript\tools\net40\WebSharper.Compiler.dll tools
xcopy /y /q packages\Zafir.TypeScript\tools\net40\WebSharper.JQuery.dll tools
xcopy /y /q packages\Zafir.TypeScript\tools\net40\Mono.Cecil.dll tools
xcopy /y /q packages\Zafir.TypeScript\tools\net40\Mono.Cecil.Pdb.dll tools
xcopy /y /q packages\Zafir.TypeScript\tools\net40\Mono.Cecil.Mdb.dll tools
xcopy /y /q packages\Zafir.TypeScript\tools\net40\FParsec.dll tools
xcopy /y /q packages\Zafir.TypeScript\tools\net40\FParsecCS.dll tools
xcopy /y /q packages\Zafir.Knockout\lib\net40\WebSharper.Knockout.dll tools
xcopy /y /q packages\NuGet.Core\lib\net40-client\NuGet.Core.dll tools
xcopy /y /q packages\IntelliFactory.Core\lib\net45\IntelliFactory.Core.dll tools
xcopy /y /q packages\IntelliFactory.Build\lib\net45\IntelliFactory.Build.dll tools
packages\FSharp.Compiler.Tools\tools\fsc.exe -o:tools\build-zafir.exe -r:tools\WebSharper.Core.dll -r:tools\WebSharper.Knockout.dll -r:tools\WebSharper.JQuery.dll -r:tools\WebSharper.TypeScript.dll -r:tools\NuGet.Core.dll -r:tools\IntelliFactory.Core.dll -r:tools\IntelliFactory.Build.dll tools\utility.fsx tools\build-zafir.fsx
tools\build-zafir.exe
