using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace JackAnalyser
{
    public class Node : NodeBase
    {
        private List<NodeBase> children = new List<NodeBase>();

        public IEnumerable<NodeBase> Children => children;

        public Node(NodeType type)
        {
            Type = type;
        }

        public T AddChild<T>(T node) where T : NodeBase
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
