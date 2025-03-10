// <copyright file="LogicEngineTests.cs" company="Omar Urbano-Rendon">
// Copyright (c) Omar Urbano-Rendon. All rights reserved.
// </copyright>

namespace SpreadSheetEngine.Tests
{
    /// <summary>
    /// Test class to test our expression tree class. Will test for normal cases, minimum cases, maximum cases and
    /// exceptional cases.
    /// </summary>
    public class LogicEngineTests
    {
        /// <summary>
        /// This tests a typical infix expression that we will encounter by the user, we expect to get a postfix
        /// expression back.
        /// </summary>
        [Test]
        public void TestingNormalCaseParseInput()
        {
            ExpressionTree expressionTree = new ExpressionTree();

            string[] parseResult = expressionTree.ParseInput("3+4+5");
            string[] test = { "3", "4", "+", "5", "+" };

            Assert.That(parseResult, Is.EqualTo(test));
        }

        /// <summary>
        /// This tests the minimum input an expression can be with one operator and one number and one variable.
        /// </summary>
        [Test]
        public void TestingMinimumCaseParseInput()
        {
            ExpressionTree expressionTree = new ExpressionTree();

            string[] parseResult = expressionTree.ParseInput("3A+4");
            string[] test = { "3A", "4", "+" };

            Assert.That(parseResult, Is.EqualTo(test));
        }

        /// <summary>
        /// This tests the case where the expression can handle really big numbers and is able to still change the
        /// expression to a postfix expression.
        /// </summary>
        [Test]
        public void TestingMaximumCaseParseInput()
        {
            ExpressionTree expressionTree = new ExpressionTree();

            string[] parseResult = expressionTree.ParseInput("999999999+888888888");
            string[] test = { "999999999", "888888888", "+" };

            Assert.That(parseResult, Is.EqualTo(test));
        }

        /// <summary>
        /// This tests an exceptional case where we don't provide enough numbers.
        /// </summary>
        [Test]
        public void TestingExceptionalCaseParseInput()
        {
            ExpressionTree expressionTree = new ExpressionTree();

            string[] parseResult = expressionTree.ParseInput(string.Empty);

            Assert.That(parseResult, Is.EqualTo(null));
        }

        /// <summary>
        /// Testing to see if we build a tree according to the post fix expression, we assign the correct root node.
        /// </summary>
        [Test]
        public void TestingNormalBuildTree()
        {
            ExpressionTree expressionTree = new ExpressionTree();

            expressionTree.BuildTree(expressionTree.ParseInput("3+3+4+5"));

            Assert.That(expressionTree.GetRoot.Value, Is.EqualTo("+"));
            Assert.That(expressionTree.GetRoot, Is.Not.EqualTo(null));
        }

        /// <summary>
        /// Testing to see if the expression is empty, no tree is built.
        /// </summary>
        [Test]
        public void TestingExceptionalBuildTree()
        {
            ExpressionTree expressionTree = new ExpressionTree();

            expressionTree.BuildTree(expressionTree.ParseInput(string.Empty));

            Assert.That(expressionTree.GetRoot, Is.EqualTo(null));
        }

        /// <summary>
        /// Testing to see if we evaluate the correct answer with normal expression.
        /// </summary>
        [Test]
        public void TestingNormalEvaluate()
        {
            ExpressionTree expressionTree = new ExpressionTree();

            expressionTree.BuildTree(expressionTree.ParseInput("3+3+3"));

            Assert.That(expressionTree.Evaluate(), Is.EqualTo(9));
        }

        /// <summary>
        /// Testing to see if we can handle an empty string, which means our result will be 0, since the tree
        /// will be null.
        /// </summary>
        [Test]
        public void TestingMinEvaluate()
        {
            ExpressionTree expressionTree = new ExpressionTree();

            expressionTree.BuildTree(expressionTree.ParseInput(string.Empty));

            Assert.That(expressionTree.Evaluate(), Is.EqualTo(0.0));
        }

        /// <summary>
        /// Testing large values in the expression tree and seeing if they get evaluated correctly.
        /// </summary>
        [Test]
        public void TestLargeValuesEvaluate()
        {
            ExpressionTree expressionTree = new ExpressionTree();

            expressionTree.BuildTree(expressionTree.ParseInput("1000000*1000000"));

            Assert.That(expressionTree.Evaluate(), Is.EqualTo(1000000000000.0));
        }

        /// <summary>
        /// Testing to see if we handle the case where there is a division by zero.
        /// </summary>
        [Test]
        public void TestingExceptionalEvaluate()
        {
            ExpressionTree expressionTree = new ExpressionTree();

            expressionTree.BuildTree(expressionTree.ParseInput("3/0"));

            Assert.Throws<DivideByZeroException>(() => expressionTree.Evaluate());
        }
    }
}