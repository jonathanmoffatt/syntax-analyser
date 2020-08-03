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
            DequeueSubroutineDeclarations(root, tokens);
            DequeueSymbol(root, tokens, "}");
            return root;
        }

        private void DequeueClassVariableDeclarations(ClassNode root, Queue<Token> tokens)
        {
            while (Peek(tokens)?.Value == "static" || Peek(tokens)?.Value == "field")
            {
                DequeueClassVariableDeclaration(root, tokens);
            }
        }

        private void DequeueClassVariableDeclaration(ClassNode root, Queue<Token> tokens)
        {
            var cvd = new ClassVariableDeclarationNode();
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

        private void DequeueSubroutineDeclarations(ClassNode root, Queue<Token> tokens)
        {
            while ((Peek(tokens)?.Value) == "constructor" || (Peek(tokens)?.Value) == "function" || (Peek(tokens)?.Value) == "method")
            {
                DequeueSubroutineDeclaration(root, tokens);
            }
        }

        private void DequeueSubroutineDeclaration(ClassNode root, Queue<Token> tokens)
        {
            var sd = new SubroutineDeclarationNode();
            root.Children.Add(sd);
            DequeueKeyword(sd, tokens);
            DequeueType(sd, tokens);
            DequeueIdentifier(sd, tokens, "expected subroutine name");
            DequeueSymbol(sd, tokens, "(");
            if (Peek(tokens)?.Value != ")")
                DequeueParameterList(sd, tokens);
            DequeueSymbol(sd, tokens, ")");
            DequeueSubroutineBody(sd, tokens);
        }

        private void DequeueParameterList(SubroutineDeclarationNode sd, Queue<Token> tokens)
        {
            var pl = new ParameterListNode();
            sd.Children.Add(pl);
            bool another;
            do
            {
                DequeueType(pl, tokens);
                DequeueIdentifier(pl, tokens, "expected parameter list identifier");
                another = Peek(tokens)?.Value == ",";
                if (another) DequeueSymbol(pl, tokens, ",");
            } while (another);
        }

        private void DequeueSubroutineBody(SubroutineDeclarationNode sd, Queue<Token> tokens)
        {
            var body = new SubroutineBodyNode();
            sd.Children.Add(body);
            DequeueSymbol(body, tokens, "{");
            DequeueVariableDeclarations(body, tokens);
            DequeueSymbol(body, tokens, "}");
        }

        private void DequeueVariableDeclarations(SubroutineBodyNode body, Queue<Token> tokens)
        {
            while (Peek(tokens)?.Value == "var")
            {
                var variables = new VariableDeclarationNode();
                body.Children.Add(variables);
                DequeueKeyword(variables, tokens);
                DequeueType(variables, tokens);
                bool more;
                do
                {
                    DequeueIdentifier(variables, tokens, "variable declarations expected an identifier");
                    more = Peek(tokens)?.Value == ",";
                    if (more) DequeueSymbol(variables, tokens, ",");
                } while (more);
                DequeueSymbol(variables, tokens, ";");
            }
        }

        private void DequeueType(BranchNode parentNode, Queue<Token> tokens)
        {
            Token type = Dequeue(tokens);
            if (type != null)
                parentNode.Children.Add(type);
            else
                throw new ApplicationException("class variable definition expected a type");
        }

        private void DequeueKeyword(BranchNode parentNode, Queue<Token> tokens)
        {
            parentNode.Children.Add(tokens.Dequeue());
        }

        private Token DequeueIdentifier(BranchNode parentNode, Queue<Token> tokens, string error)
        {
            Token identifier = Dequeue(tokens);
            if (identifier is IdentifierToken)
                parentNode.Children.Add(identifier);
            else
            {
                string suffix = identifier == null ? ", reached end of file instead" : $", got {identifier} instead";
                throw new ApplicationException(error + suffix);
            }
            return identifier;
        }

        private Token DequeueSymbol(BranchNode parentNode, Queue<Token> tokens, string symbol)
        {
            Token symbolToken = Dequeue(tokens);
            if (symbolToken is SymbolToken && symbolToken.Value == symbol)
                parentNode.Children.Add(symbolToken);
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
