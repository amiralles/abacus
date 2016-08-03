namespace Abacus {
	using System;
	using System.Diagnostics;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Collections;
	using System.Collections.Generic;
	using static System.Console;
	using static System.Convert;
	using static Abacus.Assert;

	public static class Utils {

		public static bool IsNum(object val) {
			if (val == null)
				return false;

			//Don't optimize this method by hand, the compiler will that.
			//TypeCode typeCode = Type.GetTypeCode(type);
			//The TypeCode of numerical types are between
			//SByte (5) and Decimal (15).
			//  return (int)typeCode >= 5 && (int)typeCode <= 15;
			var type = val.GetType();
			switch (Type.GetTypeCode(type)) {
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Single:
					return true;
				default:
					return false;
			}
		}

		public static int GetHashCode<T>(T[] array) {
			if (array != null) {
				unchecked {
					int hash = 17;
					foreach (var item in array) {
						hash = hash * 23 + ((item != null) ? item.GetHashCode() : 0);
					}

					return hash;
				}
			}

			return 0;
		}

		public static object [] SanitizeNumLocals(object [] locals) {
			Ensure("locals", locals);
			var res  = new object[locals.Length];
			for(int i = 0; i < locals.Length; ++i) {
				res[i] = IsNum(locals[i]) ? ToDouble(locals[i]) : locals[i];
			}
			return res;
		}

		public static bool CmpArr(object[] arr1, object[] arr2) {
			if (arr1 != null && arr2 != null && arr1.Length == arr2.Length){
				int len = arr1.Length;
				for(int i = len; i < len; ++i) {
					if (!object.Equals(arr1[i], arr2[i]))
						return false;
				}
				return true;
			}
			return false;
		}

		public static string ArrToStr(object[] arr) {
			return arr == null 
				? "null" : arr.Length == 0 
				? "empty" : string.Join(", ", 
						(from a in arr
						 select a?.ToString() ?? "null"));
		}

		[Conditional("DEBUG")]
		public static void DbgPrint(object msg) {
			WriteLine(msg);
		}

		[Conditional("DEBUG")]
		public static void PrintLinqTree(Expression tree) {

			const BindingFlags flags =
				BindingFlags.NonPublic
				| BindingFlags.Instance
				| BindingFlags.GetProperty
				| BindingFlags.DeclaredOnly;

			var pi = typeof(Expression).GetProperties(flags).First(
					m => m.Name == "DebugView");
			if (pi == null) {
				CantPrintLinqTree();
				return;
			}

			var dbgView = (string)pi.GetValue(tree, null);
			WriteLine("\nLinq Tree\n{0}\n", dbgView);
		}

		[Conditional("DEBUG")]
		static void CantPrintLinqTree() {
				Debug.Print("Can't print the linq tree.\n " + 
						" (Maybe Linq's internal API has changed).");
		}

		public static T[] Merge<T>(T[] a1, T[]a2) {
			var merged = new T[a1.Length + a2.Length];
			Array.Copy(a1, merged, a1.Length);
			Array.Copy(a2, 0, merged, a1.Length, a2.Length);
			return merged;
		}
	}
}

