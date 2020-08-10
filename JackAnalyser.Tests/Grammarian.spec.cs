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
        private Token t1, t2, t3, t4;

        [TestInitialize]
        public void Setup()
        {
            t1 = new Token(NodeType.Keyword, "class");
            t2 = new Token(NodeType.Identifier, "blah");
            t3 = new Token(NodeType.Symbol, "{");
            t4 = new Token(NodeType.Symbol, "}");
            classUnderTest = new Grammarian();
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
        private Token cvd1, cvd1a, cvd2, cvd3, cvd4, cvd5, cvd6;

        [TestInitialize]
        public void Setup()
        {
            cvd1 = new Token(NodeType.Keyword, "static");
            cvd1a = new Token(NodeType.Keyword, "field");
            cvd2 = new Token(NodeType.Keyword, "boolean");
            cvd3 = new Token(NodeType.Identifier, "hasStarted");
            cvd4 = new Token(NodeType.Symbol, ",");
            cvd5 = new Token(NodeType.Identifier, "hasFinished");
            cvd6 = new Token(NodeType.Symbol, ";");
            classUnderTest = new Grammarian();
        }

        [TestMethod]
        public void RecognisesStaticClassVariableDeclaration()
        {
            classUnderTest
                .LoadTokens(cvd1, cvd2, cvd3, cvd4, cvd5, cvd6)
                .ParseClassVariableDeclaration()
                .ShouldGenerateXml(@"
                  <classVarDec>
                    <keyword>static</keyword>
                    <keyword>boolean</keyword>
                    <identifier>hasStarted</identifier>
                    <symbol>,</symbol>
                    <identifier>hasFinished</identifier>
                    <symbol>;</symbol>
                  </classVarDec>
            ");
        }

        [TestMethod]
        public void RecognisesFieldClassVariableDeclarations()
        {
            classUnderTest
                .LoadTokens(cvd1a, cvd2, cvd3, cvd4, cvd5, cvd6)
                .ParseClassVariableDeclaration()
                .ShouldGenerateXml(@"
                  <classVarDec>
                    <keyword>field</keyword>
                    <keyword>boolean</keyword>
                    <identifier>hasStarted</identifier>
                    <symbol>,</symbol>
                    <identifier>hasFinished</identifier>
                    <symbol>;</symbol>
                  </classVarDec>
            ");
        }

        [TestMethod]
        public void ThrowsExceptionIfClassVariableDefinitionTypeMissing()
        {
            classUnderTest.LoadTokens(cvd1);
            classUnderTest
                .Invoking(c => c.ParseClassVariableDeclaration())
                .Should().Throw<ApplicationException>()
                .WithMessage("class variable definition expected a type, reached end of file instead");
        }

        [TestMethod]
        public void ThrowsExceptionIfClassVariableDefinitionVariableNameMissing()
        {
            classUnderTest.LoadTokens(cvd1, cvd2);
            classUnderTest
                .Invoking(c => c.ParseClassVariableDeclaration())
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
        private Token sd1, sd1a, sd1b, sd2, sd3, sd4, sd5, sd6, sd7, sd8, sd9, sd10, sd11, sd12;
        private Token vd1, vd2, vd3, vd4, vd5, vd6, vd7, vd8, vd9, vd10;

        [TestInitialize]
        public void Setup()
        {
            sd1 = new Token(NodeType.Keyword, "constructor");
            sd1a = new Token(NodeType.Keyword, "function");
            sd1b = new Token(NodeType.Keyword, "method");
            sd2 = new Token(NodeType.Keyword, "void");
            sd3 = new Token(NodeType.Identifier, "doSomething");
            sd4 = new Token(NodeType.Symbol, "(");
            sd5 = new Token(NodeType.Keyword, "int");
            sd6 = new Token(NodeType.Identifier, "x");
            sd7 = new Token(NodeType.Symbol, ",");
            sd8 = new Token(NodeType.Identifier, "Game");
            sd9 = new Token(NodeType.Identifier, "game");
            sd10 = new Token(NodeType.Symbol, ")");
            sd11 = new Token(NodeType.Symbol, "{");
            vd1 = new Token(NodeType.Keyword, "var");
            vd2 = new Token(NodeType.Keyword, "boolean");
            vd3 = new Token(NodeType.Identifier, "hasStarted");
            vd4 = new Token(NodeType.Symbol, ",");
            vd5 = new Token(NodeType.Identifier, "hasFinished");
            vd6 = new Token(NodeType.Symbol, ";");
            vd7 = new Token(NodeType.Keyword, "var");
            vd8 = new Token(NodeType.Identifier, "Player");
            vd9 = new Token(NodeType.Identifier, "player");
            vd10 = new Token(NodeType.Symbol, ";");
            sd12 = new Token(NodeType.Symbol, "}");
            classUnderTest = new Grammarian();
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

        [TestInitialize]
        public void Setup()
        {
            classUnderTest = new Grammarian();
            Token ls1 = new Token(NodeType.Keyword, "let");
            Token ls2 = new Token(NodeType.Identifier, "x");
            Token ls3 = new Token(NodeType.Symbol, "=");
            Token ls4 = new Token(NodeType.IntegerConstant, "1234");
            Token ls5 = new Token(NodeType.Symbol, ";");
            classUnderTest.LoadTokens(ls1, ls2, ls3, ls4, ls5);
        }

        [TestMethod]
        public void RecognisesLetStatementWithSimpleExpression()
        {
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

        [TestInitialize]
        public void Setup()
        {
            Token cls1 = new Token(NodeType.Keyword, "let");
            Token cls2 = new Token(NodeType.Identifier, "y");
            Token cls3 = new Token(NodeType.Symbol, "[");
            Token cls4 = new Token(NodeType.Identifier, "x");
            Token cls5 = new Token(NodeType.Symbol, "+");
            Token cls6 = new Token(NodeType.IntegerConstant, "1");
            Token cls7 = new Token(NodeType.Symbol, "]");
            Token cls8 = new Token(NodeType.Symbol, "=");
            Token cls9 = new Token(NodeType.Symbol, "~");
            Token cls10 = new Token(NodeType.Identifier, "finished");
            Token cls11 = new Token(NodeType.Symbol, ";");
            classUnderTest = new Grammarian();
            classUnderTest.LoadTokens(cls1, cls2, cls3, cls4, cls5, cls6, cls7, cls8, cls9, cls10, cls11);
        }

        [TestMethod]
        public void RecognisesLetStatementWithMoreComplexExpression()
        {
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

    #region ReturnStatementGrammar

    [TestClass]
    public class ReturnStatementGrammar
    {
        private Grammarian classUnderTest;
        private Token t1, t2, t3;

        [TestInitialize]
        public void Setup()
        {
            classUnderTest = new Grammarian();
            t1 = new Token(NodeType.Keyword, "return");
            t2 = new Token(NodeType.Identifier, "result");
            t3 = new Token(NodeType.Symbol, ";");
        }

        [TestMethod]
        public void ShouldParseCorrectlyWithNoReturnValue()
        {
            classUnderTest.LoadTokens(t1, t3)
                .ParseReturnStatement()
                .ShouldGenerateXml(@"
                    <returnStatement>
                        <keyword>return</keyword>
                        <symbol>;</symbol>
                    </returnStatement>
                ");
        }

        [TestMethod]
        public void ShouldParseCorrectlyWithReturnExpression()
        {
            classUnderTest.LoadTokens(t1, t2, t3)
                .ParseReturnStatement()
                .ShouldGenerateXml(@"
                    <returnStatement>
                        <keyword>return</keyword>
                        <expression>
                            <term>
                                <identifier>result</identifier>
                            </term>
                        </expression>
                        <symbol>;</symbol>
                    </returnStatement>
                ");
        }
    }

    #endregion

    #region IfStatementGrammar

    [TestClass]
    public class IfStatementGrammar
    {
        private Grammarian classUnderTest;
        private Token t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16, t17, t18, t19, t20;

        [TestInitialize]
        public void Setup()
        {
            classUnderTest = new Grammarian();
            t1 = new Token(NodeType.Keyword, "if");
            t2 = new Token(NodeType.Symbol, "(");
            t3 = new Token(NodeType.Symbol, "(");
            t4 = new Token(NodeType.Identifier, "x");
            t5 = new Token(NodeType.Symbol, "*");
            t6 = new Token(NodeType.IntegerConstant, "5");
            t7 = new Token(NodeType.Symbol, ")");
            t8 = new Token(NodeType.Symbol, ">");
            t9 = new Token(NodeType.IntegerConstant, "30");
            t10 = new Token(NodeType.Symbol, ")");
            t11 = new Token(NodeType.Symbol, "{");
            t12 = new Token(NodeType.Keyword, "return");
            t13 = new Token(NodeType.Symbol, ";");
            t14 = new Token(NodeType.Symbol, "}");
            t15 = new Token(NodeType.Keyword, "else");
            t16 = new Token(NodeType.Symbol, "{");
            t17 = new Token(NodeType.Keyword, "return");
            t18 = new Token(NodeType.Identifier, "result");
            t19 = new Token(NodeType.Symbol, ";");
            t20 = new Token(NodeType.Symbol, "}");
        }

        [TestMethod]
        public void ParsesCorrectlyWithoutAnElseBlock()
        {
            classUnderTest.LoadTokens(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14);
            BranchNode node = classUnderTest.ParseIfStatement();
            Console.WriteLine(node.ToXml());
            node.ShouldGenerateXml(@"
                <ifStatement>
                  <keyword>if</keyword>
                  <symbol>(</symbol>
                  <expression>
                    <term>
                        <symbol>(</symbol>
                        <expression>
                            <term>
                                <identifier>x</identifier>
                            </term>
                            <symbol>*</symbol>
                            <term>
                                <integerConstant>5</integerConstant>
                            </term>
                        </expression>
                        <symbol>)</symbol>
                    </term>
                    <symbol>&gt;</symbol>
                    <term>
                        <integerConstant>30</integerConstant>
                    </term>
                </expression>
                <symbol>)</symbol>
                <symbol>{</symbol>
                <statements>
                    <returnStatement>
                      <keyword>return</keyword>
                      <symbol>;</symbol>
                    </returnStatement>
                </statements>
                <symbol>}</symbol>
              </ifStatement>
            ");
        }

        [TestMethod]
        public void ParsesCorrectlyWithAnElseBlock()
        {
            classUnderTest.LoadTokens(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16, t17, t18, t19, t20);
            classUnderTest.ParseIfStatement().ShouldGenerateXml(@"
                <ifStatement>
                  <keyword>if</keyword>
                  <symbol>(</symbol>
                  <expression>
                    <term>
                        <symbol>(</symbol>
                        <expression>
                            <term>
                                <identifier>x</identifier>
                            </term>
                            <symbol>*</symbol>
                            <term>
                                <integerConstant>5</integerConstant>
                            </term>
                        </expression>
                        <symbol>)</symbol>
                    </term>
                    <symbol>&gt;</symbol>
                    <term>
                        <integerConstant>30</integerConstant>
                    </term>
                </expression>
                <symbol>)</symbol>
                <symbol>{</symbol>
                <statements>
                    <returnStatement>
                      <keyword>return</keyword>
                      <symbol>;</symbol>
                    </returnStatement>
                </statements>
                <symbol>}</symbol>
                <keyword>else</keyword>
                <symbol>{</symbol>
                <statements>
                    <returnStatement>
                        <keyword>return</keyword>
                        <expression>
                            <term>
                                <identifier>result</identifier>
                            </term>
                        </expression>
                        <symbol>;</symbol>
                    </returnStatement>
                </statements>
                <symbol>}</symbol>
              </ifStatement>
            ");
        }
    }

    #endregion

    #region WhileStatementGrammar

    [TestClass]
    public class WhileStatementGrammar
    {
        private Grammarian classUnderTest;

        [TestInitialize]
        public void Setup()
        {
            classUnderTest = new Grammarian();
            var t1 = new Token(NodeType.Keyword, "while");
            var t2 = new Token(NodeType.Symbol, "(");
            var t3 = new Token(NodeType.Identifier, "inProgress");
            var t4 = new Token(NodeType.Symbol, ")");
            var t5 = new Token(NodeType.Symbol, "{");
            var t6 = new Token(NodeType.Keyword, "let");
            var t7 = new Token(NodeType.Identifier, "x");
            var t8 = new Token(NodeType.Symbol, "=");
            var t9 = new Token(NodeType.Identifier, "y");
            var t10 = new Token(NodeType.Symbol, ";");
            var t11 = new Token(NodeType.Symbol, "}");
            classUnderTest.LoadTokens(t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11);
        }

        [TestMethod]
        public void ParsesCorrectly()
        {
            classUnderTest.ParseWhileStatement().ShouldGenerateXml(@"
                <whileStatement>
                  <keyword>while</keyword>
                  <symbol>(</symbol>
                  <expression>
                    <term>
                        <identifier>inProgress</identifier>
                    </term>
                  </expression>
                <symbol>)</symbol>
                <symbol>{</symbol>
                <statements>
                    <letStatement>
                        <keyword>let</keyword>
                        <identifier>x</identifier>
                        <symbol>=</symbol>
                        <expression>
                            <term>
                                <identifier>y</identifier>
                            </term>
                        </expression>
                        <symbol>;</symbol>
                    </letStatement>
                </statements>
                <symbol>}</symbol>
              </whileStatement>
            ");
        }
    }

    #endregion
}
