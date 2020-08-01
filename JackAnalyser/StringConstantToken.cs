namespace JackAnalyser
{
    public class StringConstantToken : Token
    {
        protected override string ElementName => "stringConstant";

        public StringConstantToken(string stringConstant) : base(stringConstant.Replace("\"", ""))
        {
        }
    }
}
