using System.Collections.Generic;

namespace JackAnalyser
{
    public abstract class BranchNode : Node
    {
        public List<Node> Children { get; } = new List<Node>();
    }
}
