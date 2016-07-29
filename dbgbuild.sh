clear;

#lib
mcs src/core/*.cs  -t:library -out:bin/Abacus.dll\
   -r:System.Data\
   -define:DEBUG

# tests
mcs src/test/*.cs -r:lib/unbinder.dll -r:lib/Contest.Core.dll\
   	-r:bin/Abacus.dll -r:System.Data\
   	-t:library -out:bin/Abacus.Test.dll\
   	-define:DEBUG
