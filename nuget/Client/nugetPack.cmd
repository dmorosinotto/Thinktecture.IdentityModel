xcopy ..\..\build\thinktecture.identitymodel.client*.* lib\portable-net45+wp80+win8 /y
NuGet.exe pack Thinktecture.IdentityModel.Client.nuspec -OutputDirectory ..\