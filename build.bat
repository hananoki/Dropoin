"C:\Program Files (x86)\MSBuild\12.0\bin\MSBuild.exe" /p:Configuration=Release MediaConv/MediaConv.csproj

copy bin\Release\Dropoin.exe .\build
copy bin\Release\*.dll .\build
copy bin\Release\x64 .\build
copy bin\Release\x86 .\build

pause
