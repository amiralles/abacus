
namespace Abacus {
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Linq.Expressions;
	using static System.Console;
	using static System.String;
	using static System.Char;
	using static Abacus.Error;
	using static Abacus.Assert;
	using TK = Abacus.TokenKind;

	// NOTES:
	// All text will be downcased. String literals are the only exception
	// to this rule.
	public class Tokenizer {
		const string MOD1 = "mod", MOD2 = "mod2", FDIV = "div";
		const string AND = "and", OR = "or";
		const string NOT = "not";

		const char EQ = '=', LT = '<', GT = '>';

		const char PLS  = '+', MIN = '-',
				   MUL  = '*', DIV = '/', MOD = '%',
				   STAR = '*', POW = '^';

		const char LPAREN   = '(', RPAREN   = ')',
				   LBRACKET = '[', RBRACKET = ']';

		const char COLON = ':', SEMI = ';', COMMA = ',';

		const char LF = '\n', CR= '\r', SPACE = ' ', HT = '\t',
				   SQ = '\'', DQ = '\"',
				   NULLTERM = '\0',
				   TILDE = '~',
				   DOT ='.'
				   ;

		const int 
            // Safety Check.
			// If a word is longer than MAX_WORD_LEN it might be
			// an internal error that will endup overflowing 
			// the lexer. This allows us to prevent that overflow.
			// (This check is NOT testest against strings).
			MAX_WORD_LEN = 100,
			// How many tokens we'll show on dumps.
			DMPLEN = 10;

		// The input stream.
		readonly char[] _input;

		Token _lastToken;

		static readonly Token IgnoreToken = new Token("", TK.Ignore, -1, -1);

		int _currentPos  = -1,
			_endsAtColNo = -1,
		   	_lineNo      =  1;

#if DEBUG
		readonly List<Token> DumpStream = new List<Token>();
#endif

		public Tokenizer(string src) {
			_input = src.ToCharArray();
		}

		/// Tokenizes the input stream.
		public Token[] Tokenize() {
			var res = new List<Token>();
			Token t;
			TK k;
			while(!EOF) {
				t = ReadTokenOrDie();
				k = t.Kind;
				if (k == TK.Ignore || k == TK.NewLine)
					continue;

				res.Add(t);
			}
			return res.ToArray();
		}

		/// Reads the next token from the stream.
		/// (Skips meaningless tokens).
		Token ReadTokenOrDie(){
			var ppos = _currentPos;
			Token token = 
				TokenizeSpace()    ??
				TokenizeTab()      ??
				TokenizeLF()       ??
				TokenizeParen()    ??
				TokenizeBraket()   ??
				TokenizeCurly()    ??
				TokenizeUnaryOp()  ??
				TokenizeBinaryOp() ??
				TokenizeConcatOp() ??
				TokenizeRangeOp()  ??
				TokenizeDot()      ??
				TokenizeComma()    ??
				TokenizeSemi()     ??
				TokenizeColon()    ??
				TokenizeNum()      ??
				TokenizeStr()      ??
				TokenizeID();

			WriteLine(token?.ToString() ?? "null");
			if(token == null) {
				DieUnkownToken();
			}

			if (ppos == _currentPos)
				DieCursorDidntMove();

			return token;
		}

		void DieCursorDidntMove() {
			Die($"The cursor didn't move.\n"   +
				$"Pos: {_currentPos}\n"        +
				$"Last token: {_lastToken})\n" +
				$"Next word : {PeekWord()}");
		}

		Token TokenizeLF() { 
			char la;
            bool lf = false;
			while((la = PeekChar()) == LF || la == CR) {
				ReadChar();
                lf = true;
            }

			return lf ? CreateToken(TK.NewLine, LF) : null;
		}

		void EatWhileNextMatch(char ch) {
			while(PeekChar() == ch)
				ReadChar();
		}

		void EatUntilNextMatch(char ch) {
			while(PeekChar() != ch)
				ReadChar();
		}

		Token TokenizeSpace() {
			if (PeekChar() == SPACE) {
				EatWhileNextMatch(SPACE);
				return IgnoreToken;
			}
			return null;
		}


