using System.IO;
using System.Text;
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
        public void IdentifiesAllKeywords()
        {
            var t = new Tokeniser("class constructor function method field static var int char boolean void true false null this let do if else while return");
            Token token = t.GetNextToken();
            while (token != null)
            {
                token.Should().BeOfType<KeywordToken>();
                token = t.GetNextToken();
            }
        }

        [TestMethod]
        public void IdentifiesAllSymbols()
        {
            var t = new Tokeniser("{ } ( ) [ ] . , ; + - * / & | < > = ~");
            Token token = t.GetNextToken();
            while (token != null)
            {
                token.Should().BeOfType<SymbolToken>();
                token = t.GetNextToken();
            }
        }

        [TestMethod]
        public void IdentifiesIntegerConstants()
        {
            Token token = new Tokeniser("12345").GetNextToken();
            token.Should().BeOfType<IntegerConstantToken>();
            token.Value.Should().Be("12345");
        }

        [TestMethod]
        public void RejectsIntegersContainingStrings()
        {
            Token token = new Tokeniser("123four5").GetNextToken();
            token.Should().NotBeOfType<IntegerConstantToken>();
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
