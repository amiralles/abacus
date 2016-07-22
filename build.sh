#lib
mcs src/core/*.cs  -t:library -out:bin/Abacus.dll

# tests
mcs src/test/*.cs -r:lib/Contest.Core.dll -r:lib/unbinder.dll -r:bin/Abacus.dll -t:library -out:bin/Abacus.Test.dll
