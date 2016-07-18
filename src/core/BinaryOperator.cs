namespace Abacus {

	using BinOp = Abacus.BinaryOperator;

    public struct BinaryOperator {

        public BinaryOperator(Operator @operator, int precedence) {
            Operator = @operator;
            Precedence = precedence;
        }

        public Operator Operator;

        public int Precedence;

		public static BinOp 
			// Arithmetic
			Add = new BinOp(Operator.Add, 1),
			Sub = new BinOp(Operator.Sub, 1),
			Div = new BinOp(Operator.Divide, 2),
			Mul = new BinOp(Operator.Multiply, 2),
			Mod = new BinOp(Operator.Modulo, 2),
			FloorDiv = new BinOp(Operator.FloorDiv, 2),
			Pow = new BinOp(Operator.Pow, 4),

			// Concat
			Concat = new BinOp(Operator.Concat, 1),

			// Comparisons
			Eq  = new BinOp(Operator.Equal, 16),
			NEq = new BinOp(Operator.NotEqual, 16),
			Lt  = new BinOp(Operator.LessThan, 8),
			Lte = new BinOp(Operator.LessThanOrEqual, 8),
			Gt  = new BinOp(Operator.GreaterThan, 8),
			Gte = new BinOp(Operator.GreaterThanOrEqual, 8);

    }
}
