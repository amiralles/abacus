#lib
mcs src/core/*.cs  -t:library -out:bin/Abacus.dll -define:DEBUG

# tests
mcs src/test/*.cs -r:lib/unbinder.dll -r:lib/Contest.Core.dll -r:bin/Abacus.dll -t:library -out:bin/Abacus.Test.dll -define:DEBUG
