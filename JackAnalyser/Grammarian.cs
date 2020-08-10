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

        public BranchNode ParseClass()
        {
            BranchNode root = new BranchNode(NodeType.Class);
            root.AddChild(Dequeue());
            DequeueIdentifier(root, "class expected a className identifier");
            DequeueSymbol(root, "{");
            while (Peek() == "static" || Peek() == "field")
            {
                root.AddChild(ParseClassVariableDeclaration());
            }
            while (Peek() == "constructor" || Peek() == "function" || Peek() == "method")
            {
                root.AddChild(ParseSubroutineDeclaration());
            }
            DequeueSymbol(root, "}");
            return root;
        }

        public BranchNode ParseClassVariableDeclaration()
        {
            var cvd = new BranchNode(NodeType.ClassVariableDeclaration);
            DequeueKeyword(cvd);
            DequeueType(cvd);
            bool another;
            do
            {
                DequeueIdentifier(cvd, "class variable declaration expected a variable name");
                another = Peek() == ",";
                if (another) cvd.AddChild(tokens.Dequeue());
            } while (another);
            DequeueSymbol(cvd, ";");
            return cvd;
        }

        public BranchNode ParseSubroutineDeclaration()
        {
            var sd = new BranchNode(NodeType.SubroutineDeclaration);
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

        public BranchNode ParseLetStatement()
        {
            if (Peek() != "let") return null;
            var statement = new BranchNode(NodeType.LetStatement);
            DequeueKeyword(statement);
            DequeueIdentifier(statement, "let statement expected an identifier");
            DequeueSymbol(statement, "=");
            DequeueExpression(statement, "let statement expected an expression");
            DequeueSymbol(statement, ";");
            return statement;
        }

        public BranchNode ParseIfStatement()
        {
            if (Peek() != "if") return null;
            var statement = new BranchNode(NodeType.IfStatement);
            DequeueKeyword(statement);
            DequeueSymbol(statement, "(");
            DequeueExpression(statement, "if statement expected an expression");
            DequeueSymbol(statement, ")");
            DequeueSymbol(statement, "{");
            DequeueStatements(statement);
            DequeueSymbol(statement, "}");
            if (Peek() == "else")
            {
                DequeueKeyword(statement);
                DequeueSymbol(statement, "{");
                DequeueStatements(statement);
                DequeueSymbol(statement, "}");
            }
            return statement;
        }

        public BranchNode ParseReturnStatement()
        {
            if (Peek() != "return") return null;
            var statement = new BranchNode(NodeType.ReturnStatement);
            DequeueKeyword(statement);
            if (Peek() != ";")
                DequeueExpression(statement, "return statement expected an expression");
            DequeueSymbol(statement, ";");
            return statement;
        }

        public BranchNode ParseWhileStatement()
        {
            if (Peek() != "while") return null;
            var statement = new BranchNode(NodeType.WhileStatement);
            DequeueKeyword(statement);
            DequeueSymbol(statement, "(");
            DequeueExpression(statement, "while statement expected an expression");
            DequeueSymbol(statement, ")");
            DequeueSymbol(statement, "{");
            DequeueStatements(statement);
            DequeueSymbol(statement, "}");
            return statement;
        }

        private void DequeueParameterList(BranchNode parent)
        {
            var pl = parent.AddChild(new BranchNode(NodeType.ParameterList));
            bool another;
            do
            {
                DequeueType(pl);
                DequeueIdentifier(pl, "expected parameter list identifier");
                another = Peek() == ",";
                if (another) DequeueSymbol(pl, ",");
            } while (another);
        }

        private void DequeueSubroutineBody(BranchNode parent)
        {
            var body = parent.AddChild(new BranchNode(NodeType.SubroutineBody));
            DequeueSymbol(body, "{");
            DequeueVariableDeclarations(body);
            DequeueStatements(body);
            DequeueSymbol(body, "}");
        }

        private void DequeueVariableDeclarations(BranchNode body)
        {
            while (Peek() == "var")
            {
                var variables = body.AddChild(new BranchNode(NodeType.VariableDeclaration));
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

        private void DequeueStatements(BranchNode parent)
        {
            if (Peek() != "}")
            {
                var statements = parent.AddChild(new BranchNode(NodeType.Statements));
                while (Peek() != "}")
                {
                    statements.AddChild(ParseLetStatement());
                    statements.AddChild(ParseIfStatement());
                    statements.AddChild(ParseReturnStatement());
                    statements.AddChild(ParseWhileStatement());
                }
            }
        }

        private void DequeueExpression(BranchNode parent, string error)
        {
            var expression = parent.AddChild(new BranchNode(NodeType.Expression));
            DequeueTerm(expression);
            while (IsOperator(Peek()))
            {
                DequeueSymbol(expression, Peek());
                DequeueTerm(expression);
            }
        }

        private void DequeueTerm(BranchNode parent)
        {
            var term = parent.AddChild(new BranchNode(NodeType.Term));
            if (IsUnaryOperator(Peek()))
            {
                DequeueSymbol(term, Peek());
                DequeueTerm(term);
            }
            else if (Peek() == "(")
            {
                DequeueSymbol(term, "(");
                DequeueExpression(term, "term expected expression after '('");
                DequeueSymbol(term, ")");
            }
            else
            {
                term.AddChild(Dequeue());
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
                parent.AddChild(type);
            else
                throw new ApplicationException("class variable definition expected a type, reached end of file instead");
        }

        private void DequeueKeyword(BranchNode parent)
        {
            parent.AddChild(tokens.Dequeue());
        }

        private Token DequeueIdentifier(BranchNode parent, string error)
        {
            Token identifier = Dequeue();
            if (identifier?.Type == NodeType.Identifier)
            {
                parent.AddChild(identifier);
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
            if (token == null || token.Type != NodeType.Symbol || token.Value != symbol)
            {
                string suffix = token == null ? "reached end of file instead" : $"got '{token}' instead";
                throw new ApplicationException($"expected symbol '{symbol}', {suffix}");
            }
            return parent.AddChild(token);
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
