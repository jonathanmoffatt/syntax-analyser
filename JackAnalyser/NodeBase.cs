using System.Xml.Linq;

namespace JackAnalyser
{
    public abstract class NodeBase
    {
        public NodeType Type { get; protected set; }

        public abstract XElement ToXml();
    }
}
