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


### A note of caution about side effects
Abacus assumes that functions calls are **side effects free**, which means, if
you called **foo** twice, within the same session and using the same argument list,
 you will get the same result no matter what foo actualy does. 
Since you can plug your own functions, there is no way for the compiler to enforce that rule 
on your code, but if you are not careful in this respect, you may end up with unexpected results.

```
// This it's OK.
int Sum(int n1, int n2) {
	return n1 + n2
}

// This it's not.
int RemoveLastAndCount() {
	var last = _items.Last();
	_items.Remove(last);
	return _items.Count;
}
```


### How to build
On linux and the mac, you can build it using the mono compiler by running any of these commands.
(If you are running on windows, replce *mcs* with *csc*, it should work rightaway).
```
# debug build
mcs src/core/*.cs  -t:library -out:bin/Abacus.dll -define:DEBUG

# release build
mcs src/core/*.cs  -t:library -out:bin/Abacus.dll
```

Or you can run ./dbgbuild.sh for the debug build or ./build.sh for the release one. 


### How to test
Abacus is tested using the contest test framework, so you don't hace to install anything, just
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
