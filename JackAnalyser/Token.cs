using System.Xml.Linq;

namespace JackAnalyser
{
    public abstract class Token : Node
    {
        public string Value { get; private set; }
        protected abstract string ElementName { get; }

        protected Token(string tokenValue)
        {
            Value = tokenValue;
        }

        public XElement ToXml()
        {
            return new XElement(ElementName, $" {Value} ");
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
