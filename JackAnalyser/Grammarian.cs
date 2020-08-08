using System;
using System.Collections.Generic;

namespace JackAnalyser
{
    public class Grammarian : IGrammarian
    {
        private Queue<Token> tokens;

        public BranchNode Get(params Token[] tokens)
        {
            return Get(new Queue<Token>(tokens));
        }

        public BranchNode Get(Queue<Token> tokens)
        {
            this.tokens = tokens;
            ClassNode root = new ClassNode();
            root.Children.Add(Dequeue());
            DequeueIdentifier(root, "class expected a className identifier");
            DequeueSymbol(root, "{");
            DequeueClassVariableDeclarations(root);
            DequeueSubroutineDeclarations(root);
            DequeueSymbol(root, "}");
            return root;
        }

        private void DequeueClassVariableDeclarations(ClassNode root)
        {
            while (Peek() == "static" || Peek() == "field")
            {
                DequeueClassVariableDeclaration(root);
            }
        }

        private void DequeueClassVariableDeclaration(ClassNode root)
        {
            var cvd = new ClassVariableDeclarationNode();
            root.Children.Add(cvd);
            DequeueKeyword(cvd);
            DequeueType(cvd);
            bool another;
            do
            {
                DequeueIdentifier(cvd, "class variable declaration expected a variable name");
                another = Peek() == ",";
                if (another) cvd.Children.Add(tokens.Dequeue());
            } while (another);
            DequeueSymbol(cvd, ";");
        }

        private void DequeueSubroutineDeclarations(ClassNode root)
        {
            while ((Peek()) == "constructor" || (Peek()) == "function" || (Peek()) == "method")
            {
                DequeueSubroutineDeclaration(root);
            }
        }

        private void DequeueSubroutineDeclaration(ClassNode root)
        {
            var sd = new SubroutineDeclarationNode();
            root.Children.Add(sd);
            DequeueKeyword(sd);
            DequeueType(sd);
            DequeueIdentifier(sd, "expected subroutine name");
            DequeueSymbol(sd, "(");
            if (Peek() != ")")
                DequeueParameterList(sd);
            DequeueSymbol(sd, ")");
            DequeueSubroutineBody(sd);
        }

        private void DequeueParameterList(SubroutineDeclarationNode sd)
        {
            var pl = new ParameterListNode();
            sd.Children.Add(pl);
            bool another;
            do
            {
                DequeueType(pl);
                DequeueIdentifier(pl, "expected parameter list identifier");
                another = Peek() == ",";
                if (another) DequeueSymbol(pl, ",");
            } while (another);
        }

        private void DequeueSubroutineBody(SubroutineDeclarationNode sd)
        {
            var body = new SubroutineBodyNode();
            sd.Children.Add(body);
            DequeueSymbol(body, "{");
            DequeueVariableDeclarations(body);
            DequeueStatements(body);
            DequeueSymbol(body, "}");
        }

        private void DequeueVariableDeclarations(SubroutineBodyNode body)
        {
            while (Peek() == "var")
            {
                var variables = new VariableDeclarationNode();
                body.Children.Add(variables);
                DequeueKeyword(variables);
                DequeueType(variables);
                bool more;
                do
                {
                    DequeueIdentifier(variables, "variable declarations expected an identifier");
                    more = Peek() == ",";
                    if (more) DequeueSymbol(variables, ",");
                } while (more);
                DequeueSymbol(variables, ";");
            }
        }

        private void DequeueStatements(SubroutineBodyNode body)
        {
            if (Peek() != "}")
            {
                var statements = new StatementsNode();
                body.Children.Add(statements);
                while (Peek() != "}")
                {
                    DequeueLetStatement(statements);
                }
            }
        }

        private void DequeueLetStatement(StatementsNode parentNode)
        {
            if (Peek() == "let")
            {
                var statement = new LetStatementNode();
                parentNode.Children.Add(statement);
                DequeueKeyword(statement);
                DequeueIdentifier(statement, "let statement expected an identifier");
                DequeueSymbol(statement, "=");
                DequeueExpression(statement, "let statement expected an expression");
                DequeueSymbol(statement, ";");
            }
        }

        private void DequeueExpression(BranchNode parentNode, string error)
        {
            parentNode.Children.Add(Dequeue());
        }

        private void DequeueType(BranchNode parentNode)
        {
            Token type = Dequeue();
            if (type != null)
                parentNode.Children.Add(type);
            else
                throw new ApplicationException("class variable definition expected a type, reached end of file instead");
        }

        private void DequeueKeyword(BranchNode parentNode)
        {
            parentNode.Children.Add(tokens.Dequeue());
        }

        private Token DequeueIdentifier(BranchNode parentNode, string error)
        {
            Token identifier = Dequeue();
            if (identifier is IdentifierToken)
                parentNode.Children.Add(identifier);
            else
            {
                string suffix = identifier == null ? ", reached end of file instead" : $", got '{identifier}' instead";
                throw new ApplicationException(error + suffix);
            }
            return identifier;
        }

        private Token DequeueSymbol(BranchNode parentNode, string symbol)
        {
            Token token = Dequeue();
            if (token is SymbolToken && token.Value == symbol)
                parentNode.Children.Add(token);
            else
            {
                string suffix = token == null ? "reached end of file instead" : $"got '{token}' instead";
                throw new ApplicationException($"expected symbol '{symbol}', {suffix}");
            }
            return token;
        }

        private Token Dequeue()
        {
            return tokens.Count > 0 ? tokens.Dequeue() : null;
        }

        private string Peek()
        {
            return tokens.Count > 0 ? tokens.Peek().Value : null;
        }
    }
}
