// <copyright file="VariableNode.cs" company="Omar Urbano-Rendon">
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
    /// This class is intended to hold variable values, this will include a value and name.
    /// </summary>
    public class VariableNode : Node
    {
        /// <summary>
        /// Represents the value of the variable.
        /// </summary>
        private double varValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableNode"/> class.
        /// </summary>
        /// <param name="varName">Variable Name.</param>
        /// <param name="dictImport">Dictionary to include all variables and their values.</param>
        public VariableNode(string varName, Dictionary<string, double> dictImport)
        {
            this.Value = varName;
            dictImport.TryGetValue(varName, out this.varValue);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableNode"/> class.
        /// </summary>
        /// <param name="varName">Variable name.</param>
        public VariableNode(string varName)
        {
            this.Value = varName;
            this.varValue = 0.0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableNode"/> class.
        /// </summary>
        /// <param name="newValue">Value of variable.</param>
        /// <param name="varName">name of the variable.</param>
        public VariableNode(string newValue, string varName)
        {
            this.Value = varName;
            this.varValue = Convert.ToDouble(newValue);
        }

        /// <summary>
        /// Gets or sets and sets the value of the variable class.
        /// </summary>
        public double VariableValue
        {
            get { return this.varValue; } set { this.varValue = Convert.ToDouble(value); }
        }

        /// <summary>
        /// Gets or sets the name of variable class.
        /// </summary>
        public string VariableName
        {
            get { return this.Value; } set { this.Value = value; }
        }

        /// <summary>
        /// Will return the value of the variable when evaluate is called and overriden by variable node.
        /// </summary>
        /// <returns>double.</returns>
        public override double Evaluate()
        {
            return this.varValue;
        }
    }
}
