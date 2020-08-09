using System.Collections.Generic;
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
        public void PassesTokensToTheGrammarian()
        {
            KeywordToken t1 = new KeywordToken("class");
            IdentifierToken t2 = new IdentifierToken("blah");
            SymbolToken t3 = new SymbolToken("{");
            SymbolToken t4 = new SymbolToken("}");
            tokeniser
                .SetupSequence(t => t.GetNextToken())
                .Returns(t1)
                .Returns(t2)
                .Returns(t3)
                .Returns(t4)
                .Returns(() => null);
            ClassNode classNode = new ClassNode();
            mocker
                .GetMock<IGrammarian>()
                .Setup(f => f.ParseClass())
                .Returns(classNode);
            classUnderTest.Parse(tokeniser.Object);
            classUnderTest.Tree.Should().Be(classNode);
            mocker
                .GetMock<IGrammarian>()
                .Verify(f => f.LoadTokens(It.Is<Queue<Token>>(q =>
                    q.Contains(t1) &&
                    q.Contains(t2) &&
                    q.Contains(t3) &&
                    q.Contains(t4)
                )));
        }
    }

    #endregion
}
