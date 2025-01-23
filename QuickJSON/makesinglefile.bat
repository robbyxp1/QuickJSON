Rem this requires external tools!
echo Making main quickjson
eddtest mergecsharp quickjson utils >QuickJson.cs
echo Making fluent
eddtest mergecsharp fluent >QuickJsonFluent.cs
echo Done!

