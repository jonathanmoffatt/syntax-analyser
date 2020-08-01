using System;
namespace JackAnalyser
{
    public class KeywordToken : Token
    {
        protected override string ElementName => "keyword";

        public KeywordToken(string keyword) : base(keyword)
        {
        }
    }
}
