color B

del  .PublishFiles\*.*   /s /q

dotnet restore

dotnet build

cd RepSerDemo

dotnet publish -o ..\RepSerDemo\bin\Debug\netcoreapp2.2\

md ..\.PublishFiles

xcopy ..\RepSerDemo\bin\Debug\netcoreapp2.2\*.* ..\.PublishFiles\ /s /e 

echo "Successfully!!!! ^ please see the file .PublishFiles"

cmd