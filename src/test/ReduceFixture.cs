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

	class ReduceFixture {

		_ reduce_data_table = assert => {
			var tbl = new DataTable();
			tbl.Columns.Add("Precio", typeof(double));
			for (int i =0; i < 10; ++i) {
				var row = tbl.NewRow();
				row["Precio"] = i;
				tbl.Rows.Add(row);
			}

			//TODO: Add more test, increase volume, apply mutiple reductions to 
			//      the same set.
			//      Add an overload that takes a session object handled by usr code.
			tbl = tbl.Reduce("Precio >= 5");
			assert.Equal(5, tbl.Rows.Count);
		};
	}
}
