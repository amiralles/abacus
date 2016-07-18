
namespace Abacus {
	using System;
	using System.Linq.Expressions;

	public class Interpreter {

		// TODO: Add session (for caching purposes).
		public static object Eval(string src, string[] locals = null) {
			//TODO: If same src and same locals, we don't have to rebuild, 
			//      we can use a cached version of the compiled funcion.

			//TODO: If same session, same locals, same src, we don't
			//      have to do anything, just return the previous result.

			var fn = Compile(src, locals);
			return fn(locals);
		}

		static Func<object[], object> Compile(string src, string[]locals) {
			var tokenizer = new Tokenizer(src);
			var parser  = new Parser(tokenizer, locals);
			var tree    = parser.Parse();
			var walker  = new SyntaxWalker();
			var program = walker.Walk(tree);
			var argv    = GetArgv(locals);
			var lambda  = Expression.Lambda<Func<object[], object>>(program, argv);
			return lambda.Compile();
		}

		static ParameterExpression[] GetArgv(string[] locals) {
			ParameterExpression[] argv;
			if (locals != null) {
				argv = new ParameterExpression[locals.Length];
				for(int i=0; i < locals.Length; ++i)
					argv[i] = Expression.Parameter(typeof(object), locals[i]);
			}
			else {
				argv = new ParameterExpression[0];
			}
			return argv;
		}

		/// This method is responsable for dispaching any function call
		/// than happens inside the script.
		public static object DispatchCall(
				object target, string lowerFuncName, object[] argv) {

			return null;
		}
	}
}
