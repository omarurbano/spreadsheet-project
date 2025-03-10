// <copyright file="CellColorCommand.cs" company="Omar Urbano-Rendon">
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
    /// This class is to keep track of all color changes in order to do redo and undo operations.
    /// </summary>
    public class CellColorCommand : ICommand
    {
        /// <summary>
        /// This string is for the MenuStrip.
        /// </summary>
        private static readonly string UndoMessage = "Undo Color Change";

        /// <summary>
        /// This string is for the MenuStrip.
        /// </summary>
        private static readonly string RedoMessage = "Redo Color Change";

        /// <summary>
        /// Will save cell passed in from the constructor.
        /// </summary>
        private Cell savedCell;

        /// <summary>
        /// Keeps track of previous text.
        /// </summary>
        private uint oldColor;

        /// <summary>
        /// Keeps track of new test.
        /// </summary>
        private uint newColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CellColorCommand"/> class.
        /// </summary>
        /// <param name="cell">Spreadsheet Cell.</param>
        /// <param name="updatedColor">new text after property changed.</param>
        public CellColorCommand(Cell cell, uint updatedColor)
        {
            this.savedCell = cell;
            this.oldColor = cell.Color;
            this.newColor = updatedColor;
        }

        /// <summary>
        /// This method will set the new text to the current cell. This will invoke the property changes.
        /// </summary>
        public void Execute()
        {
            this.savedCell.Color = this.newColor;
        }

        /// <summary>
        /// This method will undo the new text assignment from the current cell.
        /// </summary>
        public void Undo()
        {
            this.savedCell.Color = this.oldColor;
        }

        /// <summary>
        /// This method will redo the text assignment of the current cell.
        /// </summary>
        public void Redo()
        {
            this.savedCell.Color = this.newColor;
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
