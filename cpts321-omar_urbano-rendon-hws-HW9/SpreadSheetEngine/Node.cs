// <copyright file="Node.cs" company="Omar Urbano-Rendon">
// Copyright (c) Omar Urbano-Rendon. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SpreadSheetEngine
{
    /// <summary>
    /// Abstract class to be inherited by other Node classes. Has what all other node classes have in common. Class
    /// cannot be instantiated.
    /// </summary>
    public abstract class Node
    {
        /// <summary>
        /// Value to store, can represent an operator, number or variable.
        /// </summary>
        private string mValue;

        /// <summary>
        /// Gets or sets the mValue within the Node class.
        /// </summary>
        public string Value
        {
            get { return this.mValue; } set { this.mValue = value; }
        }

        /// <summary>
        /// Abstract Evaluate method to be implemented by classes that inherit from base class.
        /// </summary>
        /// <returns>double.</returns>
        public abstract double Evaluate();
    }
}
