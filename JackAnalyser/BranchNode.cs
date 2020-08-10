using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace JackAnalyser
{
    public class BranchNode : Node
    {
        private List<Node> children = new List<Node>();

        public IEnumerable<Node> Children => children;

        public BranchNode(NodeType type)
        {
            Type = type;
        }

        public T AddChild<T>(T node) where T : Node
        {
            if (node != null)
                children.Add(node);
            return node;
        }

        public override XElement ToXml()
        {
            string elementName = Type.GetAttribute<ElementNameAttribute, NodeType>().Name;
            return new XElement(elementName, Children.Select(c => c.ToXml()));
        }
    }
}
