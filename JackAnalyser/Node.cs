using System.Linq;
using System.Xml.Linq;

namespace JackAnalyser
{
    public abstract class Node
    {
        public NodeType Type { get; protected set; }

        public abstract XElement ToXml();
    }
}
