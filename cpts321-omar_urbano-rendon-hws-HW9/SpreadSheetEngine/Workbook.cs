// <copyright file="Workbook.cs" company="Omar Urbano-Rendon">
// Copyright (c) Omar Urbano-Rendon. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace SpreadSheetEngine
{
    /// <summary>
    /// This class will handle all operations relating reading and writing an xml file.
    /// </summary>
    public class Workbook
    {
        /// <summary>
        /// XDocument to format our xml file.
        /// </summary>
        private XDocument xSpreadsheet;

        /// <summary>
        /// List of cells that have been modifed in the spreadsheet.
        /// </summary>
        private List<Cell> listOfModifiedCells = new List<Cell>();

        /// <summary>
        /// This will contain our cell and elements of cells based on the xml file when reading.
        /// </summary>
        private Dictionary<Cell, ElementsFromXML> xmlCells = new Dictionary<Cell, ElementsFromXML>();

        /// <summary>
        /// This will contain our dependent cells from the xml file.
        /// </summary>
        private Dictionary<string, ElementsFromXML> dependentCells = new Dictionary<string, ElementsFromXML>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Workbook"/> class.
        /// </summary>
        /// <param name="workingSpreadsheet">Spreadsheet of cells.</param>
        public Workbook(SpreadSheet workingSpreadsheet)
        {
            this.GetModifiedCells(workingSpreadsheet);

            // Making an XDocument, with spreadsheet element being the outer most element.
            this.xSpreadsheet = new XDocument(new XElement(
               "SpreadSheet",
               this.listOfModifiedCells.Select(cell => // Selecting all cells in list.
                    new XElement(
                        "cell", // All other elements pertain to the cell contents.
                        new XAttribute("name", Convert.ToChar(cell.ColumnIndex + 65).ToString() + (cell.RowIndex + 1).ToString()),
                        new XElement("bgcolor", cell.Color.ToString()),
                        new XElement("text", cell.Text)))));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Workbook"/> class.
        /// </summary>
        public Workbook()
        {
            this.xmlCells = new Dictionary<Cell, ElementsFromXML>();
        }

        /// <summary>
        /// This method will return the total list count of modified cells list.
        /// </summary>
        /// <returns>number of cells in list.</returns>
        public int GetCount()
        {
            return this.listOfModifiedCells.Count;
        }

        /// <summary>
        /// This method returns the modified cells list.
        /// </summary>
        /// <returns>list of cells.</returns>
        public List<Cell> GetModifiedCellsList()
        {
            return this.listOfModifiedCells;
        }

        /// <summary>
        /// This method returns the XDocument with the xml information.
        /// </summary>
        /// <returns>xDocument.</returns>
        public XDocument GetCellDoc()
        {
            return this.xSpreadsheet;
        }

        /// <summary>
        /// This method will read the xml file and parse by the element tag names. This will then be used to
        /// fill in the spreadsheet.
        /// </summary>
        /// <param name="fileToParse">The xml file that is being read.</param>
        /// <param name="spreadSheet">The spreadsheet object.</param>
        public void ReadXMLFiletoLoad(XmlDocument fileToParse, SpreadSheet spreadSheet)
        {
            XmlNodeList cellList = fileToParse.GetElementsByTagName("cell");

            for (int i = 0; i < cellList.Count; i++)
            {
                var result = cellList[i];
                var index = result.Attributes.GetNamedItem("name").InnerText;
                if (!this.dependentCells.ContainsKey(index)) // inserting key if not already there.
                {
                    this.dependentCells[index] = default(ElementsFromXML);
                }

                // Cell cell = new Cell(index[0] - , index[1] -1); //need to pass in spreadsheet to retrieve cells.
                for (int j = 0; j < result.ChildNodes.Count; j++)
                {
                    this.SetCellFromFile(result.ChildNodes[j], index, this.dependentCells[index]);
                }
            }

            var result4 = this.dependentCells;
            Dictionary<string, ElementsFromXML> sortedDict = this.ProcessSortingDependecies(this.dependentCells);

            var result5 = this.dependentCells;

            this.SetKeyValues(sortedDict, spreadSheet);
        }

        /// <summary>
        /// This method will be able to convert the a hex string into an argb uint to ensure the UI will be able
        /// to handle the color correctly. It will take each pair of hex characters and convert them to bytes. It
        /// will then do a bitwise OR to properly convert each one. A is shifted 24 to the left, red 16 to the left,
        /// and g is 8 to the left.
        /// </summary>
        /// <param name="hexColor">hex string of the color.</param>
        /// <returns>uint of the hex string converted.</returns>
        private static uint HexToArgb(string hexColor)
        {
            if (hexColor.StartsWith("#"))
            {
                hexColor = hexColor.Substring(1); // Starts after the hash symbol.
            }

            // Parse the hex string into integer values
            byte a = 255;
            byte r = Convert.ToByte(hexColor.Substring(0, 2), 16);
            byte g = Convert.ToByte(hexColor.Substring(2, 2), 16);
            byte b = Convert.ToByte(hexColor.Substring(4, 2), 16);

            // Combine into ARGB uint
            uint argb = ((uint)a << 24) | ((uint)r << 16) | ((uint)g << 8) | b;

            return argb;
        }

        /// <summary>
        /// We are retrieving the cell from the spreadsheet class and passing in the index from the xml file.
        /// </summary>
        /// <param name="spreadSheet">spreadsheet class.</param>
        /// <param name="cellIndex">index of cell being reference.</param>
        /// <returns>cell.</returns>
        private static Cell RetrieveCellIndex(SpreadSheet spreadSheet, string cellIndex)
        {
            int column = (int)cellIndex[0] - 'A';
            int row = int.Parse(cellIndex.Substring(1)) - 1;

            Cell currCell = spreadSheet.GetCell(row, column);

            if (currCell != null)
            {
                return currCell;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// This method is able to set the text of the cell and color of the cell. It will make sure it is in the
        /// correct column since columns have A-Z and will ensure the right hex color is set.
        /// </summary>
        /// <param name="sortedElements">Elements from xml file.</param>
        /// <param name="spreadSheet">spreadsheet object.</param>
        private void SetKeyValues(Dictionary<string, ElementsFromXML> sortedElements, SpreadSheet spreadSheet)
        {
            foreach (var item in sortedElements)
            {
                string letterIndex = item.Key.ToString();
                int column = (int)letterIndex[0] - 'A';
                int row = int.Parse(letterIndex.Substring(1)) - 1;

                Cell currCell = spreadSheet.GetCell(row, column);
                if (item.Value.CellText != null)
                {
                    currCell.Text = item.Value.CellText;
                }

                if (item.Value.CellColor != null)
                {
                    currCell.Color = Convert.ToUInt32(item.Value.CellColor);
                }
            }
        }

        /// <summary>
        /// This method is in charge of gathering all edited cells from the spreadsheet.
        /// </summary>
        /// <param name="workingSpreadsheet">current working spreadsheet.</param>
        private void GetModifiedCells(SpreadSheet workingSpreadsheet)
        {
            // Will go through each row and column and checking for modified cells.
            for (int i = 0; i < workingSpreadsheet.RowCount; i++)
            {
                for (int j = 0; j < workingSpreadsheet.ColumnCount; j++)
                {
                    var cell = workingSpreadsheet.GetCell(i, j);
                    if ((cell.Text != string.Empty && cell.Value != string.Empty)
                        || cell.Color != (uint)0xFFFFFFFF)
                    {
                        this.listOfModifiedCells.Add(cell); // Adding modified cells only.
                    }
                }
            }
        }

        /// <summary>
        /// This method will send in the xmlnode and current cell and will assign the fields from what tags we
        /// are looking for in the xml file.
        /// </summary>
        /// <param name="node">Xml Node.</param>
        /// <param name="keyIndex">key index.</param>
        /// <param name="elements">Elements from XML file.</param>
        private void SetCellFromFile(XmlNode node, string keyIndex, ElementsFromXML elements)
        {
            if (node.Name == "text")
            {
                // currCell.Text = node.InnerText;
                elements.CellText = node.InnerText;
                if (elements.CellText != string.Empty && elements.CellText[0] == '=')
                {
                    elements.DependentCellsCount = this.CountDependentCells(node.InnerText);
                }
                else
                {
                    elements.DependentCellsCount = 0;
                }

                this.dependentCells[keyIndex] = elements;
            }
            else if (node.Name == "bgcolor")
            {
                uint colorCode;
                if (uint.TryParse(node.InnerText, out colorCode))
                {
                    // currCell.Color = colorCode;
                    elements.CellColor = colorCode.ToString();
                    this.dependentCells[keyIndex] = elements;
                }
                else
                {
                    // currCell.Color = HexToArgb(node.InnerText);
                    elements.CellColor = HexToArgb(node.InnerText).ToString();
                    this.dependentCells[keyIndex] = elements;
                }
            }
        }

        /// <summary>
        /// This method keeps track of the dependent cells, so when the spreadsheet is loaded, those cells are
        /// being referenced properly.
        /// </summary>
        /// <param name="text">string.</param>
        /// <returns>count.</returns>
        private int CountDependentCells(string text)
        {
            List<string> indexes = new List<string>();
            string temp = string.Empty;
            int letterCount = 0;
            int intCount = 0;

            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsLetterOrDigit(text[i]))
                {
                    temp += text[i];
                }
                else /*if (!char.IsLetterOrDigit(text[i]) || i == text.Length - 1)*/
                {
                    for (int j = 0; j < temp.Length; j++)
                    {
                        if (char.IsDigit(temp[j]))
                        {
                            intCount++;
                        }

                        if (char.IsLetter(temp[j]))
                        {
                            letterCount++;
                        }
                    }

                    if (letterCount == 1 && intCount > 0)
                    {
                        indexes.Add(temp);
                    }

                    letterCount = 0;
                    intCount = 0;
                    temp = string.Empty;
                }
            }

            letterCount = 0;
            intCount = 0;
            for (int j = 0; j < temp.Length; j++)
            {
                if (char.IsDigit(temp[j]))
                {
                    intCount++;
                }

                if (char.IsLetter(temp[j]))
                {
                    letterCount++;
                }
            }

            if (letterCount == 1 && intCount > 0)
            {
                indexes.Add(temp);
            }

            return indexes.Count;
        }

        /// <summary>
        /// This method will processes and sorts elements based on their dependencies and removes elements with
        /// no dependencies, adds them to the processed list, and updates remaining elements until all are processed.
        /// </summary>
        /// <param name="currDict">Dictionary of string and Elements from XML.</param>
        /// <returns>Sorted Dictionary.</returns>
        private Dictionary<string, ElementsFromXML> ProcessSortingDependecies(Dictionary<string, ElementsFromXML> currDict)
        {
            Dictionary<string, ElementsFromXML> proccesedElements = new Dictionary<string, ElementsFromXML>();
            do
            {
                for (int i = 0; i < currDict.Count; i++)
                {
                    if (currDict.ElementAt(i).Value.DependentCellsCount <= 0)
                    {
                        proccesedElements.Add(currDict.ElementAt(i).Key, currDict.ElementAt(i).Value);
                        currDict.Remove(currDict.ElementAt(i).Key);
                    }
                }

                this.CheckProcessedElements(proccesedElements, currDict);
            }
            while (currDict.Count > 0);

            return proccesedElements;
        }

        /// <summary>
        /// This method updates the dependency counts in currDict by checking against processed elements.
        /// It then decreases DependentCellsCount for elements in currDict if they reference keys from proccesedElements.
        /// </summary>
        /// <param name="proccesedElements">Sorted Dictionary.</param>
        /// <param name="currDict">Current Dictionary.</param>
        private void CheckProcessedElements(Dictionary<string, ElementsFromXML> proccesedElements, Dictionary<string, ElementsFromXML> currDict)
        {
            for (int i = 0; i < currDict.Count; i++)
            {
                for (int j = 0; j < proccesedElements.Count; j++)
                {
                    if (currDict.ElementAt(i).Value.CellText.Contains(proccesedElements.ElementAt(j).Key))
                    {
                        ElementsFromXML copy = currDict.ElementAt(i).Value;
                        copy.DependentCellsCount--;
                        currDict[currDict.ElementAt(i).Key] = copy;
                    }
                }
            }
        }

        /// <summary>
        /// Represents elements from a cell.
        /// </summary>
        private struct ElementsFromXML
        {
            /// <summary>
            /// Represents text from a cell.
            /// </summary>
            public string CellText;

            /// <summary>
            /// represents the color of a cell.
            /// </summary>
            public string CellColor;

            /// <summary>
            /// Represents the index.
            /// </summary>
            public string Index;

            /// <summary>
            /// Represents the dependent cell count.
            /// </summary>
            public int DependentCellsCount;
        }
    }
}