		Token TokenizeTab() {
			if (PeekChar() == HT) {
				EatWhileNextMatch(HT);
				return IgnoreToken;
			}
			return null;
		}

        string Expected (string term) {
            return $"Expected term: {term}." + 
                   $"next char: ({PeekChar()}) " + 
				   $"- pos: {_currentPos} - ln: {_lineNo}.";
        }

        string SingleQuoteExpected () => Expected("Single quote");

        string DoubleQuoteExpected () => Expected("Double quote");

		Token TokenizeParen()    { 
			if (PeekChar() == LPAREN)
				return CreateToken(TK.LeftParen, ReadChar());

			if (PeekChar() == RPAREN)
				return CreateToken(TK.RightParen, ReadChar());

			return null; 
		}
        
		Token TokenizeBraket()   {
			if (PeekChar() == LBRACKET)
				return CreateToken(TK.LeftBracket, ReadChar());

			if (PeekChar() == RBRACKET)
				return CreateToken(TK.RightBracket, ReadChar());

		   	return null; 
		}

		Token TokenizeCurly()    { return null; }

		static bool IsDelimiter(Token t) {
			return t.Kind == TK.Comma || t.Kind == TK.SemiColon;
		}

		static bool IsOperator(Token t) {
			int kind = (int) t.Kind;
			return (kind >= 11 && kind <= 32) || t.Kind == TK.Dot;
		}

		// Returns true is t is Operator, Terminator or Delimiter.
		static bool IsOTD(Token t) {
			return IsOperator(t) || 
				   IsDelimiter(t);
		}

		Token TokenizeUnaryOp() { 
			if (PeekWord() == NOT)  
				return CreateToken(TK.Not, ReadWord());

			if (PeekChar() == MIN && (_lastToken == null || IsOTD(_lastToken)))
				return CreateToken(TK.UnaryMinus, ReadChar());

			if (PeekChar() == PLS && (_lastToken == null || IsOTD(_lastToken)))
				return CreateToken(TK.UnaryPlus, ReadChar());

			return null;
		}

		Token TokenizeConcatOp() { 
			return PeekChar() == TILDE
				? CreateToken(TK.Concat, ReadChar()) 
				: null;
		}

		Token TokenizeNotEq(char la2) {
			if (la2 == GT) {
				ReadChar();
				ReadChar();
				return CreateToken(TK.NotEqual, "<>");
			}
			return null;
		}

		Token TokenizeLtEq(char la2) {
			if (la2 == EQ) {
				ReadChar();
				ReadChar();
				return CreateToken(TK.LessThanOrEqual, "<=");
			}
			return null;
		}

		Token TokenizeGtEq(char la2) {
			if (la2 == EQ) {
				ReadChar();
				ReadChar();
				return CreateToken(TK.GreaterThanOrEqual, ">=");
			}
			return null;
		}

		Token TokenizePow(char la2) {
			if (la2 == MUL) {
				ReadChar();
				ReadChar();
				return CreateToken(TK.Pow, POW);
			}
			return null;
		}

		Token TokenizeBinaryOp() { 
			char la  = PeekChar();
			char la2 = PeekChar(at: 2);
			
			if (la == EQ) return CreateToken(TK.Equal, ReadChar());
			if (la == LT) {
				return TokenizeNotEq(la2) ??
				   	   TokenizeLtEq(la2)  ??
					   CreateToken(TK.LessThan, ReadChar());
			}

			if (la == GT) {
				return TokenizeGtEq(la2) ?? 
					   CreateToken(TK.GreaterThan, ReadChar());
			}

			if (la == PLS) return CreateToken(TK.Add, ReadChar());
			if (la == MIN) return CreateToken(TK.Sub, ReadChar());
			if (la == DIV) return CreateToken(TK.Div, ReadChar());
			if (la == POW) return CreateToken(TK.Pow, ReadChar());
			if (la == MOD) return CreateToken(TK.Mod, ReadChar());
			if (la == MUL)
				return TokenizePow(la2) ?? CreateToken(TK.Mul, ReadChar());

			string nextword;
			if ((nextword = PeekWord()) == MOD1 || nextword == MOD2)
				return CreateToken(TK.Mod, ReadWord());

			if (nextword == AND)
				return CreateToken(TK.And, ReadWord());

			if (nextword == OR)
				return CreateToken(TK.Or, ReadWord());

			if (nextword == FDIV)
				return CreateToken(TK.FloorDiv, ReadWord());


			return null; 
		}

