// <copyright file="SpreadSheetForm.cs" company="Omar Urbano-Rendon">
// Copyright (c) Omar Urbano-Rendon. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpreadSheetEngine;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace Spreadsheet_Omar_Urbano_Rendon
{
    /// <summary>
    /// This class is in charge of the UI and changes relevant to the Grid.
    /// </summary>
    public partial class SpreadSheetForm : Form
    {
        /// <summary>
        /// Our general command use.
        /// </summary>
        private ICommand currCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpreadSheetForm"/> class.
        /// </summary>
        public SpreadSheetForm()
        {
            this.InitializeComponent();
            this.ResetDataGridView();
            this.mSpreadsheet = new SpreadSheetEngine.SpreadSheet(50, 26);
            this.mSpreadsheet.SpreadSheetPropertyChanged += this.OnSpreadSheetCellPropertyChanged;
            this.dataGridView1.CellBeginEdit += this.OnCellBeginEdit;
            this.dataGridView1.CellEndEdit += this.OnCellEndEdit;
            this.undoToolStripMenuItem.Enabled = false;
            this.redoToolStripMenuItem.Enabled = false;
        }

        /// <summary>
        /// This method will initialize our spreadsheet on the UI side of things, It will have 29 columns and 50 rows.
        /// </summary>
        private void InitializeDataGrid()
        {
            // this.Controls.Add(this.dataGridView1);
            this.dataGridView1.ColumnCount = 26;

            for (int columnCount = 0, alphabet = 65; columnCount < 26; columnCount++, alphabet++)
            {
                // Will convert ascii value of integers to their corresponding value.
                this.dataGridView1.Columns[columnCount].Name = Convert.ToChar(alphabet).ToString();
            }

            this.dataGridView1.RowCount = 50;

            for (int rowCount = 1; rowCount < 51; rowCount++)
            {
                // writing header values for the rows.
                this.dataGridView1.Rows[rowCount - 1].HeaderCell.Value = rowCount.ToString();
            }
        }

        /// <summary>
        /// This method ensures that we clear out the gridview data prior to intializing our spreadsheet.
        /// </summary>
        private void ResetDataGridView()
        {
            this.dataGridView1.CancelEdit();
            this.dataGridView1.Columns.Clear();
            this.dataGridView1.DataSource = null;
            this.InitializeDataGrid();
        }

        /// <summary>
        /// This method is our event handler which will be invoked after the spreadsheet engine has completed its computation
        /// and only then will it update things in the UI side of things.
        /// </summary>
        /// <param name="sender">object, cell.</param>
        /// <param name="e">string.</param>
        private void OnSpreadSheetCellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is Cell cell && e.PropertyName == "Text")
            {
                this.dataGridView1[cell.ColumnIndex, cell.RowIndex].Value = ((Cell)sender).Value;
            }
            else if (sender is Cell cells && e.PropertyName == "Color")
            {
                this.dataGridView1[cells.ColumnIndex, cells.RowIndex].Style.BackColor = Color.FromArgb((int)cells.Color);
            }
        }

        /// <summary>
        /// When a cell gets edited in the UI, we will update the cell to show any formulas if the result is displayed
        /// and there was an expression to get to it to allow easy editing.
        /// </summary>
        /// <param name="sender">DataGrid.</param>
        /// <param name="e">Gives us what column and index.</param>
        private void OnCellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            string msg = string.Format("Editing Cell at ({0}, {1})", e.ColumnIndex, e.RowIndex);
            this.Text = msg;

            var currCell = this.mSpreadsheet.GetCell(e.RowIndex, e.ColumnIndex);

            if (currCell.Text == currCell.Value) // Checking to see if its text or an equation.
            {
                this.dataGridView1[e.ColumnIndex, e.RowIndex].Value = this.mSpreadsheet.GetCell(e.RowIndex, e.ColumnIndex).Text;
            }
            else
            {
                this.dataGridView1[e.ColumnIndex, e.RowIndex].Value = this.mSpreadsheet.GetCell(e.RowIndex, e.ColumnIndex).Text;
            }
        }

        /// <summary>
        /// This method allows us to know when the user is done editing to update the property and then it will
        /// invoke the property event handler where if there is a formula, it will evaluate it. If not then the
        /// text will be displayed.
        /// </summary>
        /// <param name="sender">Datagrid.</param>
        /// <param name="e">Gives us the column and row.</param>
        private void OnCellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string msg = string.Format("Finished Editing Cell at ({0}, {1})", e.ColumnIndex, e.RowIndex);
            this.Text = msg;
            if (this.mSpreadsheet.GetCell(e.RowIndex, e.ColumnIndex).Text == this.dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString())
            {
                this.dataGridView1[e.ColumnIndex, e.RowIndex].Value = this.mSpreadsheet.GetCell(e.RowIndex, e.ColumnIndex).Value;
            }
            else
            {
                try
                {
                    // Will change any variables that are lowercase to uppercase.
                    this.currCommand = new CellUndoCommand(
                        this.mSpreadsheet.GetCell(e.RowIndex, e.ColumnIndex),
                        this.dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString().ToUpper());

                    // Will create list to add command and push to the command stack.
                    List<ICommand> changeHistory = new List<ICommand>();
                    changeHistory.Add(this.currCommand);
                    this.mSpreadsheet.AddToUndo(changeHistory);
                    this.undoToolStripMenuItem.Enabled = true;
                    this.undoToolStripMenuItem.Text = "Undo Text Change";
                    this.currCommand.Execute(); // Will assign new text to the cell, invoking property changes.
                }
                catch (UninitializedCellException f) // If we encounter a reference to an uninitialized cell.
                {
                    MessageBox.Show(text: f.Message);
                    this.dataGridView1[e.ColumnIndex, e.RowIndex].Value = string.Empty;
                }
                catch (DivideByZeroException f) // Catching if one cell reference in denominator is zero.
                {
                    MessageBox.Show(text: f.Message);
                    this.mSpreadsheet.GetCell(e.RowIndex, e.ColumnIndex).Text = string.Empty;
                    this.dataGridView1[e.ColumnIndex, e.RowIndex].Value = string.Empty;
                }
                catch (UnknownCharacterException f)
                {
                    MessageBox.Show(text: f.Message);
                    this.dataGridView1[e.ColumnIndex, e.RowIndex].Value = string.Empty;
                }
                catch (InvalidOperationException)
                {
                    MessageBox.Show(text: "More operators than numbers/variables. Please retype expression");
                    this.dataGridView1[e.ColumnIndex, e.RowIndex].Value = string.Empty;
                }
                catch (NotImplementedException)
                {
                    MessageBox.Show(text: "Operator has not been implemented");
                    this.dataGridView1[e.ColumnIndex, e.RowIndex].Value = string.Empty;
                }
            }
        }

        /// <summary>
        /// This method is in charge of handling when the user clicks the button, "Demo project" and will run and generate
        /// text in 50 random cells, write text in all of column B and then write all of column A but referencing column B.
        /// </summary>
        /// <param name="sender">button.</param>
        /// <param name="e">button clicked.</param>
        private void OnPerformDemoBtnClicked(object sender, EventArgs e)
        {
            int numRandomText = 50;
            char columnB = 'B', columnA = 'A';
            string inputStringForColB = "This is Cell " + columnB;
            string inputStringForColA = "=" + columnB;

            this.SetRandomTextSpreadSheet(numRandomText);
            this.FillColumnWithText(columnB, inputStringForColB);
            this.FillColumnWithText(columnA, inputStringForColA);
        }

        /// <summary>
        /// This method is in charge of generating random indexes within the range of the spreadsheet 2D array, it will then
        /// write "CPTS 321 is cool" in the random cell.
        /// </summary>
        /// <param name="numOfCells">Number of cells to write to.</param>
        private void SetRandomTextSpreadSheet(int numOfCells)
        {
            Random random = new Random();

            // This will go through the specified number of cells and write to random places in the spreadsheet.
            for (int i = 0; i < numOfCells; i++)
            {
                var cell = this.mSpreadsheet.GetCell(random.Next(0, 50), random.Next(0, 26));
                cell.Text = "CPTS 321 is cool!";
            }
        }

        /// <summary>
        /// This method is in charge of running through a specified column and changing all the cells in that column to the
        /// string passed in through the parameters.
        /// </summary>
        /// <param name="mColumn">char to represent our column header.</param>
        /// <param name="newText">string to set to.</param>
        private void FillColumnWithText(char mColumn, string newText)
        {
            // This will go through a single column and update text for cell and make its way down the rows.
            for (int j = 0; j < this.mSpreadsheet.RowCount; j++)
            {
                var currCell = this.mSpreadsheet.GetCell(j, (int)mColumn - 'A');
                if (currCell != null)
                {
                    currCell.Text = newText + (j + 1);
                }
            }
        }

        /// <summary>
        /// If the user clicks on the menustrip option to change background color, a color dialog will be shown
        /// and they will be able to change the color, once they hit ok, it will invoke the foreach loop.
        /// </summary>
        /// <param name="sender">Menustrip.</param>
        /// <param name="e">Event argument.</param>
        private void ChangeBackgroundColorToolStripMenuItemClick(object sender, EventArgs e)
        {
            ColorDialog myDialog = new ColorDialog();
            List<ICommand> arrHistory = new List<ICommand>();

            // Keeps the user from selecting a custom color.
            myDialog.AllowFullOpen = false;

            // Allows the user to get help. (The default is false.)
            myDialog.ShowHelp = true;

            if (myDialog.ShowDialog() == DialogResult.OK)
            {
                // In case multiple cells are selected, will use a loop to add them all to the list, then push
                // to the stack once loop ends.
                foreach (DataGridViewCell selectedCell in this.dataGridView1.SelectedCells)
                {
                    this.currCommand = new CellColorCommand(
                        this.mSpreadsheet.GetCell(
                            selectedCell.RowIndex,
                            selectedCell.ColumnIndex), (uint)myDialog.Color.ToArgb());
                    arrHistory.Add(this.currCommand);
                    this.undoToolStripMenuItem.Enabled = true;
                    this.currCommand.Execute();
                }

                this.undoToolStripMenuItem.Text = "Undo Color Change";
                this.mSpreadsheet.AddToUndo(arrHistory);
            }
        }

        /// <summary>
        /// This method will undo a command which can either be a text change or color change.
        /// </summary>
        /// <param name="sender">MenuStrip.</param>
        /// <param name="e">Menustrip item.</param>
        private void UndoTextChangeToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.mSpreadsheet.GetStackUndoCount() > 0) // As long its not empty.
            {
                var topResult = this.mSpreadsheet.PeekUndoStack(); // Getting top of stack List.

                foreach (var cmd in topResult) // Will traverse list and invoke the Undo command.
                {
                    if (cmd is CellUndoCommand undoText) // Checks to see if its a text command.
                    {
                        undoText.Undo();
                        this.redoToolStripMenuItem.Text = undoText.GetRedoMessage();
                    }
                    else if (cmd is CellColorCommand undoColor) // Checks to see if its a color command.
                    {
                        undoColor.Undo();
                        this.redoToolStripMenuItem.Text = undoColor.GetRedoMessage();
                    }
                }

                this.mSpreadsheet.AddToRedo(this.mSpreadsheet.PopUndoStack());
                this.redoToolStripMenuItem.Enabled = true;

                if (this.mSpreadsheet.GetStackUndoCount() == 0) // If empty, disable button.
                {
                    this.undoToolStripMenuItem.Enabled = false;
                }
                else // Update the next change to the menustrip button text.
                {
                    var nextUndo = this.mSpreadsheet.PeekUndoStack().First();
                    if (nextUndo is CellUndoCommand textUndo)
                    {
                        this.undoToolStripMenuItem.Text = textUndo.GetUndoMessage();
                    }
                    else if (nextUndo is CellColorCommand colorUndo)
                    {
                        this.undoToolStripMenuItem.Text = colorUndo.GetUndoMessage();
                    }
                }
            }
        }

        /// <summary>
        /// This method will redo a command which can either be a text change or color change.
        /// </summary>
        /// <param name="sender">MenuStrip.</param>
        /// <param name="e">MenuStrip item.</param>
        private void RedoTextChangeToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (this.mSpreadsheet.GetStackRedoCount() > 0) // As long its not empty.
            {
                var topResult = this.mSpreadsheet.PeekRedoStack(); // Getting top of stack List.

                foreach (var cmd in topResult) // Will traverse list and invoke the Redo command.
                {
                    if (cmd is CellUndoCommand undoText) // Checks to see if its a text command.
                    {
                        undoText.Redo();
                        this.undoToolStripMenuItem.Text = undoText.GetUndoMessage();
                    }
                    else if (cmd is CellColorCommand undoColor) // Checks to see if its a color command.
                    {
                        undoColor.Redo();
                        this.undoToolStripMenuItem.Text = undoColor.GetUndoMessage();
                    }
                }

                this.mSpreadsheet.AddToUndo(this.mSpreadsheet.PopRedoStack());
                this.undoToolStripMenuItem.Enabled = true;

                if (this.mSpreadsheet.GetStackRedoCount() == 0) // If empty, disable button.
                {
                    this.redoToolStripMenuItem.Enabled = false;
                }
                else // Update the next change to the menustrip button text.
                {
                    var nextUndo = this.mSpreadsheet.PeekRedoStack().First();
                    if (nextUndo is CellUndoCommand textUndo)
                    {
                        this.redoToolStripMenuItem.Text = textUndo.GetRedoMessage();
                    }
                    else if (nextUndo is CellColorCommand colorUndo)
                    {
                        this.redoToolStripMenuItem.Text = colorUndo.GetRedoMessage();
                    }
                }
            }
        }

        /// <summary>
        /// This method will open a save dialog once the user clicks the save button. User will then be able to
        /// select where to save the file. Code from Fiboncci hw/microsoft documentation. Adjusted to fit the
        /// xml requirement instead of txt.
        /// </summary>
        /// <param name="sender">menustrip.</param>
        /// <param name="e">event args.</param>
        private void SaveToolStripMenuItemClick(object sender, EventArgs e)
        {
            Stream myStream;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*";
            saveFileDialog.FilterIndex = 2;
            saveFileDialog.RestoreDirectory = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = saveFileDialog.OpenFile()) != null) // Will open dialog and wait for user to pick a file or cancel.
                {
                    this.mSpreadsheet.SaveCurrentSpreadsheet(myStream);
                    myStream.Close();
                }
            }
        }

        /// <summary>
        /// This method will open a file dialog and user will be able to select a xml file and load it to the
        /// spreadsheet.
        /// </summary>
        /// <param name="sender">menu strip.</param>
        /// <param name="e">event arg.</param>
        private void LoadToolStripMenuItemClick(object sender, EventArgs e)
        {
            this.ResetSpreadsheetForLoad();

            var fileContent = string.Empty;
            var filePath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\"; // Default starting directory
                openFileDialog.Filter = "XML Files (*.xml)|*.xml|All Files (*.*)|*.*"; // Filter options
                openFileDialog.FilterIndex = 2; // Ability to see all files, 1 would only show txt files
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK) // Will open dialog and wait for user to pick a file or cancel.
                {
                    // Get the path of specified file
                    filePath = openFileDialog.FileName;

                    // Read the contents of the file into a stream
                    var fileStream = openFileDialog.OpenFile();
                    try
                    {
                        this.mSpreadsheet.LoadSpreadsheetFromFile(fileStream);
                        fileStream.Close();
                    }
                    catch (UninitializedCellException)
                    {
                        MessageBox.Show("File had a reference to an unintialized cell!");
                        this.ResetSpreadsheetForLoad();
                        fileStream.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Reset everything to ensure when we load, it will clear all previous data and load what was saved in the
        /// xml file.
        /// </summary>
        private void ResetSpreadsheetForLoad()
        {
            this.ResetDataGridView();
            this.mSpreadsheet = new SpreadSheetEngine.SpreadSheet(50, 26);
            this.mSpreadsheet.SpreadSheetPropertyChanged += this.OnSpreadSheetCellPropertyChanged;
            this.dataGridView1.CellBeginEdit += this.OnCellBeginEdit;
            this.dataGridView1.CellEndEdit += this.OnCellEndEdit;
            this.undoToolStripMenuItem.Enabled = false;
            this.redoToolStripMenuItem.Enabled = false;
        }
    }
}
