namespace JackAnalyser
{
    public class IdentifierToken : Token
    {
        public IdentifierToken(string identifier) : base(identifier)
        {
        }

        protected override string ElementName => "identifier";
    }
}
