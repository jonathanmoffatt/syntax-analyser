using System.Collections.Generic;

namespace JackAnalyser
{
    public interface IGrammarian
    {
        IGrammarian LoadTokens(Queue<Token> tokens);
        ClassNode ParseClass();
        SubroutineDeclarationNode ParseSubroutineDeclaration();
    }
}
