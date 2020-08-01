namespace JackAnalyser
{
    public class SymbolToken : Token
    {
        protected override string ElementName => "symbol";

        public SymbolToken(string symbol) : base(symbol)
        {
        }
    }
}
