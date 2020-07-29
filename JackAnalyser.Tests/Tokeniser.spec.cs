using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JackAnalyser.Tests
{
    #region WhenWorkingWithTheTokeniser

    [TestClass]
    public class WhenWorkingWithTheTokeniser
    {

        [TestInitialize]
        public void Setup()
        {

        }

        [TestMethod]
        public void IdentifiesAKeywordToken()
        {
            Token t = new Tokeniser("class").GetNextToken();
            t.Should().BeOfType<KeywordToken>();
        }

        [TestMethod]
        public void IdentifiesTheClassKeyword()
        {
            new Tokeniser("class").GetNextToken().Value.Should().Be("class");
        }

        [TestMethod]
        public void WalksThroughTheInput()
        {
            var t = new Tokeniser("static field let");
            t.GetNextToken().Value.Should().Be("static");
            t.GetNextToken().Value.Should().Be("field");
            t.GetNextToken().Value.Should().Be("let");
        }

        [TestMethod]
        public void SkipsOverWhitespace()
        {
            var t = new Tokeniser("    static     field\nlet   \r\n\tclass");
            t.GetNextToken().Value.Should().Be("static");
            t.GetNextToken().Value.Should().Be("field");
            t.GetNextToken().Value.Should().Be("let");
            t.GetNextToken().Value.Should().Be("class");
        }

        [TestMethod]
        public void ChecksWhetherAtEnd()
        {
            var t = new Tokeniser("class");
            t.AtEnd().Should().BeFalse();
            t.GetNextToken();
            t.AtEnd().Should().BeTrue();
        }

        [TestMethod]
        public void IgnoresWhitespaceWhenCheckingIfAtEnd()
        {
            var t = new Tokeniser("   class\n\t   ");
            t.AtEnd().Should().BeFalse();
            t.GetNextToken();
            t.AtEnd().Should().BeTrue();
        }

        [TestMethod]
        public void IsImmediatelyAtEndIfInputIsEmpty()
        {
            new Tokeniser("").AtEnd().Should().BeTrue();
        }

        [TestMethod]
        public void NextTokenIsNullIfAtEnd()
        {
            new Tokeniser("").GetNextToken().Should().BeNull();
        }
    }

    #endregion
}
