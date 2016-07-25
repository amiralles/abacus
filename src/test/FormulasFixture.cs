#pragma warning disable 414, 219
namespace Abacus.Test {
	using System;
	using System.Reflection;
	using System.Linq.Expressions;
	using static Abacus.Utils;
	using static Abacus.Error;
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

		_ basic_math_with_locals = assert => {

			var names  = new [] { 
				"pos25", "neg2", "neg35", 
				"pos2",  "neg3", "pos3", 
				"zero",  "pos1", "pos5",
				"pos35"
			};

			var locals = new object[] { 
				2.5, -2, -3.5,
				2,   -3,  3,
				0,    1,  5,
				3.5
			};

			var sess = new Session(123);
			Func<string, object> locEval = src =>
					Eval(src, names, locals, ref sess);

			// add
			assert.Equal( 5d,   locEval("pos25 + pos25"));
			assert.Equal(-5.5d, locEval("neg2  + neg35"));
			assert.Equal(-1d,   locEval("pos2  + neg3"));
			assert.Equal(-5d,   locEval("neg2  + neg3"));
			assert.Equal( 1d,   locEval("neg2  + pos3"));
			assert.Equal( 5d,   locEval("pos2  + pos3"));

			// sub
			// assert.Equal(-1.5d, locEval("pos2 - pos35"));
			assert.Equal( 1d,   locEval("neg2 - neg3"));
			assert.Equal( 5d,   locEval("pos2 - neg3"));
			assert.Equal(-5d,   locEval("neg2 - pos3"));
			assert.Equal(-1d,   locEval("pos2 - pos3"));

			// mul
			assert.Equal( 6d, locEval("pos2 * pos3"));
			assert.Equal( 6d, locEval("neg2 * neg3"));
			assert.Equal(-6d, locEval("pos2 * neg3"));
			assert.Equal(-6d, locEval("neg2 * pos3"));
			assert.Equal( 6d, locEval("pos2 * pos3"));
			assert.Equal( 0d, locEval("pos2 * zero"));

			// div
			assert.Equal( 1.5,    locEval("pos3 / pos2"));
			assert.Equal( 1.5,    locEval("neg3 / neg2"));
			assert.Equal(-1.5,    locEval("pos3 / neg2"));
			assert.Equal(-1.5,    locEval("neg3 / pos2"));
			assert.Equal( 1.5,    locEval("pos3 / pos2"));
			assert.Equal(ERRDIV0, locEval("pos3 / zero"));
			assert.Equal(ERRNAN,  locEval("zero / zero"));
			assert.Equal(0d,      locEval("zero / pos5"));


			// mod
			assert.Equal( 1d,    locEval("pos3 % pos2"));
			assert.Equal(-1d,    locEval("neg3 % neg2"));
			assert.Equal( 1d,    locEval("pos3 % neg2"));
			assert.Equal(-1d,    locEval("neg3 % pos2"));
			assert.Equal( 1d,    locEval("pos3 % pos2"));
			assert.Equal(ERRNAN, locEval("pos3 % zero"));
			assert.Equal(ERRNAN, locEval("zero % zero"));

			// pow
			assert.Equal( 8d,    locEval("pos2 ** pos3"));
			assert.Equal(-0.125, locEval("neg2 ** neg3"));
			assert.Equal( 0.125, locEval("pos2 ** neg3"));
			assert.Equal(-8d,    locEval("neg2 ** pos3"));
			assert.Equal( 8d,    locEval("pos2 ** pos3"));
			assert.Equal( 1d,    locEval("pos2 ** zero"));
			assert.Equal( 0d,    locEval("zero ** pos2"));
			assert.Equal( 1d,    locEval("zero ** zero"));
			assert.Equal( 0d,    locEval("zero ** pos1"));
		};

