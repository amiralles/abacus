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
			tbl.Columns.Add("Fecha",  typeof(DateTime));

			var startDate = new DateTime(2012, 05, 14);
			for (int i = 0; i < 10; ++i) {
				var row = tbl.NewRow();
				row["Precio"] = i;
				row["Fecha"]  = startDate.AddDays(i);
				tbl.Rows.Add(row);
			}

			var red = tbl.Reduce("Precio >= 5");
			assert.Equal(5, red.Rows.Count);

			red = tbl.Reduce("Precio >= 5 or Precio = 2");
			assert.Equal(6, red.Rows.Count);

			red = tbl.Reduce("Fecha = dat('15/05/2012')");
			assert.Equal(1, red.Rows.Count);
		};
	}
}
