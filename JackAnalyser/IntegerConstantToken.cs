namespace JackAnalyser
{
    public class IntegerConstantToken : Token
    {
        protected override string ElementName => "integerConstant";

        public IntegerConstantToken(string integer) : base(integer)
        {
        }
    }
}
