
namespace Abacus {
	using System;
	using System.Diagnostics;
	using System.Linq;
	using System.Collections.Generic;
	using static System.Console;
	using static Abacus.Error;
	using static Abacus.Assert;
	using TK = Abacus.TokenKind;
	using BinOp = Abacus.BinaryOperator;

	public class Parser {
#if DEBUG
		const int DMP_STREAM_LEN = 20;
		const int MAX_SAME_PEEK_COUNT = 300;
		int _lastPeekPos, _samePeekCount;
#endif
		readonly Token[] _stream;
		readonly string[] _locals;
		readonly Token EOFToken      = new Token("\0", TK.EOF, -1, -1);
		readonly SyntaxNode NullExpr = new Const(null);
		readonly SyntaxNode STD      = new Const(null);

		int _currPos       = -1,
			_currLineNo    =  1;

#if DEBUG
		SyntaxTree _trackingTree;
#endif
		// Token _lastToken;

		static readonly Dictionary<TK, BinOp>  _arithOps = 
			new Dictionary<TK, BinOp> {
				{ TK.Add,      BinOp.Add },
				{ TK.Sub,      BinOp.Sub },
				{ TK.Mul,      BinOp.Mul },
				{ TK.Div,      BinOp.Div },
				{ TK.FloorDiv, BinOp.FloorDiv },
				{ TK.Mod,      BinOp.Mod },
				{ TK.Pow,      BinOp.Pow },
			};

		static readonly Dictionary<TK, BinOp>  _cmpOps = 
			new Dictionary<TK, BinOp> {
				{ TK.Equal,              BinOp.Eq },
				{ TK.NotEqual,           BinOp.NEq },
				{ TK.LessThan,           BinOp.Lt },
				{ TK.LessThanOrEqual,    BinOp.Lte },
				{ TK.GreaterThan,        BinOp.Gt },
				{ TK.GreaterThanOrEqual, BinOp.Gte },
			};


		public Parser(Tokenizer tokenizer, string[] locals) {
			Ensure("tokenizer", tokenizer);
			_stream = tokenizer.Tokenize();
			_locals = locals;
		}


#if DEBUG
		void TrackTree(ref SyntaxTree tree) {
			_trackingTree = tree;
		}
#endif

		public SyntaxTree Parse() {
			var tree = new SyntaxTree();

#if DEBUG
			TrackTree(ref tree);
#endif

			while(!EOF)
				tree.Add(ParseExpression());

			return tree;
		}

		SyntaxNode ParseParenExpr() {
			ReadToken(TokenKind.LeftParen);
			var res = new ParenExpression(ParseOr());
			ReadToken(TokenKind.RightParen);
			return res;
		}

		SyntaxNode ParseUnaryPlus() {
			ReadToken(TK.UnaryPlus);
			return new UnaryExpression(Operator.Pos, ParseFactor());
		}

		SyntaxNode ParseUnaryMinus() {
			ReadToken(TK.UnaryMinus);
			return new UnaryExpression(Operator.Neg, ParseFactor());
		}

		SyntaxNode ParseFactor() {
			switch (PeekToken().Kind) {
				case TokenKind.UnaryPlus:  return ParseUnaryPlus();
				case TokenKind.UnaryMinus: return ParseUnaryMinus();
				case TokenKind.LeftParen:  return ParseParenExpr();
				default:
				   var res = ParsePrimary();
				   res = AddTrailers(res);
				   return res;
			}
		}

		SyntaxNode ParsePrimary() {
			var la = PeekToken();

			if (la.Kind == TK.NumericLiteral)
				return ParseNumLit();

			if (la.Kind == TK.StringLiteral)
				return ParseStrLit();

			if (la.Kind == TK.Identifier) {
				var name = ReadToken(TK.Identifier).Text;
				if (_locals.Contains(name))
					return new GetLocal(name);

				//Std call.
				return ParseFnCall(STD, name);
			}

			var msg = $"Can't parse primary => {la}.";
#if DEBUG
			msg += "\n{DumpTokens()}"); 
#endif
			Die(msg);
			return null;
		}

