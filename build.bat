echo off
cls

rem lib
csc .\src\core\*.cs /t:library /out:.\bin\abacus.dll /r:System.Data.dll ^
	-debug -pdb:.\bin\abacus


rem tests
csc .\src\test\*.cs /r:.\lib\Contest.Core.dll ^
	 /r:.\lib\unbinder.dll /r:System.Data.dll /r:.\bin\abacus.dll ^
	 /t:library /out:.\bin\abacus.test.dll

rem app
csc .\src\app\*.cs  /t:exe /out:.\bin\abacus.exe ^
	 /r:.\bin\abacus.dll

