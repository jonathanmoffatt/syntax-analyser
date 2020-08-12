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

        public void AddChildren<T>(params T[] nodes) where T : NodeBase
        {
            foreach (var node in nodes)
                AddChild(node);
        }

        public override XElement ToXml()
        {
            string elementName = Type.GetAttribute<ElementNameAttribute, NodeType>().Name;
            if (Children.Any())
                return new XElement(elementName, Children.Select(c => c.ToXml()));
            // this is stupid, but necessary because the grader doesn't like self closing tags, and also
            // expects the closing tag to be on the next line.
            return new XElement(elementName, "\n");
        }
    }
}
