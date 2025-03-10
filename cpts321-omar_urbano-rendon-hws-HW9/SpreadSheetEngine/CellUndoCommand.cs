// <copyright file="CellUndoCommand.cs" company="Omar Urbano-Rendon">
// Copyright (c) Omar Urbano-Rendon. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadSheetEngine
{
    /// <summary>
    /// This class inherits from ICommand interface to be able to have an undo and redo functionality.
    /// </summary>
    public class CellUndoCommand : ICommand
    {
        /// <summary>
        /// This string is for the MenuStrip.
        /// </summary>
        private static readonly string UndoMessage = "Undo Text Change";

        /// <summary>
        /// This string is for the MenuStrip.
        /// </summary>
        private static readonly string RedoMessage = "Redo Text Change";

        /// <summary>
        /// Represents a single cell.
        /// </summary>
        private Cell savedCell;

        /// <summary>
        /// Keeps track of previous text.
        /// </summary>
        private string oldText;

        /// <summary>
        /// Keeps track of new test.
        /// </summary>
        private string newText;

        /// <summary>
        /// Initializes a new instance of the <see cref="CellUndoCommand"/> class.
        /// </summary>
        /// <param name="cell">Spreadsheet Cell.</param>
        /// <param name="updatedText">new text after property changed.</param>
        public CellUndoCommand(Cell cell, string updatedText)
        {
            this.savedCell = cell;
            this.oldText = cell.Text;
            this.newText = updatedText;
        }

        /// <summary>
        /// This method will set the new text to the current cell. This will invoke the property changes.
        /// </summary>
        public void Execute()
        {
            this.savedCell.Text = this.newText;
        }

        /// <summary>
        /// This method will undo the new text assignment from the current cell.
        /// </summary>
        public void Undo()
        {
            this.savedCell.Text = this.oldText;
        }

        /// <summary>
        /// This method will redo the text assignment of the current cell.
        /// </summary>
        public void Redo()
        {
            this.savedCell.Text = this.newText;
        }

        /// <summary>
        /// This method will return the undo message for the menu strip.
        /// </summary>
        /// <returns>string.</returns>
        public string GetUndoMessage()
        {
            return UndoMessage;
        }

        /// <summary>
        /// This method will return the redo message for the menu strip.
        /// </summary>
        /// <returns>string.</returns>
        public string GetRedoMessage()
        {
            return RedoMessage;
        }
    }
}
