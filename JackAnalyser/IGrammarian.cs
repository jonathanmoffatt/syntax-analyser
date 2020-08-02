using System.Collections.Generic;

namespace JackAnalyser
{
    public interface IGrammarian
    {
        BranchNode Get(Queue<Token> tokens);
    }
}
