// <copyright file="NumericalNode.cs" company="Omar Urbano-Rendon">
// Copyright (c) Omar Urbano-Rendon. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SpreadSheetEngine
{
    /// <summary>
    /// Class is in charge of representing values that are numbers in an expression.
    /// </summary>
    public class NumericalNode : Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NumericalNode"/> class.
        /// </summary>
        /// <param name="newValue">Number as a string.</param>
        public NumericalNode(string newValue)
        {
            this.Value = newValue;
        }

        /// <summary>
        /// Gets or sets the value of the numerical class.
        /// </summary>
        public double NumericalValue
        {
            get { return Convert.ToDouble(this.Value); }
            set { this.Value = value.ToString(); }
        }

        /// <summary>
        /// Will return numerical value of Numerical Node when overridden evaluate is called.
        /// </summary>
        /// <returns>double.</returns>
        public override double Evaluate()
        {
            return Convert.ToDouble(this.NumericalValue);
        }
    }
}