		_ basic_comparisons_with_locals = assert => {

			var names  = new [] { 
				"pos1", "pos2", "pos3", "pos4", "zero",	"stre" 
			};

			var locals = new object[] { 
				1, 2, 3, 4, 0, "" 
			};

			var sess = new Session(456);
			Func<string, object> locEval = src =>
					Eval(src, names, locals, ref sess);

			assert.IsTrue(locEval("pos1 =  pos1"));
			assert.IsTrue(locEval("pos2 <> pos3"));
			assert.IsTrue(locEval("pos2 >  pos1"));
			assert.IsTrue(locEval("pos2 >= pos1"));
			assert.IsTrue(locEval("pos2 >= pos2"));
			assert.IsTrue(locEval("pos2 <= pos3"));
			assert.IsTrue(locEval("pos3 <= pos3"));
			assert.IsTrue(locEval("pos2 <  pos3"));
			assert.IsTrue(locEval("stre = stre"));

			assert.IsFalse(locEval("pos2 =  pos1"));
			assert.IsFalse(locEval("pos3 <> pos3"));
			assert.IsFalse(locEval("pos1 >  pos1"));
			assert.IsFalse(locEval("zero >= pos1"));
			assert.IsFalse(locEval("pos1 >= pos2"));
			assert.IsFalse(locEval("pos4 <= pos3"));
			assert.IsFalse(locEval("pos4 <= pos3"));
			assert.IsFalse(locEval("pos4 <  pos3"));

		};

		_ basic_logic_with_locals = assert => {
			var names  = new [] { 
				"pos1", "pos2", "zero"
			};

			var locals = new object[] { 
				1, 2, 0
			};

			var sess = new Session(789);
			Func<string, object> locEval = src =>
					Eval(src, names, locals, ref sess);

			// And
			assert.IsTrue(locEval("pos1 and pos1"));
			assert.IsTrue(locEval("pos1=pos1 and pos2=pos2"));
			assert.IsFalse(locEval("pos1=pos1 and pos1=pos2"));
			assert.IsFalse(locEval("pos1=pos2 and pos1=pos1"));

			// Or
			assert.IsTrue(locEval("zero or pos1"));
			assert.IsTrue(locEval("pos1=pos1 or pos1=pos2"));
			assert.IsTrue(locEval("pos1=pos2 or pos1=pos1"));
			assert.IsTrue(locEval("pos1 or zero"));
			assert.IsFalse(locEval("zero or zero"));

			// Not
			assert.IsTrue(locEval("not zero"));
			assert.IsFalse(locEval("not pos1"));
			assert.IsFalse(locEval("not pos1=pos1"));
			assert.IsTrue(locEval("not pos1=zero"));

		};

		_ basic_function_calls = assert => {
			assert.Equal(true, Eval("Bln(1)"));
		};

		_ hook_api_call  = assert => {
			var bak = Interpreter.OnDispatchCall;
			try {

				// Hook an external api.
				Interpreter.OnDispatchCall = (reciever, name, arg) =>
						new MethodResult(handled: true, result: 123);

				assert.Equal(123, Eval("NonStdCall('frali', 'fruli', 'fru')"));
			}
			finally {
				// Restore OnDispatchCall original implementation to 
				// avoid cross test failures.
				Interpreter.OnDispatchCall = bak;
			}
		};

		_ compile_cache = assert => {
			var sess   = new Session(1);
			var names  = new [] {"a", "b"}; 

			for (int i=0; i < 100; ++i) {
				// We must change locals to avoid run-time caching.
				var locals = new object[] { i, 2 };
				assert.Equal(i + 2d, Eval("a+b", names, locals, ref sess));
			}

			assert.Equal(1, sess.CacheMisses, "wrong number of cache misses");
			assert.Equal(99, sess.CacheHits,   "wrong number of cache hits");
		};


		_ run_time_cache = assert => {
			var sess   = new Session(1);
			var names  = new [] {"a", "b"}; 
			var locals = new object[] {1, 2};

			for (int i=0; i < 100; ++i)
				assert.Equal(3d, Eval("a+b", names, locals, ref sess));

			assert.Equal(1,  sess.ResCacheMisses, "wrong number of misses");
			assert.Equal(99, sess.ResCacheHits,   "wrong number of hits");
		};

		// TODO: Function calls with recievers.
		// TODO: Op presedence
	}
}

