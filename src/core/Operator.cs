namespace Abacus {
    public enum Operator {
		// unary
        Pos,
        Neg,

		// artihmetic
        Add,
        Sub,
        Multiply,
        Divide,
        Modulo,
        Pow,
        FloorDiv, //DIV

		// comparisons
        Equal,
        NotEqual,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
		// special case
        Concat
    }
}
