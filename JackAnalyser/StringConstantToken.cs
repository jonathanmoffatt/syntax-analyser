namespace JackAnalyser
{
    public class StringConstantToken : Token
    {
        public StringConstantToken(string stringConstant) : base(stringConstant.Replace("\"", ""))
        {
        }
    }
}
