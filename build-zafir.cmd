@ECHO OFF
setlocal
set PATH=%PATH%;%ProgramFiles(x86)%\Microsoft SDKs\F#\3.1\Framework\v4.0
set PATH=%PATH%;%ProgramFiles%\Microsoft SDKs\F#\3.1\Framework\v4.0
fsi.exe --exec tools/configure-zafir.fsx
fsi.exe --exec tools/build-zafir.fsx
