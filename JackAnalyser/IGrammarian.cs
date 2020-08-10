using System.Collections.Generic;

namespace JackAnalyser
{
    public interface IGrammarian
    {
        IGrammarian LoadTokens(Queue<Token> tokens);
        Node ParseClass();
        Node ParseClassVariableDeclaration();
        Node ParseReturnStatement();
        Node ParseSubroutineDeclaration();
    }
}
