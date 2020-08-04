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
            XElement xElement = new SymbolToken(")").ToXml();
            xElement.Name.ToString().Should().Be("symbol");
        }

        [TestMethod]
        public void PopulatesXmlElementWithTheValueOfTheTokenWithPadding()
        {
            new SymbolToken(")").ToXml().Value.Should().Be(" ) ");
        }

        [TestMethod]
        public void SetsElementNameBasedOnTheTypeOfToken()
        {
            new KeywordToken("let").ToXml().Name.ToString().Should().Be("keyword");
            new StringConstantToken("hello").ToXml().Name.ToString().Should().Be("stringConstant");
            new IntegerConstantToken("123").ToXml().Name.ToString().Should().Be("integerConstant");
            new IdentifierToken("counter").ToXml().Name.ToString().Should().Be("identifier");
        }
    }

    #endregion

    #region WhenGettingABranchNodeAsXml

    [TestClass]
    public class WhenGettingABranchNodeAsXml
    {
        [TestMethod]
        public void ReturnsEmptyElementIfThereAreNoChildren()
        {
            new StatementsNode().ToXml().ToString().Should().Be("<statements />");
        }

        [TestMethod]
        public void NestsChildrenUnderTheElement()
        {
            var root = new ClassNode();
            root.Children.Add(new KeywordToken("class"));
            root.Children.Add(new IdentifierToken("Game"));
            root.Children.Add(new SymbolToken("{"));
            var variables = new ClassVariableDeclarationNode();
            root.Children.Add(variables);
            variables.Children.Add(new KeywordToken("field"));
            variables.Children.Add(new KeywordToken("int"));
            variables.Children.Add(new IdentifierToken("direction"));
            variables.Children.Add(new SymbolToken(";"));
            root.ToXml().ToString().Should().Be(
@"<class>
  <keyword> class </keyword>
  <identifier> Game </identifier>
  <symbol> { </symbol>
  <classVarDec>
    <keyword> field </keyword>
    <keyword> int </keyword>
    <identifier> direction </identifier>
    <symbol> ; </symbol>
  </classVarDec>
</class>"
);
        }
    }

    #endregion
}
