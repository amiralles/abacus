#pragma warning disable 414, 219
namespace Abacus.Test {
	using static Interpreter;
	using _ = System.Action<Contest.Core.Runner>;


	class Formulas {

		_ basic_math = assert => {
			assert.Equal( 5, Eval(" 2 +  3"));
			assert.Equal(-5, Eval("-2 + -3"));
			assert.Equal(-1, Eval(" 2 + -3"));
			assert.Equal(-1, Eval(" 2 -  3"));
			assert.Equal(-5, Eval("-2 + -3"));
			assert.Equal( 1, Eval("-2 +  3"));
			assert.Equal( 5, Eval("+2 + +3"));
			assert.Equal( 5, Eval("+2 + +3"));

			//TODO: Mul/Div/Mod
			//TODO: Pow
		};

		//TODO: Op presedence
		//TODO: Comparisons
		//TODO: And/Or/Not
	}
}
