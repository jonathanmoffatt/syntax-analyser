using System.Linq;
using System.Xml.Linq;

namespace JackAnalyser
{
    public abstract class Node
    {
        protected abstract string ElementName { get; }

        public XElement ToXml()
        {
            if (this is Token t)
                return new XElement(ElementName, $" {t.Value} ");
            if (this is BranchNode b)
                return new XElement(ElementName, b.Children.Select(c => c.ToXml()));
            return null;
        }
    }
}
