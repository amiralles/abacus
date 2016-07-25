
namespace Abacus {
	using System;
	using System.Reflection;
	using System.Diagnostics;
	using System.Collections.Generic;
	using System.Linq;
	using System.Linq.Expressions;
	using static Abacus.Assert;
	using static Abacus.Utils;
	using static Abacus.Error;
	using static System.Console;
	using static System.Linq.Expressions.Expression;
	using BF = System.Reflection.BindingFlags;
	using FN = System.Func<string[], object[], object>;

	public struct Session {
		//Do not use this field, use GetCompilationCache instead.
		Dictionary<string, FN> _compilationCache;

		public Session(int id) {
			_compilationCache = new Dictionary<string, FN>();
			CacheMisses = 0;
			CacheHits = 0;
			Id = id;
		}

		Dictionary<string, FN> GetCompilationCache() => _compilationCache ?? 
			(_compilationCache = new Dictionary<string, FN>());

		public int CacheMisses, CacheHits, Id;

		/// Returns a cached version of the compiled function
		/// if it has one, null otherwise.
		public Func<string[], object[], object> GetCompiled(string src) {
			var res = GetCompilationCache().ContainsKey(src) 
				? GetCompilationCache()[src]
				: null;

			if (res == null)
				++ CacheMisses;
			else
				++ CacheHits;

			return res;
		}

		public void Cache(string src, object[] locals, object res) {
			//TODO: Compute hash\
			//TODO: Save res.	
		}

		/// Caches a compilation unit.
		public void CacheCompilation (string src, FN fn) {
			Ensure("src", src);
			Ensure("fn", fn);

			_compilationCache[src] = fn;
		}

		public bool TryGetRes(string src, object[] locals, ref object res) {
			res = null;
			return false;
		}
	}
}

