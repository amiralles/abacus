#pragma warning disable 414, 219
namespace Abacus.Test {
	using static Interpreter;
	using _ = System.Action<Contest.Core.Runner>;
	using TK = Abacus.TokenKind;

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

		_ unary_minus = assert => {
			var tokenizer = new Tokenizer(" -2 +  -3");
			var stream = tokenizer.Tokenize();
			assert.Equal(5, stream.Length);
			assert.Equal(TK.UnaryMinus, stream[0].Kind);
			assert.Equal(TK.UnaryMinus, stream[3].Kind);
		};

		_ paren_unary_minus = assert => {
			var tokenizer = new Tokenizer(" -(2) +  -(3)");
			var stream = tokenizer.Tokenize();
			assert.Equal(9, stream.Length);
			assert.Equal(TK.UnaryMinus, stream[0].Kind);
			assert.Equal(TK.UnaryMinus, stream[5].Kind);
		};

		_ unary_plus = assert => {
			var tokenizer = new Tokenizer(" +2 +  +3");
			var stream = tokenizer.Tokenize();
			assert.Equal(5, stream.Length);
			assert.Equal(TK.UnaryPlus, stream[0].Kind);
			assert.Equal(TK.UnaryPlus, stream[3].Kind);
		};

	}
}
