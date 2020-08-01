using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace JackAnalyser
{
    public class Parser
    {
        private List<Token> tokens;
        private ITokeniser tokeniser;

        public Parser(ITokeniser tokeniser)
        {
            this.tokeniser = tokeniser;
        }

        public Tree Tree { get; set; }

        public void Parse()
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
