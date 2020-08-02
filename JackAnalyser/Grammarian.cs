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
            DequeueIdentifier(root, tokens, "class expected a className identifier");
            DequeueSymbol(root, tokens, "{");
            DequeueClassVariableDeclarations(root, tokens);
            DequeueSymbol(root, tokens, "}");
            return root;
        }

        private void DequeueClassVariableDeclarations(ClassNode root, Queue<Token> tokens)
        {
            while ((Peek(tokens)?.Value) == "static" || (Peek(tokens)?.Value) == "field")
            {
                DequeueClassVariableDeclaration(root, tokens);
            }
        }

        private void DequeueClassVariableDeclaration(ClassNode root, Queue<Token> tokens)
        {
            var cvd = new ClassVariableDeclaration();
            root.Children.Add(cvd);
            DequeueKeyword(cvd, tokens);
            DequeueType(cvd, tokens);
            bool another;
            do
            {
                DequeueIdentifier(cvd, tokens, "class variable declaration expected a variable name");
                another = Peek(tokens)?.Value == ",";
                if (another) cvd.Children.Add(tokens.Dequeue());
            } while (another);
            DequeueSymbol(cvd, tokens, ";");
        }

        private void DequeueType(BranchNode node, Queue<Token> tokens)
        {
            Token type = Dequeue(tokens);
            if (type != null)
                node.Children.Add(type);
            else
                throw new ApplicationException("class variable definition expected a type");
        }

        private void DequeueKeyword(BranchNode node, Queue<Token> tokens)
        {
            node.Children.Add(tokens.Dequeue());
        }

        private Token DequeueIdentifier(BranchNode node, Queue<Token> tokens, string error)
        {
            Token identifier = Dequeue(tokens);
            if (identifier is IdentifierToken)
                node.Children.Add(identifier);
            else
                throw new ApplicationException(error);
            return identifier;
        }

        private Token DequeueSymbol(BranchNode node, Queue<Token> tokens, string symbol)
        {
            Token symbolToken = Dequeue(tokens);
            if (symbolToken is SymbolToken && symbolToken.Value == symbol)
                node.Children.Add(symbolToken);
            else
                throw new ApplicationException($"expected symbol '{symbol}'");
            return symbolToken;
        }

        private Token Dequeue(Queue<Token> tokens)
        {
            return tokens.Count > 0 ? tokens.Dequeue() : null;
        }

        private Token Peek(Queue<Token> tokens)
        {
            return tokens.Count > 0 ? tokens.Peek() : null;
        }
    }
}
