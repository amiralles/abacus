#pragma warning disable 414, 219
namespace Abacus.Test {
	using System;
	using System.Reflection;
	using System.Linq.Expressions;
	using static Abacus.Utils;
	using static System.Linq.Expressions.Expression;
	using static Interpreter;
	using static System.Console;
	using static System.DateTime;
	using _ = System.Action<Contest.Core.Runner>;


	class FormulasFixture {

		_ basic_math = assert => {
			// add
			assert.Equal( 5d,   Eval(" 2.5 +  2.5"));
			assert.Equal(-5.5d, Eval("-2 + -3.5"));
			assert.Equal(-1d,   Eval(" 2 + -3"));
			assert.Equal(-5d,   Eval("-2 + -3"));
			assert.Equal( 1d,   Eval("-2 +  3"));
			assert.Equal( 5d,   Eval("+2 + +3"));

			// sub
			assert.Equal(-1.5d, Eval(" 2 -  3.5"));
			assert.Equal( 1d,   Eval("-2 - -3"));
			assert.Equal( 5d,   Eval(" 2 - -3"));
			assert.Equal(-5d,   Eval("-2 -  3"));
			assert.Equal(-1d,   Eval("+2 - +3"));

			// mul
			assert.Equal( 6d, Eval(" 2 *  3"));
			assert.Equal( 6d, Eval("-2 * -3"));
			assert.Equal(-6d, Eval(" 2 * -3"));
			assert.Equal(-6d, Eval("-2 *  3"));
			assert.Equal( 6d, Eval("+2 * +3"));
			assert.Equal( 0d, Eval("+2 *  0"));

			// div
			assert.Equal( 1.5,    Eval(" 3 /  2"));
			assert.Equal( 1.5,    Eval("-3 / -2"));
			assert.Equal(-1.5,    Eval(" 3 / -2"));
			assert.Equal(-1.5,    Eval("-3 /  2"));
			assert.Equal( 1.5,    Eval("+3 / +2"));
			assert.Equal(ERRDIV0, Eval("+3 / 0"));
			assert.Equal(ERRNAN,  Eval(" 0 / 0"));
			assert.Equal(0d,      Eval(" 0 / 5"));

			// mod
			assert.Equal( 1d,    Eval(" 3 %  2"));
			assert.Equal(-1d,    Eval("-3 % -2"));
			assert.Equal( 1d,    Eval(" 3 % -2"));
			assert.Equal(-1d,    Eval("-3 %  2"));
			assert.Equal( 1d,    Eval("+3 % +2"));
			assert.Equal(ERRNAN, Eval("+3 % 0"));
			assert.Equal(ERRNAN, Eval(" 0 % 0"));

			// pow
			assert.Equal( 8d,    Eval(" 2 **  3"));
			assert.Equal(-0.125, Eval("-2 ** -3"));
			assert.Equal( 0.125, Eval(" 2 ** -3"));
			assert.Equal(-8d,    Eval("-2 **  3"));
			assert.Equal( 8d,    Eval("+2 ** +3"));
			assert.Equal( 1d,    Eval("+2 **  0"));
			assert.Equal( 0d,    Eval(" 0 **  2"));
			assert.Equal( 1d,    Eval(" 0 **  0"));
			assert.Equal( 0d,    Eval(" 0 **  1"));
		};

		_ basic_comparisons = assert => {
			assert.IsTrue(Eval("1 =  1"));
			assert.IsTrue(Eval("2 <> 3"));
			assert.IsTrue(Eval("2 >  1"));
			assert.IsTrue(Eval("2 >= 1"));
			assert.IsTrue(Eval("2 >= 2"));
			assert.IsTrue(Eval("2 <= 3"));
			assert.IsTrue(Eval("3 <= 3"));
			assert.IsTrue(Eval("2 <  3"));
			assert.IsTrue(Eval("'' = ''"));

			assert.IsFalse(Eval("2 =  1"));
			assert.IsFalse(Eval("3 <> 3"));
			assert.IsFalse(Eval("1 >  1"));
			assert.IsFalse(Eval("0 >= 1"));
			assert.IsFalse(Eval("1 >= 2"));
			assert.IsFalse(Eval("4 <= 3"));
			assert.IsFalse(Eval("4 <= 3"));
			assert.IsFalse(Eval("4 <  3"));

		};

		_ basic_logic = assert => {
			// And
			assert.IsTrue(Eval("1 and 1"));
			assert.IsTrue(Eval("1=1 and 2=2"));
			assert.IsFalse(Eval("1=1 and 1=2"));
			assert.IsFalse(Eval("1=2 and 1=1"));

			// Or
			assert.IsTrue(Eval("0 or 1"));
			assert.IsTrue(Eval("1=1 or 1=2"));
			assert.IsTrue(Eval("1=2 or 1=1"));
			assert.IsTrue(Eval("1 or 0"));
			assert.IsFalse(Eval("0 or 0"));

			// Not
			assert.IsTrue(Eval("not 0"));
			assert.IsFalse(Eval("not 1"));
			assert.IsFalse(Eval("not 1=1"));
			assert.IsTrue(Eval("not 1=0"));

		};

