using System.Collections.Generic;

namespace JackAnalyser
{
    public interface IGrammarian
    {
        IGrammarian LoadTokens(Queue<Token> tokens);
        BranchNode ParseClass();
        BranchNode ParseClassVariableDeclaration();
        BranchNode ParseReturnStatement();
        BranchNode ParseSubroutineDeclaration();
    }
}