		bool NextIsNameOrLiteral(string term) {

			if(IsNullOrEmpty(term))
				return false;

			var offset = _currentPos + term.Length;

			char ch = default(char);
			while(!EOF && ((ch = PeekCharAbsolute(offset + 1)) == SPACE))	{
				++offset;
			}

			return IsLetter(ch) || IsDigit(ch) || ch == '\"' || ch == '\'';
		}

		Token TokenizeRangeOp()  { return null; }

		Token TokenizeDot()      { return null; }

		Token TokenizeComma()    { 
            return PeekChar() == COMMA
                ? CreateToken(TK.Comma, ReadChar()) 
                : null;
		}

		Token TokenizeSemi()     { 
            return PeekChar() == SEMI
                ? CreateToken(TK.SemiColon, ReadChar()) 
                : null;
        }

		Token TokenizeColon() {
			if (PeekChar() == COLON) {
				ReadChar();
				return IgnoreToken;
			}
			return null;
		}

		Token TokenizeNum() { 
            return char.IsNumber(PeekChar()) ? ReadNum() : null;
        }

        Token ReadNum() {
            int size = 0;
            char ch;
            while((char.IsNumber((ch = PeekChar(at: size + 1))) || ch == DOT))
                ++size;

			var buff = ReadChars(size);
            return CreateToken(TK.NumericLiteral, new string(buff));
        }


		Token TokenizeStr() { 
			char ch;
			if ((ch = PeekChar()) == SQ)
				return TokenizeSingleQuoteStr();

			if (ch == DQ)
				return TokenizeDoubleQuoteStr();

			return null; 
		}

		Token TokenizeSingleQuoteStr() {
			DieIf(PeekChar() != SQ, SingleQuoteExpected());
			ReadChar();//open

			int size = 0;
			char ch;

			if (PeekChar() == SQ) { //<= empty string.
				ReadChar(); //close
				return CreateToken(TK.StringLiteral, $"''");
			}

			while((ch = PeekChar(at: size + 1)) != SQ && ch != NULLTERM)
				++size;

			var buff = ReadChars(size, downcase: false);
			DieIf(PeekChar() != SQ, SingleQuoteExpected());
			ReadChar();
			return CreateToken(TK.StringLiteral, $"'{new string(buff)}'");

		}

		Token TokenizeDoubleQuoteStr() {
			DieIf(PeekChar() != DQ, DoubleQuoteExpected());
			ReadChar();

			int size = 0;
			char ch;
			while((ch = PeekChar(at: size + 1)) != DQ && ch != NULLTERM)
				++size;
			var buff = ReadChars(size, downcase: false);

			DieIf(PeekChar() != DQ, DoubleQuoteExpected());
			ReadChar();
			return CreateToken(TK.StringLiteral, $"\"{new string(buff)}\"");
		}

		Token TokenizeID() {
			var nextWord = PeekWord();
			if (!IsNullOrEmpty(nextWord))
				return CreateToken(TK.Identifier, ReadWord());

			return null;
		}

		Token CreateToken(TK kind, char ch) {
	        return CreateToken(kind, string.Intern(ch.ToString()));
        }

		Token CreateToken(TK kind, string text) {
			var t =	 new Token(text, kind, _endsAtColNo, _lineNo);
#if DEBUG
			DumpStream.Add(t);
			DieIf(t.Kind == TK.Identifier && IsNullOrEmpty(t.Text),
					"ID no puede ser null.");
#endif
			_lastToken = t;
			return t;
		}

        bool WordTerminator(char ch) {
            var chnum = (int) ch;
            if ((chnum >= 48 && chnum <= 57)  || //<= 0..9
				(chnum >= 65 && chnum <= 90)  || //<= a-z
                (chnum >= 97 && chnum <= 122) || //<= A-Z
				(chnum == 95) //_
				)
                return false;

            return true;
        }

