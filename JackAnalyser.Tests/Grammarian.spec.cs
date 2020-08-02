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
        private KeywordToken cvd1;
        private KeywordToken cvd1a;
        private KeywordToken cvd2;
        private IdentifierToken cvd3;
        private SymbolToken cvd4;
        private IdentifierToken cvd5;
        private SymbolToken cvd6;

        [TestInitialize]
        public void Setup()
        {
            mocker = new AutoMocker();
            t1 = new KeywordToken("class");
            t2 = new IdentifierToken("blah");
            t3 = new SymbolToken("{");
            t4 = new SymbolToken("}");
            cvd1 = new KeywordToken("static");
            cvd1a = new KeywordToken("field");
            cvd2 = new KeywordToken("boolean");
            cvd3 = new IdentifierToken("hasStarted");
            cvd4 = new SymbolToken(",");
            cvd5 = new IdentifierToken("hasFinished");
            cvd6 = new SymbolToken(";");
            classUnderTest = mocker.CreateInstance<Grammarian>();
        }

        [TestMethod]
        public void ReturnsAClassNode()
        {
            classUnderTest.Get(t1, t2, t3, t4).Should().BeOfType<ClassNode>();
        }

        [TestMethod]
        public void IncludesChildTokens()
        {
            BranchNode node = classUnderTest.Get(t1, t2, t3, t4);
            node.Children.Should().BeEquivalentTo(t1, t2, t3, t4);
        }

        [TestMethod]
        public void ThrowsExceptionIfClassIsMissingClassName()
        {
            classUnderTest
                .Invoking(c => c.Get(t1, t3, t4))
                .Should().Throw<ApplicationException>()
                .WithMessage("class expected a className identifier");
        }

        [TestMethod]
        public void ThrowsExceptionIfClassIsMissingOpeningBrace()
        {
            classUnderTest
                .Invoking(c => c.Get(t1, t2, t4))
                .Should().Throw<ApplicationException>()
                .WithMessage("expected symbol '{'");
        }

        [TestMethod]
        public void ThrowsExceptionIfClassIsMissingClosingBrace()
        {
            classUnderTest
                .Invoking(c => c.Get(t1, t2, t3))
                .Should().Throw<ApplicationException>()
                .WithMessage("expected symbol '}'");
        }

        [TestMethod]
        public void ThrowsExceptionIfClassIsMissingEverything()
        {
            classUnderTest
                .Invoking(c => c.Get(t1))
                .Should().Throw<ApplicationException>()
                .WithMessage("class expected a className identifier");
        }

        [TestMethod]
        public void RecognisesStaticClassVariableDeclaration()
        {
            BranchNode node = classUnderTest.Get(t1, t2, t3, cvd1, cvd2, cvd3, cvd4, cvd5, cvd6, t4);
            node.Children[3].Should().BeOfType<ClassVariableDeclaration>();
            ((ClassVariableDeclaration)node.Children[3]).Children.Should().BeEquivalentTo(cvd1, cvd2, cvd3, cvd4, cvd5, cvd6);
        }

        [TestMethod]
        public void RecognisesFieldClassVariableDeclaration()
        {
            BranchNode node = classUnderTest.Get(t1, t2, t3, cvd1a, cvd2, cvd3, cvd4, cvd5, cvd6, t4);
            node.Children[3].Should().BeOfType<ClassVariableDeclaration>();
            ((ClassVariableDeclaration)node.Children[3]).Children.Should().BeEquivalentTo(cvd1a, cvd2, cvd3, cvd4, cvd5, cvd6);
        }

        [TestMethod]
        public void RecognisesMultipleClassVariableDeclarations()
        {
            BranchNode node = classUnderTest.Get(t1, t2, t3, cvd1, cvd2, cvd3, cvd4, cvd5, cvd6, cvd1a, cvd2, cvd3, cvd4, cvd5, cvd6, t4);
            node.Children[3].Should().BeOfType<ClassVariableDeclaration>();
            ((ClassVariableDeclaration)node.Children[3]).Children.Should().BeEquivalentTo(cvd1, cvd2, cvd3, cvd4, cvd5, cvd6);
            node.Children[4].Should().BeOfType<ClassVariableDeclaration>();
            ((ClassVariableDeclaration)node.Children[4]).Children.Should().BeEquivalentTo(cvd1a, cvd2, cvd3, cvd4, cvd5, cvd6);
        }

        [TestMethod]
        public void ThrowsExceptionIfClassVariableDefinitionTypeMissing()
        {
            classUnderTest
                .Invoking(c => c.Get(t1, t2, t3, cvd1))
                .Should().Throw<ApplicationException>()
                .WithMessage("class variable definition expected a type");
        }

        [TestMethod]
        public void ThrowsExceptionIfClassVariableDefinitionVariableNameMissing()
        {
            classUnderTest
                .Invoking(c => c.Get(t1, t2, t3, cvd1, cvd2))
                .Should().Throw<ApplicationException>()
                .WithMessage("class variable declaration expected a variable name");
        }
    }

    #endregion
}
