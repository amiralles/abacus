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

		//TODO: Add hook for client func calls.
		_ reduce_data_table = assert => {
			var tbl = new DataTable();
			tbl.Columns.Add("Price", typeof(double));
			tbl.Columns.Add("Date",  typeof(DateTime));
			tbl.Columns.Add("User",  typeof(string));

			var startDate = new DateTime(2012, 05, 14);
			for (int i = 0; i < 10; ++i) {
				var row = tbl.NewRow();
				row["Price"]  = i;
				row["Date"]   = startDate.AddDays(i);
				row["User"] = i % 2 == 0 ?  "amiralles" : "pipex";

				tbl.Rows.Add(row);
			}

			var red = tbl.Reduce("Price >= 5");
			assert.Equal(5, red.Rows.Count);

			red = tbl.Reduce("Price >= 5 or Price = 2");
			assert.Equal(6, red.Rows.Count);

			red = tbl.Reduce("Date = dat('15/05/2012')");
			assert.Equal(1, red.Rows.Count);

			// Mix table values with local vars.
			var names  = new [] { "usr" };
			var locals = new object[] { "pipex" };
			var fx = "User = usr and Date = dat('15/05/2012')";
			red = tbl.Reduce(fx, names, locals);
			assert.Equal(1, red.Rows.Count);

			// Date Add
			red = tbl.Reduce("Date = dat('15/05/2012') + 1");
			assert.Equal(1, red.Rows.Count);

			// Date Sub
			red = tbl.Reduce("Date = dat('15/05/2012') - 1");
			assert.Equal(1, red.Rows.Count);
		};
	}
}
