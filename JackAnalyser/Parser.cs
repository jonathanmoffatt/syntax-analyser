using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace JackAnalyser
{
    public class Parser
    {
        private List<Token> tokens;
        private readonly INodeFactory nodeFactory;

        public Node Tree { get; set; }

        public Parser(INodeFactory nodeFactory)
        {
            this.nodeFactory = nodeFactory;
        }

        public void Parse(ITokeniser tokeniser)
        {
            tokens = new List<Token>();
            Token token = tokeniser.GetNextToken();
            while (token != null)
            {
                tokens.Add(token);
                token = tokeniser.GetNextToken();
            }
            Tree = nodeFactory.Get((KeywordToken)tokens.FirstOrDefault());
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