		SyntaxNode ParseNumLit() {
			var t = ReadToken(TK.NumericLiteral);
			if (t.Text.Contains('.'))
				return new Const(double.Parse(t.Text), typeof(double));
			return new Const(int.Parse(t.Text), typeof(int));
		}

		SyntaxNode ParseStrLit() {
			var t = ReadToken(TK.StringLiteral);
			return new Const(t.Text, typeof(string));
		}

        SyntaxNode AddTrailers(SyntaxNode expr) {
            if (TryReadToken(TokenKind.Dot)) {
                var name = ReadToken(TK.Identifier).Text;
				return ParseFnCall(expr, name);
            }
			
            return expr;
		}

		SyntaxNode ParseFnCall(SyntaxNode target, string funcname) {
			Ensure("target", target);
			Ensure("funcname", funcname);

			ReadToken(TK.LeftParen);
			var argv = new List<SyntaxNode>();

			while(PeekToken().Kind != TK.RightParen) {
				if (TryReadToken(TK.Comma)) {
					argv.Add(NullExpr); //<= optional.
				}
				else {
					argv.Add(ParseExpression());
					TryReadToken(TK.Comma);
				}
			}

			ReadToken(TK.RightParen);

			return new FuncCall(target, funcname, argv.ToArray());
		}

		BinaryOperator GetCmpOperator(Token t) {
			return _cmpOps[t.Kind];
		}


		BinaryOperator GetArithOperator(Token t) {
			return _arithOps[t.Kind];
		}

		BinaryOperator GetStringOperator(Token t) {
			return BinaryOperator.Concat;
		}

		SyntaxNode ParseExpression(int precedence) {
			SyntaxNode node = ParseFactor();

			while (true) {
				var t = PeekToken();

				BinaryOperator op;
				if (t.IsArithmeticOp)
					op = GetArithOperator(t);
				else if (t.IsStringOp)
					op = GetStringOperator(t);
				else
					return node;

				if (op.Precedence >= precedence) {
					ReadToken();
					var rhs = ParseExpression(op.Precedence + 1);
					node = new BinaryExpression(op, node, rhs);
				}
				else {
					return node;
				}
			}
		}

		SyntaxNode ParseExpression() {
			return ParseOr();
		}

		SyntaxNode ParseOr() {
            var node = ParseAnd();
            while (TryReadToken(TokenKind.Or))
                node = new OrExpression(node, ParseOr());

            return node;
		}

		SyntaxNode ParseAnd() {
            var node = ParseNot();
            while (TryReadToken(TokenKind.And))
                node = new AndExpression(node, ParseAnd());

            return node;
		}

		SyntaxNode ParseNot() {
            return TryReadToken(TokenKind.Not)
                ? new NotExpression(ParseComparison())
                : ParseComparison();
		}

		SyntaxNode ParseComparison(){
            var node = ParseExpression(precedence: 0);
            while (true) {
                var ot = PeekToken();
                switch (ot.Kind) {
                    case TokenKind.Equal:
                    case TokenKind.NotEqual:
                    case TokenKind.GreaterThan:
                    case TokenKind.LessThan:
                    case TokenKind.GreaterThanOrEqual:
                    case TokenKind.LessThanOrEqual:
                        ReadToken(ot.Kind);
                        break;
                    default:
                        return node;
                }
                var rhs = ParseComparison();
                node = MakeComparison(GetCmpOperator(ot), node, rhs);
            }
		}

        SyntaxNode MakeComparison(
				BinaryOperator op, SyntaxNode lhs, SyntaxNode rhs) {
            switch (op.Operator) {
                case Operator.Equal:
                    return new EqualExpression(lhs, rhs);
                case Operator.NotEqual:
                    return new NotEqualExpression(lhs, rhs);
                case Operator.LessThan:
                    return new LessThanExpression(lhs, rhs);
                case Operator.LessThanOrEqual:
                    return new LessThanEqExpression(lhs, rhs);
                case Operator.GreaterThan:
                    return new GreaterThanExpression(lhs, rhs);
                case Operator.GreaterThanOrEqual:
                    return new GreaterThanEqExpression(lhs, rhs);
                default:
                    Die($"Comparison expected => '{op}'");
					return null;
            }
        }

