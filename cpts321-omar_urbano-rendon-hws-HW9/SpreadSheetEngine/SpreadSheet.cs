// <copyright file="SpreadSheet.cs" company="Omar Urbano-Rendon">
// Copyright (c) Omar Urbano-Rendon. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SpreadSheetEngine
{
    /// <summary>
    /// This class is to represent the Spreadsheet application and is in charge of creating a 2D array of Cells to represent
    /// the values in indexes of a spreadsheet. Also handles all operations to manipulate the Cells.
    /// </summary>
    public class SpreadSheet
    {
        /// <summary>
        /// This is where we are keeping track of all cells that reference other cells.
        /// </summary>
        private Dictionary<Cell, List<Cell>> dependentCells;

        /// <summary>
        /// A 2D array to represent the spreadsheet of cells.
        /// </summary>
        private Cell[,] spreadSheet;

        /// <summary>
        /// Max columns count.
        /// </summary>
        private int columnCount;

        /// <summary>
        /// Max row count.
        /// </summary>
        private int rowCount;

        /// <summary>
        /// This will hold all previous states of cells in order to execute an undo.
        /// </summary>
        private Stack<List<ICommand>> undoStack;

        /// <summary>
        /// This will hold all cell states after an undo.
        /// </summary>
        private Stack<List<ICommand>> redoStack;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpreadSheet"/> class.
        /// </summary>
        /// <param name="newRow">Number of rows of cells to create.</param>
        /// <param name="newColumn">Number of columns of cells to create.</param>
        public SpreadSheet(int newRow = 50, int newColumn = 26) // Default values but can overwritten if we input a different amount.
        {
            if (newRow < 0 || newColumn < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            else
            {
                this.dependentCells = new Dictionary<Cell, List<Cell>>();
                this.spreadSheet = new Cell[newRow, newColumn];
                this.columnCount = newColumn;
                this.rowCount = newRow;
                this.undoStack = new Stack<List<ICommand>>();
                this.redoStack = new Stack<List<ICommand>>();

                for (int row = 0; row < newRow; row++)
                {
                    for (int column = 0; column < newColumn; column++)
                    {
                        this.spreadSheet[row, column] = new SpreadSheetCell(row, column);
                        this.spreadSheet[row, column].PropertyChanged += this.OnCellPropertyChanged;
                        this.spreadSheet[row, column].PropertyChanged += this.OnCellColorPropertyChanged;
                    }
                }
            }
        }

        /// <summary>
        /// This is the property changed event handler for the spreadsheet class.
        /// </summary>
        public event PropertyChangedEventHandler SpreadSheetPropertyChanged;

        /// <summary>
        /// Gets max column Count. It is read only.
        /// </summary>
        public int ColumnCount
        {
            get { return this.columnCount; }
        }

        /// <summary>
        /// Gets mex row Count. It is ready only.
        /// </summary>
        public int RowCount
        {
            get { return this.rowCount; }
        }

        /// <summary>
        /// This method will return a single cell from the 2D array based on the indexes provided in the parameters.
        /// </summary>
        /// <param name="row">int of row.</param>
        /// <param name="column">int of column.</param>
        /// <returns>Returns the specific cell at row and column passed in.</returns>
        public Cell GetCell(int row, int column)
        {
            // Check if parameters are greater than max columns and rows.
            if (row > this.RowCount || column > this.ColumnCount)
            {
                return null;
            }

            // Checks to see if parameters are less than index 0.
            else if (row < 0 || column < 0)
            {
                return null;
            }

            // returns the cell.
            else
            {
                return this.spreadSheet[row, column];
            }
        }

        /// <summary>
        /// This method will push to the Undo Stack.
        /// </summary>
        /// <param name="cellCommand">Cell Command of text or color.</param>
        public void AddToUndo(List<ICommand> cellCommand)
        {
            this.undoStack.Push(cellCommand);
        }

        /// <summary>
        /// This method will push to the Redo Stack.
        /// </summary>
        /// <param name="cellCommand">Cell Command of text or color.</param>
        public void AddToRedo(List<ICommand> cellCommand)
        {
            this.redoStack.Push(cellCommand);
        }

        /// <summary>
        /// This method will get the count of the stack.
        /// </summary>
        /// <returns>Number of Commands in Stack.</returns>
        public int GetStackUndoCount()
        {
            return this.undoStack.Count;
        }

        /// <summary>
        /// This method will get the count of the stack.
        /// </summary>
        /// <returns>Number of Commands in Stack.</returns>
        public int GetStackRedoCount()
        {
            return this.redoStack.Count;
        }

        /// <summary>
        /// This method will pop an item from the Stack.
        /// </summary>
        /// <returns>ICommand.</returns>
        public List<ICommand> PopUndoStack()
        {
            return this.undoStack.Pop();
        }

        /// <summary>
        /// This method will pop an item from the Stack.
        /// </summary>
        /// <returns>ICommand.</returns>
        public List<ICommand> PopRedoStack()
        {
            return this.redoStack.Pop();
        }

        /// <summary>
        /// This will take a peek of the top of the stack.
        /// </summary>
        /// <returns>ICommand.</returns>
        public List<ICommand> PeekUndoStack()
        {
            return this.undoStack.Peek();
        }

        /// <summary>
        /// This will take a peek of the top of the stack.
        /// </summary>
        /// <returns>ICommand.</returns>
        public List<ICommand> PeekRedoStack()
        {
            return this.redoStack.Peek();
        }

        /// <summary>
        /// This method will be able to retrieve the cells that have been edited and export to an xml file and save
        /// in the directory that the user specified.
        /// </summary>
        /// <param name="xmlStream">savefiledialog.</param>
        public void SaveCurrentSpreadsheet(Stream xmlStream)
        {
            Workbook workbook = new Workbook(this);
            workbook.GetCellDoc().Save(xmlStream);
        }

        /// <summary>
        /// This method will load the spreadsheet that was previously saved. It will take the stream open dialog that
        /// the user selected and it will read the xml file.
        /// </summary>
        /// <param name="xmlStream">open dialog.</param>
        public void LoadSpreadsheetFromFile(Stream xmlStream)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlStream);

            Workbook workbook = new Workbook();

            /*var result = */workbook.ReadXMLFiletoLoad(doc, this);
        }

        /// <summary>
        /// This method is the property change event handler for the Spreadsheet class which is invoked when there is a change
        /// in the cell text and its property is invoked. It will check to see if it has a formula or not. If it does not, text
        /// and value will be the same. If there is, then it will compute the formula and show the text from the specified cell.
        /// </summary>
        /// <param name="sender">Cell.</param>
        /// <param name="e">string regarding cell text.</param>
        private void OnCellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is Cell cell)
            {
                if (e.PropertyName == "Text")
                {
                    if (cell.Text == string.Empty)
                    {
                        var currCell = this.GetCell(cell.RowIndex, cell.ColumnIndex);
                        currCell.Text = cell.Text;
                        ((SpreadSheetCell)currCell).SetValue(currCell.Text);
                        this.SpreadSheetPropertyChanged?.Invoke(currCell, new PropertyChangedEventArgs("Text"));
                        return;
                    }
                    else if (cell.Text[0] != '=') // If there is no equals sign, then it is set as text and value.
                    {
                        // Updating a cell that is not reference by other cells.
                        if (!this.CheckIfCellIsDependent(cell))
                        {
                            var currCell = (SpreadSheetCell)this.GetCell(cell.RowIndex, cell.ColumnIndex);
                            currCell.Text = cell.Text;
                            currCell.SetValue(currCell.Text);
                            this.SpreadSheetPropertyChanged?.Invoke(currCell, new PropertyChangedEventArgs("Text"));
                        }
                        else // This means the cell was reference by another cell, we have to do a few more steps to ensure everything gets updated.
                        {
                            var currCell = (SpreadSheetCell)this.GetCell(cell.RowIndex, cell.ColumnIndex);
                            currCell.Text = cell.Text;
                            currCell.SetValue(currCell.Text);
                            this.SpreadSheetPropertyChanged?.Invoke(currCell, new PropertyChangedEventArgs("Text"));
                            this.UpdateDependencyList(cell); // Will update our dependency list and cells reference the cell.
                        }
                    }
                    else
                    {
                        ExpressionTree expressionTree = new ExpressionTree(); // Sets expression Tree.
                        bool variablesIncluded = false; // Used to check while building the tree if there is variables.

                        // Gets current cell that was edited and the text.
                        var currCell = (SpreadSheetCell)this.GetCell(cell.RowIndex, cell.ColumnIndex);
                        currCell.Text = cell.Text;

                        // If there was no variables, this is what our result will be.
                        var result = expressionTree.EvaluateExpression(cell.Text.Remove(0, 1), out variablesIncluded);

                        if (!variablesIncluded) // If we are dealing with only numbers, if will go through this.
                        {
                            if (double.IsNaN(result))
                            {
                                result = expressionTree.Evaluate(); // Since we built the tree as we go
                            }

                            currCell.SetValue(result.ToString());
                            this.SpreadSheetPropertyChanged?.Invoke(currCell, new PropertyChangedEventArgs("Text"/*currCell.Value*/));
                        }
                        else // We found variables and they will be dealt with here.
                        {
                            var currVariables = expressionTree.GetVariableNames();
                            this.AddingDependenciesToCells(cell, currVariables); // This will keep track of cell dependencies.

                            // Will determine if we are either dealing with numbers, text or with uninitialized cells.
                            if (this.GetSpreadSheetValues(currVariables))
                            {
                                if (this.IsVariablesAllValues(currVariables)) // Dealing with variables with all numbers.
                                {
                                    expressionTree.GatherCellValuesToTree(currVariables);
                                    currCell.SetValue(expressionTree.Evaluate().ToString());
                                    this.SpreadSheetPropertyChanged?.Invoke(currCell, new PropertyChangedEventArgs("Text"/*currCell.Value*/));
                                }
                                else // Dealing with text or a mix of text and numbers.
                                {
                                    currCell.SetValue(this.CombineVarResults(currVariables));
                                    this.SpreadSheetPropertyChanged?.Invoke(currCell, new PropertyChangedEventArgs("Text"/*currCell.Value*/));
                                }
                            }
                            else // Throw exception if we try to reference an uninitialized cell.
                            {
                                throw new UninitializedCellException(message: "Cell has not been initialized!");
                            }
                        }
                    }
                }
                else
                {
                    return;
                }
            }
        }

        /// <summary>
        /// When there is a color property change, it will update the spreadsheet and invoke the spreadsheet property
        /// changed passing in the current cell and the property name.
        /// </summary>
        /// <param name="sender">Cell.</param>
        /// <param name="e">Property name.</param>
        private void OnCellColorPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is Cell cell)
            {
                if (e.PropertyName == "Color")
                {
                    var currCell = (SpreadSheetCell)this.GetCell(cell.RowIndex, cell.ColumnIndex);
                    currCell.Color = cell.Color;
                    this.SpreadSheetPropertyChanged?.Invoke(currCell, new PropertyChangedEventArgs("Color"));
                }
            }
        }

        /// <summary>
        /// This method will retrieve the values of the variables if they are referenced in the expression.
        /// It will also handle if the cell has not been initialized.
        /// </summary>
        /// <param name="treeVariables">List of string, double, string.</param>
        /// <returns>boolean.</returns>
        private bool GetSpreadSheetValues(List<(string, double, string)> treeVariables)
        {
            if (treeVariables.Count > 0)
            {
                // Going through the whole list to get the values of each variable.
                for (int i = 0; i < treeVariables.Count; i++)
                {
                    var column = treeVariables[i].Item1[0] - 'A'; // Gets Column from variable
                    var row = int.Parse(treeVariables[i].Item1.Substring(1)) - 1; // Gets Row based on number
                    double dValue = double.NaN; // Set to this.

                    if (double.TryParse(this.spreadSheet[row, column].Value, out dValue))
                    {
                        if (this.spreadSheet[row, column].Value != string.Empty) // Checking to parse string.
                        {
                            treeVariables[i] = (treeVariables[i].Item1, dValue, string.Empty); // There is numerical values, add to list.
                        }
                        else
                        {
                            return false; // The cell has NOT been set
                        }
                    }
                    else if (this.spreadSheet[row, column].Text != string.Empty) // Dealing with text.
                    {
                        treeVariables[i] = (treeVariables[i].Item1, dValue, this.spreadSheet[row, column].Value);
                    }
                    else
                    {
                        return false;
                    }
                }

                return true;
            }
            else // List is empty
            {
                return false;
            }
        }

        /// <summary>
        /// This method will check to see if the list is all variables, if it is then it will return true, if not,
        /// then it will return false.
        /// </summary>
        /// <param name="treeVariables">list of string, double and string.</param>
        /// <returns>boolean.</returns>
        private bool IsVariablesAllValues(List<(string, double, string)> treeVariables)
        {
            // going through to see if the Cell in location (row, col) has text in it.
            foreach (var variable in treeVariables)
            {
                if (variable.Item3 != string.Empty)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// This method will combine all the text cells to one if the user inputs =cell + cell + cell and just so
        /// happens to be a text.
        /// </summary>
        /// <param name="treeVariables">List of string, double and string.</param>
        /// <returns>string.</returns>
        private string CombineVarResults(List<(string, double, string)> treeVariables)
        {
            string result = string.Empty;

            // Combining text into a single string.
            foreach (var variable in treeVariables)
            {
                if (variable.Item3 != string.Empty)
                {
                    result += variable.Item3 + " ";
                }
                else
                {
                    result += variable.Item2.ToString() + " ";
                }
            }

            return result.Trim();
        }

        /// <summary>
        /// This method will check to see if the cell(key) is already in the Dictionary, if it is not, then it will add
        /// it to the Dictionary and then add all the variables referenced in the expression. If it is in the list,
        /// it will clear the Values, and update them with the current variables in the tree variables.
        /// </summary>
        /// <param name="cell">Cell of current cell being updated.</param>
        /// <param name="treeVariables">List of all variables in expression.</param>
        private void AddingDependenciesToCells(Cell cell, List<(string, double, string)> treeVariables)
        {
            if (!this.dependentCells.ContainsKey(cell)) // Cell not in Dictionary.
            {
                this.dependentCells.Add(cell, new List<Cell>());
                foreach (var variable in treeVariables)
                {
                    var column = variable.Item1[0] - 'A'; // Gets Column from variable
                    var row = int.Parse(variable.Item1.Substring(1)) - 1; // Gets Row based on number
                    this.dependentCells[cell].Add(this.GetCell(row, column));
                }
            }
            else // Cell is in Dictionary, will update the variables in the List of cells.
            {
                this.dependentCells[cell].Clear();
                foreach (var variable in treeVariables)
                {
                    var column = variable.Item1[0] - 'A'; // Gets Column from variable
                    var row = int.Parse(variable.Item1.Substring(1)) - 1; // Gets Row based on number
                    this.dependentCells[cell].Add(this.GetCell(row, column));
                }
            }
        }

        /// <summary>
        /// This method check to see if the cell is being reference by any other cells in the spreadsheet, as we are keeping
        /// track of all cell references.
        /// </summary>
        /// <param name="cell">Cell.</param>
        /// <returns>True if it is in Dictionary -> List, false if it is not.</returns>
        private bool CheckIfCellIsDependent(Cell cell)
        {
            // Checking to see if cell in in list of dependent cells.
            foreach (var variable in this.dependentCells)
            {
                if (variable.Value.Contains(cell))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// This method is in charge of updating our dependency list with the updated cell. For every key that has
        /// an updated reference, we will invoke the propertychanged method in order to redo the calculation.
        /// </summary>
        /// <param name="cell">Cell with updated value.</param>
        private void UpdateDependencyList(Cell cell)
        {
            // updating cells in the dependency list.
            foreach (var variable in this.dependentCells)
            {
                if (variable.Value.Contains(cell))
                {
                    for (int i = 0; i < variable.Value.Count; i++)
                    {
                        if (variable.Value[i] == cell)
                        {
                            variable.Value[i] = cell;
                        }
                    }
                }
            }

            // Will redo calculations to ensure all layered references are computed with updated values.
            foreach (var variable in this.dependentCells)
            {
                this.OnCellPropertyChanged(this.GetCell(variable.Key.RowIndex, variable.Key.ColumnIndex), new PropertyChangedEventArgs("Text"/*variable.Key.Text*/));
            }
        }

        /// <summary>
        /// Need internal class that inherits from Cell in order to initialize an individual cell in our 2D array.
        /// </summary>
        private class SpreadSheetCell : Cell
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SpreadSheetCell"/> class.
            /// </summary>
            /// <param name="mRow">integer of specific row.</param>
            /// <param name="mColumn">integer of specific column.</param>
            public SpreadSheetCell(int mRow, int mColumn)
                : base(mRow, mColumn)
            {
            }

            /// <summary>
            /// This method only allows the Spreadsheet to edit Value.
            /// </summary>
            /// <param name="newValue">string of new Value.</param>
            internal void SetValue(string newValue)
            {
                this.value = newValue;
            }
        }
    }
}