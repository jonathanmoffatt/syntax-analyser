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
                if (another) cvd.AddChild(tokens.Dequeue());
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
            if (Peek() != "let") return null;
            var statement = new LetStatementNode();
            DequeueKeyword(statement);
            DequeueIdentifier(statement, "let statement expected an identifier");
            DequeueSymbol(statement, "=");
            DequeueExpression(statement, "let statement expected an expression");
            DequeueSymbol(statement, ";");
            return statement;
        }

        public IfStatementNode ParseIfStatement()
        {
            if (Peek() != "if") return null;
            var statement = new IfStatementNode();
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

        public ReturnStatementNode ParseReturnStatement()
        {
            if (Peek() != "return") return null;
            var statement = new ReturnStatementNode();
            DequeueKeyword(statement);
            if (Peek() != ";")
                DequeueExpression(statement, "return statement expected an expression");
            DequeueSymbol(statement, ";");
            return statement;
        }

        public WhileStatementNode ParseWhileStatement()
        {
            if (Peek() != "while") return null;
            var statement = new WhileStatementNode();
            DequeueKeyword(statement);
            DequeueSymbol(statement, "(");
            DequeueExpression(statement, "while statement expected an expression");
            DequeueSymbol(statement, ")");
            DequeueSymbol(statement, "{");
            DequeueStatements(statement);
            DequeueSymbol(statement, "}");
            return statement;
        }

        private void DequeueParameterList(SubroutineDeclarationNode sd)
        {
            var pl = sd.AddChild(new ParameterListNode());
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
            var body = sd.AddChild(new SubroutineBodyNode());
            DequeueSymbol(body, "{");
            DequeueVariableDeclarations(body);
            DequeueStatements(body);
            DequeueSymbol(body, "}");
        }

        private void DequeueVariableDeclarations(SubroutineBodyNode body)
        {
            while (Peek() == "var")
            {
                var variables = body.AddChild(new VariableDeclarationNode());
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
                var statements = parent.AddChild(new StatementsNode());
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
            var expression = parent.AddChild(new ExpressionNode());
            DequeueTerm(expression);
            while (IsOperator(Peek()))
            {
                DequeueSymbol(expression, Peek());
                DequeueTerm(expression);
            }
        }

        private void DequeueTerm(BranchNode parent)
        {
            var term = parent.AddChild(new TermNode());
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
            if (identifier is IdentifierToken)
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
            if (token is SymbolToken && token.Value == symbol)
                parent.AddChild(token);
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
