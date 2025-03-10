// <copyright file="Cell.cs" company="Omar Urbano-Rendon">
// Copyright (c) Omar Urbano-Rendon. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadSheetEngine
{
    /// <summary>
    /// This class is an abstract class that inherits from INotifyPropertyChanged in order to know when cell value changes.
    /// </summary>
    public abstract class Cell : INotifyPropertyChanged
    {
        /// <summary>
        /// text represents the cell text.
        /// </summary>
        protected string text;

        /// <summary>
        /// value represents text but if it begins with '=' character.
        /// </summary>
        protected string value;

        /// <summary>
        /// value represents color in a cell.
        /// </summary>
        protected uint color;

        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> class.
        /// </summary>
        /// <param name="newRowIndex">integer of row index.</param>
        /// <param name="newColumnIndex">integer of column index.</param>
        public Cell(int newRowIndex, int newColumnIndex)
        {
            if (newRowIndex < 0 || newColumnIndex < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            this.RowIndex = newRowIndex;
            this.ColumnIndex = newColumnIndex;
            this.value = string.Empty;
            this.text = string.Empty;
            this.color = (uint)0xFFFFFFFF;
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or Sets text. Its set detects the value is the same will return. If not, then will change text and call
        /// PropertyChanged to inform that there was a property changed.
        /// </summary>
        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                if (this.text == value)
                {
                    return;
                }

                this.text = value;
                this.PropertyChanged(this, new PropertyChangedEventArgs("Text")); // invoking property changed with parameter.
            }
        }

        /// <summary>
        /// Gets or Sets 'value' in cell class and only inheriting class can set the value in cell class.
        /// </summary>
        public string Value
        {
            get => this.value;
            protected set => this.value = value;
        }

        /// <summary>
        /// Gets or Sets the color field in the cell class.
        /// </summary>
        public uint Color
        {
            get
            {
                return this.color;
            }

            set
            {
                if (this.color == value)
                {
                    return;
                }

                this.color = value;
                this.PropertyChanged(this, new PropertyChangedEventArgs("Color"));
            }
        }

        /// <summary>
        /// Gets rowIndex and is read only.
        /// </summary>
        public int RowIndex { get; }

        /// <summary>
        /// Gets columnIndex and is read only.
        /// </summary>
        public int ColumnIndex { get; }
    }
}
