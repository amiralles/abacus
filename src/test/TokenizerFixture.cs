#pragma warning disable 414, 219

namespace Abacus.Test {
	using static Interpreter;
	using _ = System.Action<Contest.Core.Runner>;
	using TK = Abacus.TokenKind;

	class TokenizerFixture {

		static Token[] Tokenize(string src) =>
			new Tokenizer(src).Tokenize();

		_ func_call_with_reciever = assert => {
			var stream = Tokenize("123.456.toString()");
			assert.Equal(5, stream.Length);
			assert.Equal("123.456", stream[0].Text);
			assert.Equal(TK.NumericLiteral, stream[0].Kind);
			assert.Equal(TK.Dot,            stream[1].Kind);
			assert.Equal(TK.Identifier,     stream[2].Kind);
			assert.Equal(TK.LeftParen,      stream[3].Kind);
			assert.Equal(TK.RightParen,     stream[4].Kind);
		};

		_ empty_string = assert => {
			var stream = Tokenize("'' = 0");
			assert.Equal(TK.StringLiteral, stream[0].Kind);
		};

		_ and_operator = assert => {
			var stream = Tokenize("1 and 1");
			assert.Equal(3, stream.Length);
			assert.Equal(TK.And, stream[1].Kind);
		};

		_ should_ignore_white_space = assert => {
			var stream = Tokenize(" 2 +  3");
			assert.Equal(3, stream.Length);
		};

		_ should_ignore_LF = assert => {
			var stream = Tokenize(" 2 +  3\n\n");
			assert.Equal(3, stream.Length);
		};

		_ unary_minus = assert => {
			var stream = Tokenize(" -2 +  -3");
			assert.Equal(5, stream.Length);
			assert.Equal(TK.UnaryMinus, stream[0].Kind);
			assert.Equal(TK.UnaryMinus, stream[3].Kind);
		};

		_ paren_unary_minus = assert => {
			var stream = Tokenize(" -(2) +  -(3)");
			assert.Equal(9, stream.Length);
			assert.Equal(TK.UnaryMinus, stream[0].Kind);
			assert.Equal(TK.UnaryMinus, stream[5].Kind);
		};

		_ unary_plus = assert => {
			var stream = Tokenize(" +2 +  +3");
			assert.Equal(5, stream.Length);
			assert.Equal(TK.UnaryPlus, stream[0].Kind);
			assert.Equal(TK.UnaryPlus, stream[3].Kind);
		};
	}
}
