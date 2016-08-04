# Abacus
Abacus is the easiest way to add scripting features to your .NET apps. Its 
lightweight interpreter allows you to define code snippets that look like 
Excel __formulas__ and can be applied over reasonably large datasets
 without sacrificing performance. 

### What can I do with it?
Basically, you can use it whenever you need to get a value out of a run-time computed expression. Common use cases
 are conditional formatting, custom filters, allow users to write formulas, and stuff like that. It'll fit in any place
 where is better to let the end user write a bit of logic or pull that logic out of config 
files rather than stuff it into your app's binaries. 
You can look it as a small programming language that can be embeded into your app.

### Can I integrate my app API into the scripting language?
Of course you can. In fact, that's an aspect where Abacus outperforms solutions
that use "SQL tricks", databinding and stuff like that.

### Syntax and supported operators
---------------------------------------------
| Category        | Operators               |
|-----------------|-------------------------|
| Primary         | foo.bar foo(bar) foo[x] |
| Unary           | - + not                 |
| Multiplicative  | * / %                   |
| Additive        | + -                     |
| Relational      | > < <= >=               |
| Equality        | = <>                    |
| Conditional     | and or                  | 
| Membership      | in                      |
---------------------------------------------

### Function calls and side effects
Abacus assumes that function calls are **side effects free**, which means that if
you called a function twice, within the same session and using the same argument list, you will get the same result no matter what the function actually does. 
Since you can plug your own functions, there is no way for the compiler to enforce that rule 
on your code, but if you are not careful, you may end up with unexpected results. Let's look at some code:

``` cpp
// Suppose you have some useful functions in this class and want to expose them to abacus. That's 
// completely possible but there are some considerations.

class YourAwesomeLibrary {
	// This it's OK. You can use functions like this one without any hassle.
	public int Sum(int n1, int n2) {
		return n1 + n2
	}
	
	// This is NOT OK, and if your code depends on functions like this, you'll be up for surprises.
	public int IncCount(int num) {
		_count += num;
		return _count;
	}
}
/* Let's see what happens when abacus executes the code:
	_count = 1;
	IncCount(1); => 2
	IncCount(1); => 2!
	IncCount(1); => 2!
	At this point _count == 4, but without dropping the cache, abacus will always return 2!.
	That's is why you should always use pure functions.
	
	(Yes, you can drop the cache on each invocation and get away with state mutations, 
	but performance wise, it'll be a complete disaster).
*/

```

### How to build
On linux and the mac, you can build it using the mono compiler by running any of these commands.

(If you are running on windows, replce *mcs* with *csc*, it should work right off the bat).
```
# debug build
mcs src/core/*.cs  -t:library -out:bin/Abacus.dll -define:DEBUG

# release build
mcs src/core/*.cs  -t:library -out:bin/Abacus.dll
```

Or you can run **./dbgbuild.sh** for the debug build or **./build.sh** for the release one. 


### How to test
Abacus is tested using the contest testing framework, so you don't have to install anything, just
go to tools and execute **./runtests.sh**. (Everything should be green).
```
cd tools
./runtests.sh
```

### How to use it to add eval capabilities
Of course you can use it to perform evals on strings and stuff like that. i.e:
```
"2+3".Eval(); //=> 5
```
But often times that will be a performance killer.
```
//TODO:
// show how to use variables.
// show how to call functions.
// show how to use the "onError" callback.
// performance considerations, session, cache, et. al.

```

### How to use it to apply conditional formatting
```
//TODO:
```

### How to use it to filter data
Let's suppose we have a table and we want to filter some records based
on a run-time compiled expression. (Maybe entered by an end user thru a
wizard or something).

``` cpp
// This is the sample data we'll use.
// ------------------------------------------------
// |                    Sales                     |
// ------------------------------------------------
// | Amount |    Date     | Customer  | SalesRep  |
// ------------------------------------------------
// |    123 | 14/05/2012  | amiralles |   foo     |
// |    222 | 14/05/2012  | amiralles |   bar     |
// |    456 | 21/02/2012  | pipex     |   foo     |
// |    789 | 21/02/2012  | pipex     |   baz     |
// |    123 | 21/02/2012  | vilmis    |   baz     |
// ------------------------------------------------

// This will give us all amiralles's purchases.
tbl.Reduce("Customer='amiralles'");

// This will give us all amiralles's purchases from 200 bucks and up.
tbl.Reduce("Customer='amiralles' and Amount >= 200");

// This will give us all pipex's and vilmis's purchases combined.
tbl.Reduce("Customer='pipex' or Customer='vilmis'");

// So far so good, now let's use some variables. From here, abacus is somewhat
// special compared to sql expressions becasue it gives you more flexibility.
// Let's suppose we want to show to the current user (a sales rep) all sales
// that belongs to him/her. We can inject our own variables into the
// interpreter's run-time context and use them as if they were standard fields.

// In a real system you'll probably grab some values from session 
// objects and sutff like that.

var locNames  = new string[] { "curr_usr" };
var locValues = new object[] { "foo" };

tbl.Reduce("SalesRep = curr_usr", locNames, locValues);

// And now, the really cool part. Hooking you own API.
// TODO: Show an example that makes sence...

```

### How to plug your own functions
```
	//TODO:
```


### Important
Abacus is not working yet, stay tuned!
