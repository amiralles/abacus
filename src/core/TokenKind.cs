namespace Abacus {

    public enum TokenKind {
        //=========================================================
		// Use values under 10 for "ignored" tokens.
        BOF = -1,
        EOF = 0,
        Comment = 1,
        WhiteSpace = 2,
        Tab = 10,
        //=========================================================
		// Use numbers from 11 to 30 to represent arithmetic ops.
        Add=11,
        Sub=12,
        Div=13,
        Mul=14,
        Mod=15,
        Mod2=16,
		FloorDiv=17,
		Pow=18,
        //=========================================================
		// Use numbers from 100 to 110 to represent comparison ops.
        Equal=100,
        NotEqual=101,
        GreaterThan=102,
        LessThan=103,
        GreaterThanOrEqual=104,
        LessThanOrEqual=105,
		In = 200,
        //=========================================================

		Let,
		Ignore,
        And,
        Or,
        Dot,
        RangeOp,
        Arrow,
        AssignOp,
        CellOp,
        NewLine,
        None,
        Colon,
        SemiColon,
        Comma,
        LeftCurly,
        RightCurly,
        PostfixDecrement,
        PrefixDecrement,
        PostfixIncrement,
        PrefixIncrement,
        Concat,
        RightBracket,
        RightParen,
        StringLiteral  = 1011,
        NumericLiteral = 1013,
        Not            = 1035,
        UnaryMinus     = 1031,
        UnaryPlus      = 1033,
        Identifier     = 1037,
        BoolLiteral    = 1043,
        LeftParen      = 1045,
        LeftBracket    = 1047,
        TrueLiteral    = 1051,
        FalseLiteral   = 1053
    }
}
