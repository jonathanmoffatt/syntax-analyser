using System.Xml.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JackAnalyser.Tests
{
    #region WhenGettingATokenAsXml

    [TestClass]
    public class WhenGettingATokenAsXml
    {
        [TestMethod]
        public void ReturnsAnXmlElement()
        {
            XElement xElement = new Token(NodeType.Symbol, ")").ToXml();
            xElement.Name.ToString().Should().Be("symbol");
        }

        [TestMethod]
        public void SetsValueXmlElementToValueOfToken()
        {
            new Token(NodeType.Symbol, ")").ToXml().Value.Should().Be(")");
        }

        [TestMethod]
        public void SetsElementNameBasedOnTheTypeOfToken()
        {
            new Token(NodeType.Keyword, "let").ToXml().Name.ToString().Should().Be("keyword");
            new Token(NodeType.StringConstant, "hello").ToXml().Name.ToString().Should().Be("stringConstant");
            new Token(NodeType.IntegerConstant, "123").ToXml().Name.ToString().Should().Be("integerConstant");
            new Token(NodeType.Identifier, "counter").ToXml().Name.ToString().Should().Be("identifier");
        }
    }

    #endregion

    #region WhenGettingABranchNodeAsXml

    [TestClass]
    public class WhenGettingABranchNodeAsXml
    {
        [TestMethod]
        public void DoesNotSelfCloseElementIfThereAreNoChildren()
        {
            new Node(NodeType.Statements).ToXml().ToString().Should().Be("<statements>\n</statements>");
        }

        [TestMethod]
        public void NestsChildrenUnderTheElement()
        {
            var root = new Node(NodeType.Class);
            root.AddChild(new Token(NodeType.Keyword, "class"));
            root.AddChild(new Token(NodeType.Identifier, "Game"));
            root.AddChild(new Token(NodeType.Symbol, "{"));
            var variables = new Node(NodeType.ClassVariableDeclaration);
            root.AddChild(variables);
            variables.AddChild(new Token(NodeType.Keyword, "field"));
            variables.AddChild(new Token(NodeType.Keyword, "int"));
            variables.AddChild(new Token(NodeType.Identifier, "direction"));
            variables.AddChild(new Token(NodeType.Symbol, ";"));
            root.ToXml().ToString().Should().Be(
@"<class>
  <keyword>class</keyword>
  <identifier>Game</identifier>
  <symbol>{</symbol>
  <classVarDec>
    <keyword>field</keyword>
    <keyword>int</keyword>
    <identifier>direction</identifier>
    <symbol>;</symbol>
  </classVarDec>
</class>"
);
        }
    }

    #endregion
}
