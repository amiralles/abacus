
#pragma warning disable 414, 219
namespace Abacus.Test {
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Reflection;
	using System.Linq.Expressions;
	using static Abacus.Utils;
	using static Abacus.Error;
	using static System.Linq.Expressions.Expression;
	using static Interpreter;
	using static System.Console;
	using static System.Convert;
	using static System.DateTime;
	using _ = System.Action<Contest.Core.Runner>;

	class StringEvalFixture {
		_ basic_eval = assert => {
			assert.Equal(5d, "2+3".Eval());
		};
	}
}
