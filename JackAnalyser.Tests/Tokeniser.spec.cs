using System.IO;
using System.Text;
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
        public void NextTokenIsNullIfAtEnd()
        {
            var t = new Tokeniser("class");
            t.GetNextToken();
            t.GetNextToken().Should().BeNull();
        }

        [TestMethod]
        public void IgnoresWhitespaceWhenCheckingIfAtEnd()
        {
            var t = new Tokeniser("class\n\t\t  \n ");
            t.GetNextToken();
            t.GetNextToken().Should().BeNull();
        }

        [TestMethod]
        public void IsImmediatelyAtEndIfInputIsEmpty()
        {
            new Tokeniser("").GetNextToken().Should().BeNull();
        }

        [TestMethod]
        public void IsImmediatelyAtEndIfInputIsAllWhitespace()
        {
            new Tokeniser("    \n   \t  \r\n   ").GetNextToken().Should().BeNull();
        }

        [TestMethod]
        public void WorksWhenPassedAStream()
        {
            byte[] bytes = Encoding.UTF8.GetBytes("static field\n");
            using var stream = new MemoryStream(bytes);
            var t = new Tokeniser(stream);
            t.GetNextToken().Value.Should().Be("static");
            t.GetNextToken().Value.Should().Be("field");
            t.GetNextToken().Should().BeNull();
        }
    }

    #endregion
}
