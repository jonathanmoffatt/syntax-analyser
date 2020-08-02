using System.Collections.Generic;

namespace JackAnalyser
{
    public interface INodeFactory
    {
        Node Get(Queue<Token> tokens);
    }
}
