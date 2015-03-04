#load "Common.csx"

Execute(() => PatchAssemblyVersionFromNugetSpecification(@"package\DbReader.nuspec", @"..\..\DbReader\DbReader\Properties\AssemblyInfo.cs"), "Patching AssemblyVersion");

Execute(() => DeleteAllPackages("."), "Delete old package");;

Execute(() => Build(@"..\..\DbReader\DbReader.sln"), "Building DbReader");

Execute(() => CopyFiles(@"..\..\DbReader\DbReader\bin\release\", @"package\lib\net45\", "*.*"), "Copying release build to NuGet lib folder");

Execute(() => CreatePackage(@"package\DbReader.nuspec", @".."), "Create Nuget Package");;