		// // This is how we get access to local variables.
		// _ compile2_closure =  assert => {
		// 	var p1 = Parameter(typeof(string), "name");
		// 	var p2 = Parameter(typeof(object[]), "locals");
		// 	var gloc = typeof(FormulasFixture).GetMethod(
		// 				"GetLocal",
		// 				new [] {typeof(string), typeof(object[])});
        //
		// 	var call = Call(null, gloc, new [] { p1, p2 });
		// 	var blk = Block(new Expression[]{ call });
		// 	var lambda = Expression.Lambda<Func<string, object[], object>>
		// 		(blk, new [] {p1, p2});
        //
		// 	PrintLinqTree(lambda);
		// 	var fn = lambda.Compile();
        //
		// 	fn("somename", new object[]{"12312","!231231"});
		// };
        //
		// public static object GetLocal(string name, object[] locals) {
		// 	WriteLine("====================================");
		// 	WriteLine(name);
		// 	WriteLine(locals?.ToString() ?? "null");
		// 	WriteLine(locals[0]);
		// 	WriteLine("====================================");
		// 	return null;
		// }
        //
		// /// This is (int a nut shell) how the code generation works.
		// /// This code emits a funcition that takes an argunment
		// /// and prints its value to te console.
		// /// (It also prints two constants values interleaved).
		// _ compile_closure =  assert => {
		// 	var p = Parameter(typeof(object), "arg");
		// 	var wl = typeof(Console).GetMethod(
		// 				"WriteLine", 
		// 				new Type[]{typeof(object)
		// 			});
        //
		// 	var call1 = Call(null, wl, Constant("fruli"));
		// 	var call2 = Call(null, wl, p);
		// 	var call3 = Call(null, wl, Constant("fru"));
        //
		// 	var blk = Block(new Expression[]{ call1, call2, call3 });
		// 	var lambda = Expression.Lambda<Action<object>>(blk, p);
        //
		// 	PrintLinqTree(lambda);
		// 	var fn = lambda.Compile();
		// 	fn("frali");
		// };
        //
		_ equality_coercions_and_fun_with_casts = assert => {
			assert.IsTrue(Eval("0 = false"));
			assert.IsTrue(Eval("0 = ''"));
			assert.IsTrue(Eval("0 = null"));
			assert.IsTrue(Eval("0 = NaN"));

			assert.IsTrue(Eval("NaN = false"));
			assert.IsTrue(Eval("NaN = 0"));
			assert.IsTrue(Eval("NaN = ''"));
			assert.IsTrue(Eval("NaN = null"));

			assert.IsTrue(Eval("'' = NaN"));
			assert.IsTrue(Eval("'' = 0"));
			assert.IsTrue(Eval("'' = false"));
			assert.IsTrue(Eval("'' = null"));

			assert.IsTrue(Eval("false = NaN"));
			assert.IsTrue(Eval("false = ''"));
			assert.IsTrue(Eval("false = 0"));
			assert.IsTrue(Eval("false = null"));

			assert.IsTrue(Eval("null = NaN"));
			assert.IsTrue(Eval("null = false"));
			assert.IsTrue(Eval("null = 0"));
			assert.IsTrue(Eval("null = ''"));
		};


		_ sanitize_num_locals = assert => {
			var locals = new object [] {1, 2.3, 4, 3.14};
			var sanitized = SanitizeNumLocals(locals);
			for(int i = 0; i < sanitized.Length; ++i)
				assert.Equal(typeof(double), sanitized[i].GetType());

			// Contains numeric values at odd indexes 
			// and non numeric at the even ones.
			var mixedLocals = new object [] {
			   	1, false, 4.5, "fruli", 3.14, Now 
			};

			sanitized = SanitizeNumLocals(mixedLocals);
			for(int i = 0; i < sanitized.Length; ++i) {
				if (i % 2 == 0)
					assert.Equal(typeof(double), sanitized[i].GetType());
				else
					assert.NotEqual(typeof(double), sanitized[i].GetType());
			}
		};

		_ access_locals = assert => {
			var names  = new [] { "a", "b" };

			//intentional num type mixup.
			var locals = new object[] { 2, 3d };
			assert.Equal(5d, Eval("a+b", names, locals));
		};

		//TODO: Add more test to Access locals
		//TODO: Function calls
		//TODO: Op presedence
		//
	}
}
