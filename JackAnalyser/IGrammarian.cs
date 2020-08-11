using System.Collections.Generic;

namespace JackAnalyser
{
    public interface IGrammarian
    {
        IGrammarian LoadTokens(Queue<Token> tokens);
        Node ParseClass();
        Node ParseClassVariableDeclaration();
        Node ParseExpression();
        Node ParseParameterList();
        Node ParseReturnStatement();
        Node ParseStatements();
        Node ParseSubroutineBody();
        Node ParseSubroutineDeclaration();
        Node ParseTerm();
        Node[] ParseVariableDeclarations();
    }
}
