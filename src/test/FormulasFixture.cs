#pragma warning disable 414, 219
namespace Abacus.Test {
	using static Interpreter;
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

			// mod
			assert.Equal( 1d,    Eval(" 3 %  2"));
			assert.Equal(-1d,    Eval("-3 % -2"));
			assert.Equal( 1d,    Eval(" 3 % -2"));
			assert.Equal(-1d,    Eval("-3 %  2"));
			assert.Equal( 1d,    Eval("+3 % +2"));
			assert.Equal(ERRNAN, Eval("+3 % 0"));

			// pow
			assert.Equal( 8d,    Eval(" 2 **  3"));
			assert.Equal(-0.125, Eval("-2 ** -3"));
			assert.Equal( 0.125, Eval(" 2 ** -3"));
			assert.Equal(-8d,    Eval("-2 **  3"));
			assert.Equal( 8d,    Eval("+2 ** +3"));
			assert.Equal( 1d,    Eval("+2 **  0"));
		};

		//TODO: Comparisons
		//TODO: And/Or/Not
		//TODO: Op presedence
	}
}
