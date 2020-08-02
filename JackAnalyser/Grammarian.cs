using System;
using System.Collections.Generic;

namespace JackAnalyser
{
    public class Grammarian : IGrammarian
    {
        public BranchNode Get(params Token[] tokens)
        {
            return Get(new Queue<Token>(tokens));
        }

        public BranchNode Get(Queue<Token> tokens)
        {
            ClassNode root = new ClassNode();
            root.Children.Add(tokens.Dequeue());
            Token identifier = DequeueIdentifier(root, tokens, "className");
            root.ClassName = identifier.Value;
            DequeueSymbol(root, tokens, "{");
            DequeueSymbol(root, tokens, "}");
            return root;
        }

        private Token DequeueIdentifier(ClassNode root, Queue<Token> tokens, string description)
        {
            Token identifier = tokens.Count > 0 ? tokens.Dequeue() : null;
            if (identifier is IdentifierToken)
                root.Children.Add(identifier);
            else
                throw new ApplicationException($"class expects a '{description}' identifier");
            return identifier;
        }

        private Token DequeueSymbol(ClassNode root, Queue<Token> tokens, string symbol)
        {
            Token symbolToken = tokens.Count > 0 ? tokens.Dequeue() : null;
            if (symbolToken is SymbolToken && symbolToken.Value == symbol)
                root.Children.Add(symbolToken);
            else
                throw new ApplicationException($"class '{root.ClassName}' expects symbol '{symbol}'");
            return symbolToken;
        }
    }
}
