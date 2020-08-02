using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;

namespace JackAnalyser.Tests
{
    #region ClassGrammar

    [TestClass]
    public class ClassGrammar
    {
        private Grammarian classUnderTest;
        private AutoMocker mocker;
        private Token t1;
        private Token t2;
        private Token t3;
        private Token t4;

        [TestInitialize]
        public void Setup()
        {
            mocker = new AutoMocker();
            t1 = new KeywordToken("class");
            t2 = new IdentifierToken("blah");
            t3 = new SymbolToken("{");
            t4 = new SymbolToken("}");
            classUnderTest = mocker.CreateInstance<Grammarian>();
        }

        [TestMethod]
        public void ShouldReturnAClassNode()
        {
            classUnderTest.Get(t1, t2, t3, t4).Should().BeOfType<ClassNode>();
        }

        [TestMethod]
        public void ShouldIncludeChildTokens()
        {
            BranchNode node = classUnderTest.Get(t1, t2, t3, t4);
            node.Children.Should().BeEquivalentTo(t1, t2, t3, t4);
        }

        [TestMethod]
        public void ShouldThrowExceptionIfClassIsMissingClassName()
        {
            classUnderTest
                .Invoking(c => c.Get(t1, t3, t4))
                .Should().Throw<ApplicationException>()
                .WithMessage("class expects a 'className' identifier");
        }

        [TestMethod]
        public void ShouldThrowExceptionIfClassIsMissingOpeningBrace()
        {
            classUnderTest
                .Invoking(c => c.Get(t1, t2, t4))
                .Should().Throw<ApplicationException>()
                .WithMessage("class 'blah' expects symbol '{'");
        }

        [TestMethod]
        public void ShouldThrowExceptionIfClassIsMissingClosingBrace()
        {
            classUnderTest
                .Invoking(c => c.Get(t1, t2, t3))
                .Should().Throw<ApplicationException>()
                .WithMessage("class 'blah' expects symbol '}'");
        }

        [TestMethod]
        public void ShouldThrowExceptionIfClassIsMissingEverything()
        {
            classUnderTest
                .Invoking(c => c.Get(t1))
                .Should().Throw<ApplicationException>()
                .WithMessage("class expects a 'className' identifier");
        }
    }

    #endregion
}
