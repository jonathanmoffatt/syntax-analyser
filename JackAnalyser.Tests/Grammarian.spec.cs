using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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

    }

    #endregion

    #region ClassVariableDeclarationGrammar

    [TestClass]
    public class ClassVariableDeclarationGrammar
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
        public void RecognisesStaticClassVariableDeclaration()
        {
            BranchNode node = classUnderTest.Get(t1, t2, t3, cvd1, cvd2, cvd3, cvd4, cvd5, cvd6, t4);
            node.Children[3].Should().BeOfType<ClassVariableDeclarationNode>();
            ((ClassVariableDeclarationNode)node.Children[3]).Children.Should().BeEquivalentTo(cvd1, cvd2, cvd3, cvd4, cvd5, cvd6);
        }

        [TestMethod]
        public void RecognisesFieldClassVariableDeclaration()
        {
            BranchNode node = classUnderTest.Get(t1, t2, t3, cvd1a, cvd2, cvd3, cvd4, cvd5, cvd6, t4);
            node.Children[3].Should().BeOfType<ClassVariableDeclarationNode>();
            ((ClassVariableDeclarationNode)node.Children[3]).Children.Should().BeEquivalentTo(cvd1a, cvd2, cvd3, cvd4, cvd5, cvd6);
        }

        [TestMethod]
        public void RecognisesMultipleClassVariableDeclarations()
        {
            BranchNode node = classUnderTest.Get(t1, t2, t3, cvd1, cvd2, cvd3, cvd4, cvd5, cvd6, cvd1a, cvd2, cvd3, cvd4, cvd5, cvd6, t4);
            node.Children[3].Should().BeOfType<ClassVariableDeclarationNode>();
            ((ClassVariableDeclarationNode)node.Children[3]).Children.Should().BeEquivalentTo(cvd1, cvd2, cvd3, cvd4, cvd5, cvd6);
            node.Children[4].Should().BeOfType<ClassVariableDeclarationNode>();
            ((ClassVariableDeclarationNode)node.Children[4]).Children.Should().BeEquivalentTo(cvd1a, cvd2, cvd3, cvd4, cvd5, cvd6);
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

    #region SubroutineDeclarationGrammar

    [TestClass]
    public class SubroutineDeclarationGrammar
    {
        private Grammarian classUnderTest;
        private AutoMocker mocker;
        private Token c1;
        private Token c2;
        private Token c3;
        private KeywordToken sd1;
        private KeywordToken sd1a;
        private KeywordToken sd1b;
        private KeywordToken sd2;
        private IdentifierToken sd3;
        private SymbolToken sd4;
        private KeywordToken sd5;
        private IdentifierToken sd6;
        private SymbolToken sd7;
        private IdentifierToken sd8;
        private IdentifierToken sd9;
        private SymbolToken sd10;
        private SymbolToken sd11;
        private SymbolToken sd12;
        private Token c4;

        [TestInitialize]
        public void Setup()
        {
            mocker = new AutoMocker();
            c1 = new KeywordToken("class");
            c2 = new IdentifierToken("blah");
            c3 = new SymbolToken("{");
            sd1 = new KeywordToken("constructor");
            sd1a = new KeywordToken("function");
            sd1b = new KeywordToken("method");
            sd2 = new KeywordToken("void");
            sd3 = new IdentifierToken("doSomething");
            sd4 = new SymbolToken("(");
            sd5 = new KeywordToken("int");
            sd6 = new IdentifierToken("x");
            sd7 = new SymbolToken(",");
            sd8 = new IdentifierToken("Game");
            sd9 = new IdentifierToken("game");
            sd10 = new SymbolToken(")");
            sd11 = new SymbolToken("{");
            sd12 = new SymbolToken("}");
            c4 = new SymbolToken("}");
            classUnderTest = mocker.CreateInstance<Grammarian>();
        }

        [TestMethod]
        public void ReturnsASubroutineDeclaration()
        {
            BranchNode node = classUnderTest.Get(c1, c2, c3, sd1, sd2, sd3, sd4, sd5, sd6, sd7, sd8, sd9, sd10, sd11, sd12, c4);
            SubroutineDeclarationNode subroutineDeclaration = node.Children[3] as SubroutineDeclarationNode;
            subroutineDeclaration.Should().NotBeNull();
            var children = subroutineDeclaration.Children;
            children[0].Should().Be(sd1);
            children[1].Should().Be(sd2);
            children[2].Should().Be(sd3);
            children[3].Should().Be(sd4);
            children[4].Should().BeOfType<ParameterListNode>();
            children[5].Should().Be(sd10);
            children[6].Should().Be(sd11);
            children[7].Should().Be(sd12);
            var parameterList = (ParameterListNode)subroutineDeclaration.Children[4];
            parameterList.Children.Should().BeEquivalentTo(sd5, sd6, sd7, sd8, sd9);
        }

        [TestMethod]
        public void ShouldHandleFunctionSubroutines()
        {
            BranchNode node = classUnderTest.Get(c1, c2, c3, sd1a, sd2, sd3, sd4, sd5, sd6, sd7, sd8, sd9, sd10, sd11, sd12, c4);
            node.Children[3].Should().BeOfType<SubroutineDeclarationNode>();
        }

        [TestMethod]
        public void ShouldHandleMethodSubroutines()
        {
            BranchNode node = classUnderTest.Get(c1, c2, c3, sd1b, sd2, sd3, sd4, sd5, sd6, sd7, sd8, sd9, sd10, sd11, sd12, c4);
            node.Children[3].Should().BeOfType<SubroutineDeclarationNode>();
        }
    }

    #endregion
}
