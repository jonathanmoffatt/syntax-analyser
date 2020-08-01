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
            tokeniser = mocker.GetMock<ITokeniser>();
        }

        [TestMethod]
        public void DoesNothingIfTheFirstTokenIsNull()
        {
            tokeniser.Setup(t => t.GetNextToken()).Returns(() => null);
            classUnderTest.Parse();
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
            classUnderTest.Parse();
            classUnderTest.Tokens().Should().HaveCount(4);
        }

        //[TestMethod]
        //public void ReturnsTreeWithClassAtTheTop()
        //{
        //    classUnderTest
        //        .Feed(new KeywordToken("class"))
        //        .Feed(new SymbolToken("{"))
        //        .Feed(new SymbolToken("}"));
        //    classUnderTest.Tree.Should().BeOfType<ClassToken>();
        //}
    }

    #endregion
}
