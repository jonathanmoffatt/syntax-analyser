using System;
using System.Collections.Generic;

namespace JackAnalyser
{
    public class Grammarian : IGrammarian
    {
        private Queue<Token> tokens;

        public IGrammarian LoadTokens(params Token[] tokens)
        {
            this.tokens = new Queue<Token>(tokens);
            return this;
        }

        public IGrammarian LoadTokens(Queue<Token> tokens)
        {
            this.tokens = tokens;
            return this;
        }

        public ClassNode ParseClass()
        {
            ClassNode root = new ClassNode();
            root.Children.Add(Dequeue());
            DequeueIdentifier(root, "class expected a className identifier");
            DequeueSymbol(root, "{");
            while (Peek() == "static" || Peek() == "field")
            {
                root.Children.Add(ParseClassVariableDeclaration());
            }
            while (Peek() == "constructor" || Peek() == "function" || Peek() == "method")
            {
                root.Children.Add(ParseSubroutineDeclaration());
            }
            DequeueSymbol(root, "}");
            return root;
        }

        public ClassVariableDeclarationNode ParseClassVariableDeclaration()
        {
            var cvd = new ClassVariableDeclarationNode();
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
            return cvd;
        }

        public SubroutineDeclarationNode ParseSubroutineDeclaration()
        {
            var sd = new SubroutineDeclarationNode();
            DequeueKeyword(sd);
            DequeueType(sd);
            DequeueIdentifier(sd, "expected subroutine name");
            DequeueSymbol(sd, "(");
            if (Peek() != ")")
                DequeueParameterList(sd);
            DequeueSymbol(sd, ")");
            DequeueSubroutineBody(sd);
            return sd;
        }

        public LetStatementNode ParseLetStatement()
        {
            LetStatementNode statement = null;
            if (Peek() == "let")
            {
                statement = new LetStatementNode();
                DequeueKeyword(statement);
                DequeueIdentifier(statement, "let statement expected an identifier");
                DequeueSymbol(statement, "=");
                DequeueExpression(statement, "let statement expected an expression");
                DequeueSymbol(statement, ";");
            }
            return statement;
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
                    LetStatementNode let = ParseLetStatement();
                    if (let != null)
                        statements.Children.Add(let);
                }
            }
        }

        private void DequeueExpression(BranchNode parent, string error)
        {
            var expression = new ExpressionNode();
            parent.Children.Add(expression);
            DequeueTerm(expression);
            while(IsOperator(Peek()))
            {
                DequeueSymbol(expression, Peek());
                DequeueTerm(expression);
            }
        }

        private void DequeueTerm(BranchNode parent)
        { 
            var term = new TermNode();
            parent.Children.Add(term);
            if (IsUnaryOperator(Peek()))
            {
                DequeueSymbol(term, Peek());
                DequeueTerm(term);
            }
            else
            {
                term.Children.Add(Dequeue());
                if (Peek() == "[")
                {
                    DequeueSymbol(term, "[");
                    DequeueExpression(term, "term expected expression");
                    DequeueSymbol(term, "]");
                }
            }
        }

        private void DequeueType(BranchNode parent)
        {
            Token type = Dequeue();
            if (type != null)
                parent.Children.Add(type);
            else
                throw new ApplicationException("class variable definition expected a type, reached end of file instead");
        }

        private void DequeueKeyword(BranchNode parent)
        {
            parent.Children.Add(tokens.Dequeue());
        }

        private Token DequeueIdentifier(BranchNode parent, string error)
        {
            Token identifier = Dequeue();
            if (identifier is IdentifierToken)
            {
                parent.Children.Add(identifier);
                if (Peek() == "[")
                {
                    DequeueSymbol(parent, "[");
                    DequeueExpression(parent, "expected expression");
                    DequeueSymbol(parent, "]");
                }
            }
            else
            {
                string suffix = identifier == null ? ", reached end of file instead" : $", got '{identifier}' instead";
                throw new ApplicationException(error + suffix);
            }
            return identifier;
        }

        private Token DequeueSymbol(BranchNode parent, string symbol)
        {
            Token token = Dequeue();
            if (token is SymbolToken && token.Value == symbol)
                parent.Children.Add(token);
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

        private bool IsOperator(string s)
        {
            return s != null && "+-*/&|<>=".Contains(s);
        }

        private bool IsUnaryOperator(string s)
        {
            return s != null && (s == "-" || s == "~");
        }
    }
}
