# Spreadsheet Application  

## Overview  
This is a C# Windows Forms-based spreadsheet application that allows users to create, edit, and manage a spreadsheet similar to Excel. The application supports basic arithmetic expressions, variable references, and now includes XML-based saving and loading functionality.  

## Features  

### Core Spreadsheet Functionality  
- Supports a 2D grid of cells, where each cell can store text or numeric values.  
- Allows arithmetic expressions (e.g., `=A1+5`) with automatic formula evaluation.  
- Supports referencing other cells dynamically, updating dependent values when a referenced cell changes.  
- Implements an undo/redo system for cell modifications.  
- Provides a background color property for cells.  

### XML-Based Loading and Saving  
- Saves spreadsheet data in an XML format, storing only cells with non-default properties.  
- Loads spreadsheet data from an XML file, preserving formulas and formatting.  
- Supports flexible XML parsing, allowing different tag orders and ignoring extra tags.  
- Clears spreadsheet data before loading a new file to prevent merging conflicts.  
- Ensures formulas are correctly evaluated after loading.  

### User Interface Enhancements  
- Menu options for saving and loading spreadsheet files.  
- Context-based updates for cell background colors.  
- Intuitive design for modifying and viewing cell contents.  

## Technical Details  
- Built using C# with WinForms for the UI.  
- Uses `System.Xml.Linq` (`XDocument`) for XML handling.  
- Implements `INotifyPropertyChanged` for efficient UI updates.  
- Follows object-oriented design principles for maintainability and scalability.  
- Adheres to Test-Driven Development (TDD) with unit tests ensuring reliability.  

## How to Use  
1. Run the application and enter values or formulas in the spreadsheet.  
2. Save your work using **File → Save** to store the spreadsheet in an XML file.  
3. Load a saved spreadsheet using **File → Load** to restore previous work.  

## Future Improvements  
- Support for additional functions and operations.  
- More advanced formatting options.  
- Improved error handling for invalid expressions.  

