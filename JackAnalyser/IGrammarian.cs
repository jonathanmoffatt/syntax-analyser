using System.Collections.Generic;

namespace JackAnalyser
{
    public interface IGrammarian
    {
        Node Get(Queue<Token> tokens);
    }
}
