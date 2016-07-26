# Abacus
Abacus is the easiest way to add scripting features to your .NET apps. Its 
lightweight interpreter allows you to define code snippets that look like 
__formulas__ and can be applied over reasonably large datasets
 without sacrificing performance. 

### What can I do with it?
Basically, you can use it whenever you need to get a value out of a run-time computed expression. Common use cases
 are conditional formatting, custom filters, allow users to write formulas, and stuff like that. It'll fit in any place
 where is better to let the end user write a bit of logic or pull that logic out of config 
files rather than stuff it into your app's binaries. 
You can look it as a small programming language that can be embeded into your app.

### Can I integrate my app API into the scripting language?
Of course you can. In fact, that's an aspect where Abacus outperforms solutions
that use "SQL tricks" and stuff like that.
TODO: Show/Explain some use cases.

### Syntax and supported operations
TODO: Add the list of supported operators.


### Function calls and side effects
Abacus assumes that function calls are **side effects free**, which means that if
you called a function twice, within the same session and using the same argument list, you will get the same result no matter what the function actually does. 
Since you can plug your own functions, there is no way for the compiler to enforce that rule 
on your code, but if you are not careful, you may end up with unexpected results.

```
// This it's OK. You can use functions like this without hassle.
int Sum(int n1, int n2) {
	return n1 + n2
}

// Although if your code depends on functions like this, you'll be up for surprises.
int IncCount(int num) {
	_count+=num;
	return count;
}

/* Let's look at this code:
	_count=1;
	IncCount(1); => 2
	IncCount(1); => 2!
	IncCount(1); => 2!
	At this point _count == 4, but without dropping the cache, abacus will always return 2!.
	That's is why you should always use side effects free functions. (Or pure functions if you will).
	
*/

```


### How to build
On linux and the mac, you can build it using the mono compiler by running any of these commands.

(If you are running on windows, replce *mcs* with *csc*, it should work just like that).
```
# debug build
mcs src/core/*.cs  -t:library -out:bin/Abacus.dll -define:DEBUG

# release build
mcs src/core/*.cs  -t:library -out:bin/Abacus.dll
```

Or you can run ./dbgbuild.sh for the debug build or ./build.sh for the release one. 


### How to test
Abacus is tested using the contest testing framework, so you don't hace to install anything, just
go to tools and execute ./runtests.sh. (Everything should be green).
```
cd tools
./runtests.sh
```

### How to plug your own functions
```
	//TODO:
```


### Important
Abacus is not working yet, stay tuned!
