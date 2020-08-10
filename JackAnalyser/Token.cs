using System.Xml.Linq;

namespace JackAnalyser
{
    public class Token : Node
    {
        public string Value { get; private set; }

        public Token(NodeType type, string tokenValue)
        {
            Type = type;
            Value = tokenValue;
        }

        public override string ToString()
        {
            return Value;
        }

        public override XElement ToXml()
        {
            string elementName = Type.GetAttribute<ElementNameAttribute, NodeType>().Name;
            return new XElement(elementName, Value);
        }
    }
}
