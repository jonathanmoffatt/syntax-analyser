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
        public void ShouldReturnAnXmlElement()
        {
            XElement xElement = new SymbolToken(")").ToXml();
            xElement.Name.ToString().Should().Be("symbol");
        }

        [TestMethod]
        public void ShouldPopulateXmlElementWithTheValueOfTheToken()
        {
            new SymbolToken(")").ToXml().Value.Should().Be(")");
        }

        [TestMethod]
        public void ShouldSetElementNameBasedOnTheTypeOfToken()
        {
            new KeywordToken("let").ToXml().Name.ToString().Should().Be("keyword");
            new StringConstantToken("hello").ToXml().Name.ToString().Should().Be("stringConstant");
            new IntegerConstantToken("123").ToXml().Name.ToString().Should().Be("integerConstant");
            new IdentifierToken("counter").ToXml().Name.ToString().Should().Be("identifier");
        }
    }

    #endregion
}
