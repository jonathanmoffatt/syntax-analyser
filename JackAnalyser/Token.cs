namespace JackAnalyser
{
    public abstract class Token : Node
    {
        public string Value { get; private set; }

        protected Token(string tokenValue)
        {
            Value = tokenValue;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
