#pragma warning disable 414, 219
namespace Abacus.Test {
	using static Interpreter;
	using static System.Console;
	using _ = System.Action<Contest.Core.Runner>;


	class Formulas {

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
			assert.IsTrue(Eval("0 = ''"));
			assert.IsTrue(Eval("'' = ''"));
			assert.IsTrue(Eval("'' = 0"));
			assert.IsTrue(Eval("2 <> 3"));
			assert.IsTrue(Eval("2 >  1"));
			assert.IsTrue(Eval("2 >= 1"));
			assert.IsTrue(Eval("2 >= 2"));
			assert.IsTrue(Eval("2 <= 3"));
			assert.IsTrue(Eval("3 <= 3"));
			assert.IsTrue(Eval("2 <  3"));

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

		//TODO: Access locals
		//TODO: Function calls
		//TODO: Op presedence
		//
	}
}
