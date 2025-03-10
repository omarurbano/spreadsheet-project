// <copyright file="LogicEngineTestsHW7.cs" company="Omar Urbano-Rendon">
// Copyright (c) Omar Urbano-Rendon. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SpreadSheetEngine.Tests
{
    /// <summary>
    /// Class is tests meant for HW7 part of the spreadsheet assignment.
    /// </summary>
    public class LogicEngineTestsHW7
    {
        /// <summary>
        /// This method is testing the new implementation of getting operators that was shown in class and are testing
        /// if our program will return the correct nodes inherited from BinaryNode.
        /// </summary>
        [Test]
        public void TestFactoryClassAddition()
        {
            NodeFactory factory = new NodeFactory();
            string op = "+";

            var test = factory.CreateOperatorNode(op);

            Assert.IsInstanceOf<AdditionNode>(test);
        }

        /// <summary>
        /// This method is testing the new implementation of getting operators that was shown in class and are testing
        /// if our program will return the correct nodes inherited from BinaryNode.
        /// </summary>
        [Test]
        public void TestFactoryClassSubtraction()
        {
            NodeFactory factory = new NodeFactory();
            string op = "-";

            var test = factory.CreateOperatorNode(op);

            Assert.IsInstanceOf<SubtractionNode>(test);
        }

        /// <summary>
        /// This method is testing the new implementation of getting operators that was shown in class and are testing
        /// if our program will return the correct nodes inherited from BinaryNode.
        /// </summary>
        [Test]
        public void TestFactoryClassMultiplication()
        {
            NodeFactory factory = new NodeFactory();
            string op = "*";

            var test = factory.CreateOperatorNode(op);

            Assert.IsInstanceOf<MultiplicationNode>(test);
        }

        /// <summary>
        /// This method is testing the new implementation of getting operators that was shown in class and are testing
        /// if our program will return the correct nodes inherited from BinaryNode.
        /// </summary>
        [Test]
        public void TestFactoryClassDivision()
        {
            NodeFactory factory = new NodeFactory();
            string op = "/";

            var test = factory.CreateOperatorNode(op);

            Assert.IsInstanceOf<DivisionNode>(test);
        }

        /// <summary>
        /// This method is testing the new implementation of getting operators that was shown in class and are testing
        /// if our program will return the correct nodes inherited from BinaryNode.
        /// </summary>
        [Test]
        public void TestFactoryClassExponent()
        {
            NodeFactory factory = new NodeFactory();
            string op = "^";

            var test = factory.CreateOperatorNode(op);

            Assert.IsInstanceOf<ExponentNode>(test);
        }

        /// <summary>
        /// This test will see if it is able to get information from the spreadsheet and overwrite our list to ensure
        /// it is retrieving the cells. There will be a check outside of this method to ensure no cells were retrieved
        /// where it wasn't initialized.
        /// </summary>
        [Test]
        public void TestNormalCaseGetSpreadSheetValues()
        {
            var treeVar = new List<(string, double, string)>
            {
                ("A3", 67, string.Empty),
                ("B5", 45, string.Empty),
                ("C10", 90, string.Empty),
                ("D15", 56, string.Empty),
                ("E7", 34, string.Empty),
            };

            var refMethod = typeof(SpreadSheet).GetMethod("GetSpreadSheetValues", BindingFlags.Instance | BindingFlags.NonPublic);
            if (refMethod != null)
            {
                object? obResult = refMethod.Invoke(new SpreadSheet(), new object[] { treeVar });
                bool result = obResult is not null && (bool)obResult;

                Assert.IsFalse(result);
            }
        }

        /// <summary>
        /// This test will see if we pass into the method an empty list, if it will be able to handle that case.
        /// </summary>
        [Test]
        public void TestMinimumCaseGetSpreadSheetValues()
        {
            var treeVar = new List<(string, double, string)>();

            var refMethod = typeof(SpreadSheet).GetMethod("GetSpreadSheetValues", BindingFlags.Instance | BindingFlags.NonPublic);
            if (refMethod != null)
            {
                object? obResult = refMethod.Invoke(new SpreadSheet(), new object[] { treeVar });
                bool result = obResult is not null && (bool)obResult;

                Assert.IsFalse(result);
            }
        }

        /// <summary>
        /// This test method will test to see if mixed values will be handled properly.
        /// </summary>
        [Test]
        public void TestMaximumCaseGetSpreadSheetValues()
        {
            var treeVar = new List<(string, double, string)>
            {
                ("K13", 6, string.Empty),
                ("L20", double.NaN, "Mixed"),
                ("M2", 34, string.Empty),
                ("N17", double.NaN, "Example"),
                ("O5", 23, string.Empty),
            };

            var refMethod = typeof(SpreadSheet).GetMethod("GetSpreadSheetValues", BindingFlags.Instance | BindingFlags.NonPublic);
            if (refMethod != null)
            {
                object? obResult = refMethod.Invoke(new SpreadSheet(), new object[] { treeVar });
                bool result = obResult is not null && (bool)obResult;

                Assert.IsFalse(result);
            }
        }

        /// <summary>
        /// This method will test to see if we handle the case where both the double and string are filled.
        /// </summary>
        [Test]
        public void TestExceptionalCaseGetSpreadSheetValues()
        {
            var treeVar = new List<(string, double, string)>
            {
                ("K13", 67, "Cannot have both double and string"),
            };

            var refMethod = typeof(SpreadSheet).GetMethod("IsVariablesAllValues", BindingFlags.Instance | BindingFlags.NonPublic);
            if (refMethod != null)
            {
                object? obResult = refMethod.Invoke(new SpreadSheet(), new object[] { treeVar });
                bool result = obResult is not null && (bool)obResult;

                Assert.IsFalse(result);
            }
        }

        /// <summary>
        /// This tests if the expression gets calculated correctly dealing with only numerical values. The normal
        /// case tests a simple infix expression.
        /// </summary>
        [Test]
        public void TestNormalNumericalCalculation()
        {
            SpreadSheet spreadSheet = new SpreadSheet(5, 5);

            Cell? cellA = spreadSheet.GetCell(1, 0);

            if (cellA != null)
            {
                cellA.Text = "=3+3";

                Assert.That(cellA.Value, Is.EqualTo("6"));
            }
        }

        /// <summary>
        /// This tests if the expression gets calculated correctly dealing with only numerical values. the minimum
        /// case tests with just one number.
        /// </summary>
        [Test]
        public void TestNormalMinNumericalCalculation()
        {
            SpreadSheet spreadSheet = new SpreadSheet(5, 5);

            Cell? cellA = spreadSheet.GetCell(1, 0);

            if (cellA != null)
            {
                cellA.Text = "=3.4";

                Assert.That(cellA.Value, Is.EqualTo("3.4"));
            }
        }

        /// <summary>
        /// This tests if the expression gets calculated correctly dealing with only numerical values. The max case
        /// tests to see several operators used.
        /// </summary>
        [Test]
        public void TestNormalMaxNumericalCalculation()
        {
            SpreadSheet spreadSheet = new SpreadSheet(5, 5);

            Cell? cellA = spreadSheet.GetCell(1, 0);

            if (cellA != null)
            {
                cellA.Text = "=(3+3)^2/6";

                Assert.That(cellA.Value, Is.EqualTo("6"));
            }
        }

        /// <summary>
        /// This tests if the expression gets calculated correctly dealing with only numerical values. The exceptional
        /// case tests for invalid operators. Throws our custom exception.
        /// </summary>
        [Test]
        public void TestNormalExceptionalNumericalCalculation()
        {
            SpreadSheet spreadSheet = new SpreadSheet(5, 5);

            Cell? cellA = spreadSheet.GetCell(1, 0);

            if (cellA != null)
            {
                Assert.Throws<UnknownCharacterException>(() => cellA.Text = "=3%3");
            }
        }
    }
}
