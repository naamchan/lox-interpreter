namespace interpreter
{
    internal class Token
    {
        internal TokenType TokenType { get; }
        internal string Lexeme { get; }
        internal object Literal { get; }
        internal int Line { get; }

        internal Token(TokenType tokenType, string lexeme, object literal, int line)
        {
            this.TokenType = tokenType;
            this.Lexeme = lexeme;
            this.Literal = literal;
            this.Line = line;
        }

        public override string ToString()
        {
            return $"{TokenType} {Lexeme} {Literal}";
        }
    }
}