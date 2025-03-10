// <copyright file="SpreadSheetEngineTests.cs" company="Omar Urbano-Rendon">
// Copyright (c) Omar Urbano-Rendon. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace SpreadSheetEngine.Tests
{
    /// <summary>
    /// This will test the DLL which is the spreadsheet engine of our spreadsheet form.
    /// </summary>
    public class SpreadSheetEngineTests
    {
        /// <summary>
        /// This method will test the normal case of a 5 x 5 spreadsheet, and will call the GetCell method to ensure it works
        /// under normal circumstances. It will check the row, column and if the values and texts where the default values.
        /// </summary>
        [Test]
        public void GetCellNormalTest()
        {
            SpreadSheet test = new SpreadSheet(5, 4);
            int row = 3, col = 2;
            var testCell = test.GetCell(row, col);

            Assert.That(col, Is.EqualTo(testCell.ColumnIndex));
            Assert.That(row, Is.EqualTo(testCell.RowIndex));
            Assert.That(testCell.Value, Is.EqualTo(string.Empty));
            Assert.That(testCell.Text, Is.EqualTo(string.Empty));
        }

        /// <summary>
        /// This method will create the minimum amount of cells in a spreadsheet and call the GetCell method to ensure it is
        /// able to handle the minimum case. It will check the row, column and if the values and texts where the default values.
        /// </summary>
        [Test]
        public void GetCellMinTest()
        {
            SpreadSheet spreadSheet = new SpreadSheet(1, 1);

            int row = 0, col = 0;
            var testCell = spreadSheet.GetCell(row, col);

            Assert.That(col, Is.EqualTo(testCell.ColumnIndex));
            Assert.That(row, Is.EqualTo(testCell.RowIndex));
            Assert.That(testCell.Value, Is.EqualTo(string.Empty));
            Assert.That(testCell.Text, Is.EqualTo(string.Empty));
        }

        /// <summary>
        /// This method will create the maximum amount of cells in a spreadsheet and call the GetCell method to ensure it is
        /// able to handle the maximum case. It will check the row, column and if the values and texts where the default values.
        /// </summary>
        [Test]
        public void GetCellMaxTest()
        {
            // Defaults to max row and columns of assignment when no parameters.
            SpreadSheet spreadSheet = new SpreadSheet();

            int row = 49, col = 25;
            var testCell = spreadSheet.GetCell(row, col);

            Assert.That(col, Is.EqualTo(testCell.ColumnIndex));
            Assert.That(row, Is.EqualTo(testCell.RowIndex));
            Assert.That(testCell.Value, Is.EqualTo(string.Empty));
            Assert.That(testCell.Text, Is.EqualTo(string.Empty));
        }

        /// <summary>
        /// This method will instantiate the spreadsheet class with values that are out of range to the prgogram. It will ensure
        /// that we throw an out of range exception in order to handle an input of that sort.
        /// </summary>
        [Test]
        public void GetCellExceptionTest()
        {
            SpreadSheet spreadSheet;

            Assert.Throws<ArgumentOutOfRangeException>(() => spreadSheet = new SpreadSheet(-1, -1));
        }

        /// <summary>
        /// This method will check to see if the GetCell method will return null if we pass in invalid arguments in the parameter
        /// and ensure it can handle that situation.
        /// </summary>
        [Test]
        public void GetCellExceptionNullTest()
        {
            SpreadSheet spreadSheet = new SpreadSheet(5, 5);

            var testCell = spreadSheet.GetCell(-1, -1);

            Assert.That(testCell, Is.Null);
        }
    }
}