using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;

namespace JackAnalyser.Tests
{
    #region WhenRunningTheParser

    [TestClass]
    public class WhenRunningTheParser
    {
        private AutoMocker mocker;
        private Parser classUnderTest;
        private Mock<ITokeniser> tokeniser;

        [TestInitialize]
        public void Setup()
        {
            mocker = new AutoMocker();
            classUnderTest = mocker.CreateInstance<Parser>();
            tokeniser = new Mock<ITokeniser>();
        }

        [TestMethod]
        public void DoesNothingIfTheFirstTokenIsNull()
        {
            tokeniser.Setup(t => t.GetNextToken()).Returns(() => null);
            classUnderTest.Parse(tokeniser.Object);
            classUnderTest.Tokens().Should().BeEmpty();
        }

        [TestMethod]
        public void GetsTokensUntilThereAreNoMore()
        {
            tokeniser
                .SetupSequence(t => t.GetNextToken())
                .Returns(new KeywordToken("class"))
                .Returns(new IdentifierToken("blah"))
                .Returns(new SymbolToken("{"))
                .Returns(new SymbolToken("}"))
                .Returns(() => null);
            classUnderTest.Parse(tokeniser.Object);
            classUnderTest.Tokens().Should().HaveCount(4);
        }

        [TestMethod]
        public void RecognisesClassNode()
        {
            KeywordToken keyword = new KeywordToken("class");
            tokeniser
                .SetupSequence(t => t.GetNextToken())
                .Returns(keyword)
                .Returns(new IdentifierToken("blah"))
                .Returns(new SymbolToken("{"))
                .Returns(new SymbolToken("}"))
                .Returns(() => null);
            ClassNode classNode = new ClassNode();
            mocker.GetMock<INodeFactory>().Setup(f => f.Get(keyword)).Returns(classNode);
            classUnderTest.Parse(tokeniser.Object);
            classUnderTest.Tree.Should().Be(classNode);
        }

        [TestMethod]
        public void RecognisesWhileNode()
        {
            KeywordToken keyword = new KeywordToken("while");
            tokeniser
                .SetupSequence(t => t.GetNextToken())
                .Returns(keyword)
                .Returns(new SymbolToken("("))
                .Returns(new IdentifierToken("blah"))
                .Returns(new SymbolToken(")"))
                .Returns(new SymbolToken("{"))
                .Returns(new SymbolToken("}"))
                .Returns(() => null);
            var whileNode = new WhileNode();
            mocker.GetMock<INodeFactory>().Setup(f => f.Get(keyword)).Returns(whileNode);
            classUnderTest.Parse(tokeniser.Object);
            classUnderTest.Tree.Should().Be(whileNode);
        }
    }

    #endregion
}
