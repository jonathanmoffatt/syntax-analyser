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
        public void RecognisesAKeywordToken()
        {
            using var t = new Tokeniser("class");
            t.GetNextToken().Should().BeOfType<KeywordToken>();
        }

        [TestMethod]
        public void RecognisesTheClassKeyword()
        {
            using var t = new Tokeniser("class");
            t.GetNextToken().Value.Should().Be("class");
        }

        [TestMethod]
        public void RecognisesAllKeywords()
        {
            using var t = new Tokeniser("class constructor function method field static var int char boolean void true false null this let do if else while return");
            Token token = t.GetNextToken();
            while (token != null)
            {
                token.Should().BeOfType<KeywordToken>();
                token = t.GetNextToken();
            }
        }

        [TestMethod]
        public void RecognisesAllSymbols()
        {
            using var t = new Tokeniser("{ } ( ) [ ] . , ; + - * / & | < > = ~");
            Token token = t.GetNextToken();
            while (token != null)
            {
                token.Should().BeOfType<SymbolToken>();
                token = t.GetNextToken();
            }
        }

        [TestMethod]
        public void RecognisesIntegerConstants()
        {
            using var t = new Tokeniser("12345");
            Token token = t.GetNextToken();
            token.Should().BeOfType<IntegerConstantToken>();
            token.Value.Should().Be("12345");
        }

        [TestMethod]
        public void RejectsIntegersContainingStrings()
        {
            using var t = new Tokeniser("123four5");
            Token token = t.GetNextToken();
            token.Should().NotBeOfType<IntegerConstantToken>();
        }

        [TestMethod]
        public void RecognisesStringConstants()
        {
            using var t = new Tokeniser("\"the quick brown fox, jumps over 23 ______ (of something)\"");
            Token token = t.GetNextToken();
            token.Should().BeOfType<StringConstantToken>();
            token.Value.Should().Be("the quick brown fox, jumps over 23 ______ (of something)");
        }

        [TestMethod]
        public void WalksThroughTheInput()
        {
            using var t = new Tokeniser("static field let");
            t.GetNextToken().Value.Should().Be("static");
            t.GetNextToken().Value.Should().Be("field");
            t.GetNextToken().Value.Should().Be("let");
        }

        [TestMethod]
        public void SkipsOverWhitespace()
        {
            using var t = new Tokeniser("    static     field\nlet   \r\n\tclass");
            t.GetNextToken().Value.Should().Be("static");
            t.GetNextToken().Value.Should().Be("field");
            t.GetNextToken().Value.Should().Be("let");
            t.GetNextToken().Value.Should().Be("class");
        }

        [TestMethod]
        public void NextTokenIsNullIfAtEnd()
        {
            using var t = new Tokeniser("class");
            t.GetNextToken();
            t.GetNextToken().Should().BeNull();
        }

        [TestMethod]
        public void IgnoresWhitespaceWhenCheckingIfAtEnd()
        {
            using var t = new Tokeniser("class\n\t\t  \n ");
            t.GetNextToken();
            t.GetNextToken().Should().BeNull();
        }

        [TestMethod]
        public void IsImmediatelyAtEndIfInputIsEmpty()
        {
            using var t = new Tokeniser("");
            t.GetNextToken().Should().BeNull();
        }

        [TestMethod]
        public void IsImmediatelyAtEndIfInputIsAllWhitespace()
        {
            using var t = new Tokeniser("    \n   \t  \r\n   ");
            t.GetNextToken().Should().BeNull();
        }

        [TestMethod]
        public void WorksWhenPassedAStream()
        {
            byte[] bytes = Encoding.UTF8.GetBytes("static field\n");
            using var stream = new MemoryStream(bytes);
            using var t = new Tokeniser(stream);
            t.GetNextToken().Value.Should().Be("static");
            t.GetNextToken().Value.Should().Be("field");
            t.GetNextToken().Should().BeNull();
        }
    }

    #endregion
}
