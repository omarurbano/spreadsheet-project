// <copyright file="HW8Tests.cs" company="Omar Urbano-Rendon">
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
    /// Testing various features from the HW8 assignemnt, this class will hold those tests.
    /// </summary>
    public class HW8Tests
    {
        /// <summary>
        /// This method is testing if a normal color is assigned to cell, if it will assign correctly.
        /// </summary>
        [Test]
        public void TestingNormalColorChange()
        {
            SpreadSheet sheet = new SpreadSheet();

            Cell? cell = sheet.GetCell(0, 0);

            if (cell != null)
            {
                cell.Color = (uint)Color.FromArgb(0xFF, 0xFA, 0x80, 0x72).ToArgb();
                Assert.That((uint)Color.Salmon.ToArgb(), Is.EqualTo(cell.Color));
            }
        }

        /// <summary>
        /// This method will take the minimum argb color in the color pallet.
        /// </summary>
        [Test]
        public void TestingMinimumColorValue()
        {
            SpreadSheet sheet = new SpreadSheet();
            Cell? cell = sheet.GetCell(0, 0);

            if (cell != null)
            {
                cell.Color = (uint)Color.Empty.ToArgb();
                Assert.That(cell.Color, Is.EqualTo(0));
            }
        }

        /// <summary>
        /// This method will test the maximum color in the argb and see if it will handle it properly.
        /// </summary>
        [Test]
        public void TestingMaximumColorValue()
        {
            SpreadSheet sheet = new SpreadSheet();
            Cell? cell = sheet.GetCell(0, 0);

            if (cell != null)
            {
                cell.Color = (uint)Color.FromArgb(255, 255, 255).ToArgb();
                Assert.That(cell.Color, Is.EqualTo((uint)Color.White.ToArgb()));
            }
        }

        /// <summary>
        /// This method will test to see if we go over the maximum color numbers, if we will throw an exception.
        /// </summary>
        [Test]
        public void TestingExceptionalColorValue()
        {
            SpreadSheet sheet = new SpreadSheet();
            Cell? cell = sheet.GetCell(0, 0);

            if (cell != null)
            {
                Assert.Throws<ArgumentException>(() => cell.Color = (uint)Color.FromArgb(256, 256, 256).ToArgb());
            }
        }

        /// <summary>
        /// This method will be testing if changes made within the CellUndoCommand will be reflected to the actual
        /// spreadsheet, this is testing a normal encounter and seeing if the undo method works as intended.
        /// </summary>
        [Test]
        public void TestNormalUndoCommand()
        {
            SpreadSheet spreadSheet = new SpreadSheet();

            Cell? cell = spreadSheet.GetCell(0, 0);

            if (cell != null)
            {
                cell.Text = "3";
                CellUndoCommand unCommand = new CellUndoCommand(spreadSheet.GetCell(0, 0), "5");
                unCommand.Execute();

                unCommand.Undo();

                Assert.That(cell.Value, Is.EqualTo("3"));
            }
        }

        /// <summary>
        /// This method will be testing if changes made within the CellUndoCommand will be reflected to the actual
        /// spreadsheet, this is testing a minimum encounter and seeing if the undo method works as intended.
        /// </summary>
        [Test]
        public void TestMinimumUndoCommand()
        {
            SpreadSheet spreadSheet = new SpreadSheet();

            Cell? cell = spreadSheet.GetCell(0, 0);

            if (cell != null)
            {
                CellUndoCommand unCommand = new CellUndoCommand(spreadSheet.GetCell(0, 0), "5");
                unCommand.Execute();

                unCommand.Undo();

                Assert.That(cell.Value, Is.EqualTo(string.Empty));
            }
        }

        /// <summary>
        /// This method will be testing if changes made within the CellUndoCommand will be reflected to the actual
        /// spreadsheet, this is testing a maximum encounter and seeing if the undo method works as intended.
        /// </summary>
        [Test]
        public void TestMaximumUndoCommand()
        {
            SpreadSheet spreadSheet = new SpreadSheet();

            Cell? cell = spreadSheet.GetCell(0, 0);

            if (cell != null)
            {
                cell.Text = string.Join("=", double.MaxValue.ToString());
                CellUndoCommand unCommand = new CellUndoCommand(spreadSheet.GetCell(0, 0), double.MaxValue.ToString() + double.MaxValue.ToString());
                unCommand.Execute();

                unCommand.Undo();

                Assert.That(cell.Value, Is.EqualTo(double.MaxValue.ToString()));
            }
        }

        /// <summary>
        /// This method will be testing if changes made within the CellUndoCommand will be reflected to the actual
        /// spreadsheet, this is testing an exceptional encounter and seeing if we throw the exception we expect.
        /// </summary>
        [Test]
        public void TestExceptionalUndoCommand()
        {
            SpreadSheet spreadSheet = new SpreadSheet();

            Cell? cell = null;

            CellUndoCommand unCommand;

            Assert.Throws<NullReferenceException>(() => unCommand = new CellUndoCommand(cell, "New Text"));
        }
    }
}
