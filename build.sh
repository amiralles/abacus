clear;
#lib
mcs src/core/*.cs  -t:library -out:bin/Abacus.dll -r:System.Data

# tests
mcs src/test/*.cs -r:lib/Contest.Core.dll \
	 -r:lib/unbinder.dll -r:System.Data -r:bin/Abacus.dll \
	 -t:library -out:bin/Abacus.Test.dll
