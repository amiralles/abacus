# Abacus
Abacus is the easiest way to add scripting features to your .NET apps. Its 
lightweight interpreter allows you to define code snippets that look like 
__formulas__ and can be applied over reasonably large datasets
 without sacrificing performance. 

### What can I do with it?
Basically, you can use it whenever you need to get a value out of a run-time computed expression. Common use cases
 are conditional formatting, custom filters, allow users to write Excel alike 
formulas, and stuff like that. Places where is 
better to let the user write a bit of logic or pull that logic out of config 
files rather than stuff'em into your app's binaries. 
You can look it as a small programming language that can be embeded into your app.

### Can I integrate my app API into the scripting language?
Of course, you can. That's one the places where Abacus outperforms solutions
that use "SQL tricks" and stuff like that.
TODO: Show/Explain some use cases.

### Syntax and supported operations
TODO: Add the list of supported operators.


### A note of caution about side effects
Abacus assumes that functions calls are **side effects free**, which means, if
you called **foo** twice, within the same session and using the same arguments,
 you will get the same result no matter what foo actualy did. 
Since you can plug your own functions, there is no way to enforce that rule 
on your code, but if you are not careful in this respect, you may end up 
observing an odd behaviour in your app. 

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

### How to test
```
	//TODO:
```


### How to build
```
	//TODO:
```


### How to plug your own functions
```
	//TODO:
```


### Important
Abacus is not working yet, stay tuned!
