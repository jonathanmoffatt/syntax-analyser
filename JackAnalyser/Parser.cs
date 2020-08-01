using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace JackAnalyser
{
    public class Parser
    {
        private List<Token> tokens;

        public Tree Tree { get; set; }

        public void Parse(ITokeniser tokeniser)
        {
            tokens = new List<Token>();
            Token token = tokeniser.GetNextToken();
            while (token != null)
            {
                tokens.Add(token);
                token = tokeniser.GetNextToken();
            }
        }

        public XDocument ToXml()
        {
            throw new NotImplementedException();
        }

        public Token[] Tokens()
        {
            return tokens.ToArray();
        }

        public XDocument TokensXml()
        {
            var xml = new XDocument();
            XElement root = new XElement("tokens");
            xml.Add(root);
            foreach (Token token in tokens)
            {
                root.Add(token.ToXml());
            }
            return xml;
        }

    }
}