		void MoveNext() {
			++ _currPos;
		}

		Token ReadToken(TK kind = default(TK)) {
			var la = PeekToken();

			DieIf(kind != default(TK) && la.Kind != kind, 
				  ExpectedButWas(kind, la.Kind));

			if (la.Kind != TK.EOF) {
				MoveNext();
			}

			if (la.Kind == TK.NewLine)
				++ _currLineNo;

			// _lastToken = la;
			return la;
		}

		Token PeekToken(int at = 1) {
			var la = (_currPos + at) >= _stream.Length
				? EOFToken 
				: _stream[_currPos + at];


#if DEBUG
			PreventPeekOverflow(la);
#endif
			return la;
		}

#if DEBUG
		void PreventPeekOverflow(Token token) {
			if (_lastPeekPos == _currPos) {
				++_samePeekCount;
			}
			else {
				_lastPeekPos   = _currPos;
				_samePeekCount = 0;
			}

			DieIf(_samePeekCount == MAX_SAME_PEEK_COUNT,
				$"Way too many peeks at the same pos.\n" +
				$"Current pos.: { _currPos }\n" + 
				$"Peeks count : { MAX_SAME_PEEK_COUNT }\n" +
				$"Peeked token: { token }");
		}
#endif


		bool TryReadToken(TK kind) {
			Token t;
			return TryReadToken(kind, out t);
		}

		bool TryReadToken(TK kind, out Token token) {
			token = null;
			if (PeekToken().Kind == kind)
				token = ReadToken();
			return token != null;
		}

		bool EOF => _currPos + 1 >= _stream.Length;

		bool EOL => PeekToken().Kind == TK.NewLine;


		string ExpectedButWas(TK expected, TK actual) =>
			$"Expected {expected} was {actual}.\n" + PosInfo();

		string PosInfo() => 
			$"Line: {_currLineNo}\n" +
			$"Current pos.: {_currPos}.\n"
#if DEBUG
			+ DumpTokens()
#endif
			+ "";

#if DEBUG

		string DumpTokens(){
			var strm = _stream.GetTokens();
			var dmp = strm.Reverse();
			if (dmp.Length == 0)
				return "";

			dmp = dmp.Take(DMP_STREAM_LEN).ToArray();
			int len = dmp.Length > DMP_STREAM_LEN ? DMP_STREAM_LEN : dmp.Length;
			var msg = "===================================\n";
			msg += "DEBUG INFO\n";
			msg += "===================================\n";
			msg += $"Current pos.: {_currPos}\n";
			msg += $"Next token:   {PeekToken()}\n";
			msg += "===================================\n";

			Func<Token, string> frmt = t => {
				int idx = Array.IndexOf(strm, t);
				var res = $"[{idx}] {t}";
				if (idx == _currPos)
					res += "    <===== !!!!";
				return res;
			};

			return msg + string.Join("\n", (from t in dmp select frmt(t)));
		}
#endif

		[Conditional("DEBUG")]
		void WatchIf(bool cnd, object aditionalInfo = null) {
			if (cnd)
				Watch(aditionalInfo);
		}

		[Conditional("DEBUG")]
		void Watch(object aditionalInfo = null) {

			WriteLine("--------------------------------------");
			WriteLine("Locals");
			WriteLine("--------------------------------------");
			WriteLine($"last token:       {_stream.Last()}");
			WriteLine($"next token:       {PeekToken()}");
			WriteLine($"cursor pos:       {_currPos}");
			WriteLine($"curr lineNo:      {_currLineNo}");
			WriteLine($"input len:        {_stream.Length}");
			WriteLine($"EOF:              {EOF}");
			WriteLine("--------------------------------------");

			if (aditionalInfo != null)
				WriteLine(aditionalInfo);

			WriteLine("Press [Enter] to continue.");
			ReadLine();
		}
	}

}
