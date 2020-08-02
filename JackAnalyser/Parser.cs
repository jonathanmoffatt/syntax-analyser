using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace JackAnalyser
{
    public class Parser
    {
        private List<Token> tokens;
        private readonly IGrammarian nodeFactory;

        public Node Tree { get; set; }

        public Parser(IGrammarian nodeFactory)
        {
            this.nodeFactory = nodeFactory;
        }

        public void Parse(ITokeniser tokeniser)
        {
            tokens = new List<Token>();
            var queue = new Queue<Token>();
            Token token = tokeniser.GetNextToken();
            while (token != null)
            {
                tokens.Add(token);
                queue.Enqueue(token);
                token = tokeniser.GetNextToken();
            }
            Tree = nodeFactory.Get(queue);
        }

        public Token[] Tokens() => tokens.ToArray();

        public XDocument ToXml()
        {
            throw new NotImplementedException();
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
