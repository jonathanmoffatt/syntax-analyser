using System.Collections.Generic;

namespace JackAnalyser
{
    public abstract class BranchNode : Node
    {
        private List<Node> children = new List<Node>();

        public IEnumerable<Node> Children => children;

        public T AddChild<T>(T node) where T : Node
        {
            if (node != null)
                children.Add(node);
            return node;
        }
    }
}
