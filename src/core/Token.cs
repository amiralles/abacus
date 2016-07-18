namespace Abacus {
	using static System.String;

    public class Token {
        public readonly string Text;
        public readonly TokenKind Kind;
		public readonly int ColNo, LineNo;

        public Token(char ch, TokenKind kind, int endsAtColNo, int lineNo)
            : this(ch.ToString(), kind, endsAtColNo, lineNo) {
        }

        public Token(string text, TokenKind kind, int endsAtColNo, int lineNo) {
			Text  = text;
            Kind  = kind;
			ColNo  = endsAtColNo - text.Length;
			LineNo = lineNo;
        }

        public override string ToString() => Format(
				"({0}) {1}", Kind, Kind == TokenKind.NewLine ? "LF" : Text);

		public bool IsOperator
			=> IsComparisonOp || IsArithmeticOp || IsStringOp;

        public bool IsComparisonOp => KindIn(100,110);

        public bool IsArithmeticOp => KindIn(11, 30);

        public bool IsStringOp => Kind == TokenKind.Concat;

		bool KindIn(int low, int hi) {
			int k = (int)Kind;
			return k >= low && k <= hi;
		
		}
    }
}
