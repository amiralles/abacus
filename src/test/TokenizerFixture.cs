#pragma warning disable 414, 219
namespace Abacus.Test {
	using static Interpreter;
	using _ = System.Action<Contest.Core.Runner>;

	class TokenizerFixture {
		_ should_ignore_white_space = assert => {
			var tokenizer = new Tokenizer(" 2 +  3");
			var stream = tokenizer.Tokenize();
			assert.Equal(3, stream.Length);
		};

		_ should_ignore_LF = assert => {
			var tokenizer = new Tokenizer(" 2 +  3\n\n");
			var stream = tokenizer.Tokenize();
			assert.Equal(3, stream.Length);
		};
	}
}
