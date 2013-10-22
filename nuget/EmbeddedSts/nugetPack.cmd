xcopy ..\..\build\thinktecture.identitymodel.embeddedsts*.* lib\net45 /y
NuGet.exe pack Thinktecture.IdentityModel.EmbeddedSts.nuspec -OutputDirectory ..\
