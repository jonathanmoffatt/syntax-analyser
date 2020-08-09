using System;
using System.Linq;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Moq.AutoMock;

namespace JackAnalyser.Tests
{
    internal static class NodeTestExtensions
    {
        public static void ShouldGenerateXml(this Node node, string expectedXml)
        {
            node.ToXml().Should().BeEquivalentTo(XElement.Parse(expectedXml));
        }
    }

    #region ClassGrammar

    [TestClass]
    public class ClassGrammar
    {
        private Grammarian classUnderTest;
        private AutoMocker mocker;
        private Token t1, t2, t3, t4;

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
            classUnderTest.LoadTokens(t1, t2, t3, t4).ParseClass().ShouldGenerateXml(@"
                <class>
                  <keyword>class</keyword>
                  <identifier>blah</identifier>
                  <symbol>{</symbol>
                  <symbol>}</symbol>
                </class>");
        }

        [TestMethod]
        public void ThrowsExceptionIfClassIsMissingClassName()
        {
            classUnderTest.LoadTokens(t1, t3, t4);
            classUnderTest
                .Invoking(c => c.ParseClass())
                .Should().Throw<ApplicationException>()
                .WithMessage("class expected a className identifier, got '{' instead");
        }

        [TestMethod]
        public void ThrowsExceptionIfClassIsMissingOpeningBrace()
        {
            classUnderTest.LoadTokens(t1, t2, t4);
            classUnderTest
                .Invoking(c => c.ParseClass())
                .Should().Throw<ApplicationException>()
                .WithMessage("expected symbol '{', got '}' instead");
        }

        [TestMethod]
        public void ThrowsExceptionIfClassIsMissingClosingBrace()
        {
            classUnderTest.LoadTokens(t1, t2, t3);
            classUnderTest
                .Invoking(c => c.ParseClass())
                .Should().Throw<ApplicationException>()
                .WithMessage("expected symbol '}', reached end of file instead");
        }

        [TestMethod]
        public void ThrowsExceptionIfClassIsMissingEverything()
        {
            classUnderTest.LoadTokens(t1);
            classUnderTest
                .Invoking(c => c.ParseClass())
                .Should().Throw<ApplicationException>()
                .WithMessage("class expected a className identifier, reached end of file instead");
        }

    }

    #endregion

    #region ClassVariableDeclarationGrammar

    [TestClass]
    public class ClassVariableDeclarationGrammar
    {
        private Grammarian classUnderTest;
        private AutoMocker mocker;
        private Token t1, t2, t3, t4;
        private Token cvd1, cvd1a, cvd2, cvd3, cvd4, cvd5, cvd6;

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
            classUnderTest
                .LoadTokens(t1, t2, t3, cvd1, cvd2, cvd3, cvd4, cvd5, cvd6, t4)
                .ParseClass()
                .ShouldGenerateXml(@"
                <class>
                  <keyword>class</keyword>
                  <identifier>blah</identifier>
                  <symbol>{</symbol>
                  <classVarDec>
                    <keyword>static</keyword>
                    <keyword>boolean</keyword>
                    <identifier>hasStarted</identifier>
                    <symbol>,</symbol>
                    <identifier>hasFinished</identifier>
                    <symbol>;</symbol>
                  </classVarDec>
                  <symbol>}</symbol>
                </class>
            ");
        }

        [TestMethod]
        public void RecognisesMultipleClassVariableDeclarations()
        {
            classUnderTest
                .LoadTokens(t1, t2, t3, cvd1, cvd2, cvd3, cvd4, cvd5, cvd6, cvd1a, cvd2, cvd3, cvd4, cvd5, cvd6, t4)
                .ParseClass()
                .ShouldGenerateXml(@"
                <class>
                  <keyword>class</keyword>
                  <identifier>blah</identifier>
                  <symbol>{</symbol>
                  <classVarDec>
                    <keyword>static</keyword>
                    <keyword>boolean</keyword>
                    <identifier>hasStarted</identifier>
                    <symbol>,</symbol>
                    <identifier>hasFinished</identifier>
                    <symbol>;</symbol>
                  </classVarDec>
                  <classVarDec>
                    <keyword>field</keyword>
                    <keyword>boolean</keyword>
                    <identifier>hasStarted</identifier>
                    <symbol>,</symbol>
                    <identifier>hasFinished</identifier>
                    <symbol>;</symbol>
                  </classVarDec>
                  <symbol>}</symbol>
                </class>
            ");
        }

        [TestMethod]
        public void ThrowsExceptionIfClassVariableDefinitionTypeMissing()
        {
            classUnderTest.LoadTokens(t1, t2, t3, cvd1);
            classUnderTest
                .Invoking(c => c.ParseClass())
                .Should().Throw<ApplicationException>()
                .WithMessage("class variable definition expected a type, reached end of file instead");
        }

        [TestMethod]
        public void ThrowsExceptionIfClassVariableDefinitionVariableNameMissing()
        {
            classUnderTest.LoadTokens(t1, t2, t3, cvd1, cvd2);
            classUnderTest
                .Invoking(c => c.ParseClass())
                .Should().Throw<ApplicationException>()
                .WithMessage("class variable declaration expected a variable name, reached end of file instead");
        }
    }

