using System;
using System.Collections.Generic;

namespace JackAnalyser
{
    public class Grammarian : IGrammarian
    {
        public BranchNode Get(Queue<Token> tokens)
        {
            return new ClassNode();
        }
    }
}