		string PeekWord(bool downcase = true) {
			int size = 0;
			while(!WordTerminator(PeekChar(at: size + 1)) && size < MAX_WORD_LEN)
				size++;

			var buff = new char[size];
			for(int i = 0; i < size; ++i) {
				var ch = PeekChar(at: i + 1);
				buff[i] = downcase ? char.ToLower(ch) : ch;
			}

			return new string(buff);
		}

        string ReadWord() {
            int size = 0;
            while(!WordTerminator(PeekChar(at: size + 1)) && size < MAX_WORD_LEN)
                size++;

            if (size == MAX_WORD_LEN)
                Die($"Tokens overflow. Tokens: {_input.Length} " + 
                    $"Pos: {_currentPos} - ln: {_lineNo} - " + 
					$"max len: {MAX_WORD_LEN}");

            return new string(ReadChars(size));
        }

		/// End Of File.
		bool EOF => PeekChar() == NULLTERM;

		/// End Of Line.
		bool EOL => PeekChar() == LF || EOF;

		void DieUnkownTokenRelease() {
			var nextWord = PeekWord();
			Die($"Unknown token.\n" + 
				$"Next term   => ({nextWord})\n"    +
				$"Current pos => {_currentPos} - Line: {_lineNo}.");
		}

#if DEBUG
		public Token[] GetReversedStream() {
			var res = new Token[DumpStream.Count];
			DumpStream.CopyTo(res);
			return res.Reverse().ToArray();
		}
#endif

        void DieUnkownToken() {
#if DEBUG
			var revStrm  = GetReversedStream();
			var dmp      = string.Join("\n", revStrm.Take(DMPLEN));
			var nextWord = PeekWord();

            Die($"Unknown token.\n" + 
                $"Last known token => {_lastToken}\n"    +
                $"Next term        => ({nextWord})\n"    +
                $"Next char        => ({PeekChar()})\n"  +
                $"Current pos      => {_currentPos} - Line: {_lineNo}\n" +
                $"Stream len       => {_input.Length}\n" +
				$"-----------------------------------\n" +
				$"{dmp}\n" + 
				$"-----------------------------------\n");
#else
			DieUnkownTokenRelease();
#endif
        }

		char[] ReadChars(int size, bool downcase = true) {
            char[] buffer = new char[size];
            for(int i = 0; i < size; ++i) {
				buffer[i] = downcase ? char.ToLower(ReadChar()) : ReadChar();
			}

			return buffer;
		}

        char ReadChar() {
            _endsAtColNo++;
            char ch = _input[++_currentPos];
			if (ch == LF)
				_lineNo++;
			return ch;
        }

        char PeekChar(int at = 1) {
            return _currentPos + at >= _input.Length 
				? NULLTERM
				: _input[_currentPos + at];
        }
	
        char PeekCharAbsolute(int idx) {
            return idx >= _input.Length 
				? NULLTERM
				: _input[idx];
        }
	
		/// Gets a subtring from the input buffer.
		string SubStr(int from, int to) {
			var blen = to - from;
			var buff = new char[blen];
			for (int i = from, j=0; i < _input.Length && j < blen; ++i, ++j)
				buff[j]=_input[i];
			return new string(buff).Trim(NULLTERM);
		}

		[Conditional("DEBUG")]
		void WatchIf(bool cnd, object aditionalInfo = null) {
			if (cnd)
				Watch(aditionalInfo);
		}

		/// Prints debug info and pauses the execution.
		[Conditional("DEBUG")]
		void Watch(object aditionalInfo = null) {

			WriteLine("--------------------------------------");
			WriteLine("Tokenizer Locals");
			WriteLine("--------------------------------------");
			WriteLine($"last token:  {_lastToken}");
			WriteLine($"next word:   ({PeekWord()})");
			WriteLine($"cursor pos:  {_currentPos}");
			WriteLine($"input len:   {_input.Length}");
			WriteLine($"curr lineNo: {_lineNo}");

			WriteLine($"EOF:         {EOF}");
			WriteLine("--------------------------------------");

			if (aditionalInfo!=null)
				WriteLine(aditionalInfo);

			WriteLine("Press [Enter] to continue.");
			ReadLine();
		}
	}
}