    #endregion

    #region SubroutineDeclarationGrammar

    [TestClass]
    public class SubroutineDeclarationGrammar
    {
        private Grammarian classUnderTest;
        private AutoMocker mocker;
        private Token sd1, sd1a, sd1b, sd2, sd3, sd4, sd5, sd6, sd7, sd8, sd9, sd10, sd11, sd12;
        private Token vd1, vd2, vd3, vd4, vd5, vd6, vd7, vd8, vd9, vd10;

        [TestInitialize]
        public void Setup()
        {
            mocker = new AutoMocker();
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
            vd1 = new KeywordToken("var");
            vd2 = new KeywordToken("boolean");
            vd3 = new IdentifierToken("hasStarted");
            vd4 = new SymbolToken(",");
            vd5 = new IdentifierToken("hasFinished");
            vd6 = new SymbolToken(";");
            vd7 = new KeywordToken("var");
            vd8 = new IdentifierToken("Player");
            vd9 = new IdentifierToken("player");
            vd10 = new SymbolToken(";");
            sd12 = new SymbolToken("}");
            classUnderTest = mocker.CreateInstance<Grammarian>();
        }

        [TestMethod]
        public void ReturnsASubroutineDeclaration()
        {
            classUnderTest
                .LoadTokens(sd1, sd2, sd3, sd4, sd5, sd6, sd7, sd8, sd9, sd10, sd11, sd12)
                .ParseSubroutineDeclaration()
                .ShouldGenerateXml(@"
                <subroutineDec>
                    <keyword>constructor</keyword>
                    <keyword>void</keyword>
                    <identifier>doSomething</identifier>
                    <symbol>(</symbol>
                    <parameterList>
                        <keyword>int</keyword>
                        <identifier>x</identifier>
                        <symbol>,</symbol>
                        <identifier>Game</identifier>
                        <identifier>game</identifier>
                    </parameterList>
                    <symbol>)</symbol>
                    <subroutineBody>
                        <symbol>{</symbol>
                        <symbol>}</symbol>
                    </subroutineBody>
                </subroutineDec>
            ");
        }

        [TestMethod]
        public void ShouldHandleFunctionSubroutines()
        {
            classUnderTest
                .LoadTokens(sd1a, sd2, sd3, sd4, sd5, sd6, sd7, sd8, sd9, sd10, sd11, sd12)
                .ParseSubroutineDeclaration()
                .ShouldGenerateXml(@"
                  <subroutineDec>
                    <keyword>function</keyword>
                    <keyword>void</keyword>
                    <identifier>doSomething</identifier>
                    <symbol>(</symbol>
                    <parameterList>
                      <keyword>int</keyword>
                      <identifier>x</identifier>
                      <symbol>,</symbol>
                      <identifier>Game</identifier>
                      <identifier>game</identifier>
                    </parameterList>
                    <symbol>)</symbol>
                    <subroutineBody>
                      <symbol>{</symbol>
                      <symbol>}</symbol>
                    </subroutineBody>
                  </subroutineDec>
            ");
        }

        [TestMethod]
        public void ShouldHandleMethodSubroutines()
        {
            classUnderTest
                .LoadTokens(sd1b, sd2, sd3, sd4, sd5, sd6, sd7, sd8, sd9, sd10, sd11, sd12)
                .ParseSubroutineDeclaration()
                .ShouldGenerateXml(@"
                  <subroutineDec>
                    <keyword>method</keyword>
                    <keyword>void</keyword>
                    <identifier>doSomething</identifier>
                    <symbol>(</symbol>
                    <parameterList>
                      <keyword>int</keyword>
                      <identifier>x</identifier>
                      <symbol>,</symbol>
                      <identifier>Game</identifier>
                      <identifier>game</identifier>
                    </parameterList>
                    <symbol>)</symbol>
                    <subroutineBody>
                      <symbol>{</symbol>
                      <symbol>}</symbol>
                    </subroutineBody>
                  </subroutineDec>
            ");
        }

        [TestMethod]
        public void ShouldHandleVariableDeclarations()
        {
            classUnderTest
                .LoadTokens(sd1, sd2, sd3, sd4, sd5, sd6, sd7, sd8, sd9, sd10, sd11, vd1, vd2, vd3, vd4, vd5, vd6, vd7, vd8, vd9, vd10, sd12)
                .ParseSubroutineDeclaration()
                .ShouldGenerateXml(@"
                      <subroutineDec>
                        <keyword>constructor</keyword>
                        <keyword>void</keyword>
                        <identifier>doSomething</identifier>
                        <symbol>(</symbol>
                        <parameterList>
                          <keyword>int</keyword>
                          <identifier>x</identifier>
                          <symbol>,</symbol>
                          <identifier>Game</identifier>
                          <identifier>game</identifier>
                        </parameterList>
                        <symbol>)</symbol>
                        <subroutineBody>
                          <symbol>{</symbol>
                          <varDec>
                            <keyword>var</keyword>
                            <keyword>boolean</keyword>
                            <identifier>hasStarted</identifier>
                            <symbol>,</symbol>
                            <identifier>hasFinished</identifier>
                            <symbol>;</symbol>
                          </varDec>
                          <varDec>
                            <keyword>var</keyword>
                            <identifier>Player</identifier>
                            <identifier>player</identifier>
                            <symbol>;</symbol>
                          </varDec>
                          <symbol>}</symbol>
                        </subroutineBody>
                      </subroutineDec>
            ");
        }
    }

    #endregion

    #region SimpleLetStatementGrammar

    [TestClass]
    public class SimpleLetStatementGrammar
    {
        private Grammarian classUnderTest;
        private AutoMocker mocker;
        private Token ls1, ls2, ls3, ls4, ls5;

        [TestInitialize]
        public void Setup()
        {
            mocker = new AutoMocker();
            ls1 = new KeywordToken("let");
            ls2 = new IdentifierToken("x");
            ls3 = new SymbolToken("=");
            ls4 = new IntegerConstantToken("1234");
            ls5 = new SymbolToken(";");
            classUnderTest = mocker.CreateInstance<Grammarian>();
        }

        [TestMethod]
        public void RecognisesLetStatementWithSimpleExpression()
        {
            classUnderTest.LoadTokens(ls1, ls2, ls3, ls4, ls5);
            classUnderTest.ParseLetStatement().ShouldGenerateXml(@"
                        <letStatement>
                          <keyword>let</keyword>
                          <identifier>x</identifier>
                          <symbol>=</symbol>
                          <expression>
                            <term>
                              <integerConstant>1234</integerConstant>
                            </term>
                          </expression>
                          <symbol>;</symbol>
                        </letStatement>
            ");
        }
    }

    #endregion

    #region ComplexLetStatementGrammar

    [TestClass]
    public class ComplexLetStatementGrammar
    {
        private Grammarian classUnderTest;
        private AutoMocker mocker;
        private Token cls1, cls2, cls3, cls4, cls5, cls6, cls7, cls8, cls9, cls10, cls11;

        [TestInitialize]
        public void Setup()
        {
            mocker = new AutoMocker();
            cls1 = new KeywordToken("let");
            cls2 = new IdentifierToken("y");
            cls3 = new SymbolToken("[");
            cls4 = new IdentifierToken("x");
            cls5 = new SymbolToken("+");
            cls6 = new IntegerConstantToken("1");
            cls7 = new SymbolToken("]");
            cls8 = new SymbolToken("=");
            cls9 = new SymbolToken("~");
            cls10 = new IdentifierToken("finished");
            cls11 = new SymbolToken(";");
            classUnderTest = mocker.CreateInstance<Grammarian>();
        }

        [TestMethod]
        public void RecognisesLetStatementWithMoreComplexExpression()
        {
            classUnderTest.LoadTokens(cls1, cls2, cls3, cls4, cls5, cls6, cls7, cls8, cls9, cls10, cls11);
            classUnderTest.ParseLetStatement()
                .ShouldGenerateXml(@"
                            <letStatement>
                              <keyword>let</keyword>
                              <identifier>y</identifier>
                              <symbol>[</symbol>
                              <expression>
                                <term>
                                  <identifier>x</identifier>
                                </term>
                                <symbol>+</symbol>
                                <term>
                                  <integerConstant>1</integerConstant>
                                </term>
                              </expression>
                              <symbol>]</symbol>
                              <symbol>=</symbol>
                              <expression>
                                <term>
                                  <symbol>~</symbol>
                                  <term>
                                    <identifier>finished</identifier>
                                  </term>
                                </term>
                              </expression>
                              <symbol>;</symbol>
                            </letStatement>
                ");
        }
    }

    #endregion

    #region IfStatementGrammar

    [TestClass]
    public class IfStatementGrammar
    {
        private Grammarian classUnderTest;
        private AutoMocker mocker;
        private Token t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15;

        [TestInitialize]
        public void Setup()
        {
            mocker = new AutoMocker();
            classUnderTest = mocker.CreateInstance<Grammarian>();

            t1 = new KeywordToken("if");
            t2 = new SymbolToken("(");
            t3 = new SymbolToken("(");
            t4 = new IdentifierToken("x");
            t5 = new SymbolToken("*");
            t6 = new IntegerConstantToken("5");
            t7 = new SymbolToken(")");
            t8 = new SymbolToken(">");
            t9 = new IntegerConstantToken("30");
            t10 = new SymbolToken(")");
            t11 = new SymbolToken("{");
            t12 = new SymbolToken("}");
            t13 = new KeywordToken("else");
            t14 = new SymbolToken("{");
            t15 = new SymbolToken("{");
        }


    }

    #endregion
}
