// <copyright file="HW9Tests.cs" company="Omar Urbano-Rendon">
// Copyright (c) Omar Urbano-Rendon. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SpreadSheetEngine.Tests
{
    /// <summary>
    /// This class will test HW9 features for normal, minimumm, maximum and exceptional test cases.
    /// </summary>
    public class HW9Tests
    {
        /// <summary>
        /// This method will test if only the three edited cells are added and returned to the list of cells.
        /// </summary>
        [Test]
        public void TestNormalGatherEditedCells()
        {
            SpreadSheet spreadSheet = new SpreadSheet();

            Cell cellA1 = spreadSheet.GetCell(0, 0);
            Cell cellG5 = spreadSheet.GetCell(4, 6);
            Cell cellD8 = spreadSheet.GetCell(8, 3);

            if (cellA1 != null && cellG5 != null && cellD8 != null)
            {
                cellA1.Text = "3";
                cellD8.Text = "=6*9";
                cellG5.Color = (uint)Color.Bisque.ToArgb();

                Workbook workbook = new Workbook(spreadSheet);

                Assert.That(workbook.GetCount(), Is.EqualTo(3));
            }
        }

        /// <summary>
        /// This method will test if we pass in a spreadsheet with no edited cells, we should return with an empty
        /// list.
        /// </summary>
        [Test]
        public void TestMinGatherEditedCells()
        {
            SpreadSheet spreadSheet = new SpreadSheet();
            Workbook workbook = new Workbook(spreadSheet);

            Assert.That(workbook.GetCount(), Is.EqualTo(0));
        }

        /// <summary>
        /// This method will test if all cells are edited, then we should return all the cells from the spreadsheet.
        /// </summary>
        [Test]
        public void TestMaxGatherEditedCells()
        {
            SpreadSheet spreadSheet = new SpreadSheet();

            string testString = "This is a test";

            if (spreadSheet != null)
            {
                for (int i = 0; i < spreadSheet.RowCount; i++)
                {
                    for (int j = 0; j < spreadSheet.ColumnCount; j++)
                    {
                        Cell cell = spreadSheet.GetCell(i, j);
                        cell.Text = testString;
                        cell.Color = (uint)Color.White.ToArgb();
                    }
                }

                Workbook workbook = new Workbook(spreadSheet);

                Assert.That(workbook.GetCount(), Is.EqualTo(1300));
            }
        }

        /// <summary>
        /// This method will test what happens if we pass in a null spreadsheet.
        /// </summary>
        [Test]
        public void TestExceptionalGatherEditedCells()
        {
            SpreadSheet? spreadSheet = null;
            Workbook workbook;

            Assert.Throws<NullReferenceException>(() => workbook = new Workbook(spreadSheet));
        }
    }
}
