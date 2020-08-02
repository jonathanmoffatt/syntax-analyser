﻿using System.Collections.Generic;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.AutoMock;

namespace JackAnalyser.Tests
{
    #region ClassGrammar

    [TestClass]
    public class ClassGrammar
    {
        private Queue<Token> tokens;
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
            tokens = new Queue<Token>(new[] { t1, t2, t3, t4 });
            classUnderTest = mocker.CreateInstance<Grammarian>();
        }

        [TestMethod]
        public void ShouldReturnAClassNode()
        {
            classUnderTest.Get(tokens).Should().BeOfType<ClassNode>();
        }

        [TestMethod]
        public void ShouldIncludeChildTokens()
        {
            BranchNode node = classUnderTest.Get(tokens);
            node.Children.Should().BeEquivalentTo(t1, t2, t3, t4);
        }
    }

    #endregion
}