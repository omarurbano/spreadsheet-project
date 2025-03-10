// <copyright file="RevisedEngineLogicTests.cs" company="Omar Urbano-Rendon">
// Copyright (c) Omar Urbano-Rendon. All rights reserved.
// </copyright>

using System.Linq.Expressions;
using NUnit.Framework;

namespace SpreadSheetEngine.Tests
{
    /// <summary>
    /// Class to test revised Engine logic after shunting yard and factory class implementation seen in lecture.
    /// </summary>
    public class RevisedEngineLogicTests
    {
        /// <summary>
        /// Testing factory class if given a + operator, if it will correctly identify the operator and return
        /// the correct object.
        /// </summary>
        [Test]
        public void TestFactoryClassAddition()
        {
            string op = "+";

            var test = NodeFactory.GetNodeType(op);

            Assert.IsInstanceOf<AdditionNode>(test);
        }

        /// <summary>
        /// Testing factory class if given a * operator, if it will correctly identify the operator and return
        /// the correct object.
        /// </summary>
        [Test]
        public void TestFactoryClassMultiplication()
        {
            string op = "*";

            var test = NodeFactory.GetNodeType(op);

            Assert.IsInstanceOf<MultiplicationNode>(test);
        }

        /// <summary>
        /// Testing factory class if given a / operator, if it will correctly identify the operator and return
        /// the correct object.
        /// </summary>
        [Test]
        public void TestFactoryClassDivision()
        {
            string op = "/";

            var test = NodeFactory.GetNodeType(op);

            Assert.IsInstanceOf<DivisionNode>(test);
        }

        /// <summary>
        /// Testing factory class if given a ^ operator, if it will correctly identify the operator and return
        /// the correct object.
        /// </summary>
        [Test]
        public void TestFactoryClassExponent()
        {
            string op = "^";

            var test = NodeFactory.GetNodeType(op);

            Assert.IsInstanceOf<ExponentNode>(test);
        }

        /// <summary>
        /// Testing factory class if given a variable, if it will correctly identify the variable and return
        /// the correct object.
        /// </summary>
        [Test]
        public void TestFactoryClassVariable()
        {
            string op = "A1";

            var test = NodeFactory.GetNodeType(op);

            Assert.IsInstanceOf<VariableNode>(test);
        }

        /// <summary>
        /// Testing factory class if given a number string, if it will correctly identify the number and return
        /// the correct object.
        /// </summary>
        [Test]
        public void TestFactoryClassNumerical()
        {
            string op = "100";

            var test = NodeFactory.GetNodeType(op);

            Assert.IsInstanceOf<NumericalNode>(test);
        }

        /// <summary>
        /// Testing factory class if given a string operator, if it will correctly identify that it is NOT a valid
        /// operator and throw a not implemented exception.
        /// </summary>
        [Test]
        public void TestFactoryClassNotImplemented()
        {
            string op = "!";

            Assert.Throws<NotImplementedException>(() => NodeFactory.GetNodeType(op));
        }

        /// <summary>
        /// This method will test the shunting yard algorithm to see if it can handle a normal case infix expression
        /// with parenthesis.
        /// </summary>
        [Test]
        public void TestShuntingyardNormal()
        {
            string op = "20+40*2/(17-7)";
            ExpressionTree mTree = new ExpressionTree();

            var postFix = mTree.ParseInput(op);
            string[] expectedResult = new string[] { "20", "40", "2", "*", "17", "7", "-", "/", "+" };

            Assert.That(postFix, Is.EqualTo(expectedResult));
        }

        /// <summary>
        /// This method will test the shunting yard algorithm to see if it can handle a minimum case infix expression
        /// with one pair of parenthesis.
        /// </summary>
        [Test]
        public void TestShuntingyardMinimum()
        {
            string op = "(17-7)";
            ExpressionTree mTree = new ExpressionTree();

            var postFix = mTree.ParseInput(op);
            string[] expectedResult = new string[] { "17", "7", "-" };

            Assert.That(postFix, Is.EqualTo(expectedResult));
        }

        /// <summary>
        /// This method will test the shunting yard algorithm to see if it can handle a maximum case infix expression
        /// with a lot parenthesis.
        /// </summary>
        [Test]
        public void TestShuntingyardMaximum()
        {
            string op = "((((((((((17-7)))))))))))";
            ExpressionTree mTree = new ExpressionTree();

            var postFix = mTree.ParseInput(op);
            string[] expectedResult = new string[] { "17", "7", "-" };

            Assert.That(postFix, Is.EqualTo(expectedResult));
        }

        /// <summary>
        /// This method will test the shunting yard algorithm to see if it will throw the exception due to not having
        /// the right parenthesis and it being added to the operator stack.
        /// </summary>
        [Test]
        public void TestShuntingyardExceptional()
        {
            string op = "3+(4*2";
            ExpressionTree mTree = new ExpressionTree();
            Assert.Throws<NotImplementedException>(() => mTree.BuildTree(mTree.ParseInput(op)));
        }
    }
}