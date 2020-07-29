namespace JackAnalyser
{
    public abstract class Token
    {
        protected Token(string tokenValue)
        {
            this.Value = tokenValue;
        }

        public string Value { get; private set; }
    }
}
