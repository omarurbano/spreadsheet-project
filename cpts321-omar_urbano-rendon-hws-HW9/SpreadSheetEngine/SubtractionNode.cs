// <copyright file="SubtractionNode.cs" company="Omar Urbano-Rendon">
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
    /// This will represent the subtraction operator and inherits from BinaryNode.
    /// </summary>
    public class SubtractionNode : BinaryNode
    {
        /// <summary>
        /// Static operator of subtraction node.
        /// </summary>
        private static string nodeOperator = "-";

        /// <summary>
        /// Initializes a new instance of the <see cref="SubtractionNode"/> class.
        /// </summary>
        public SubtractionNode()
            : base()
        {
            this.Value = "-";
            this.NodePrecedence = 1;
        }

        /// <summary>
        /// Gets the operator of Subtraction Node.
        /// </summary>
        public static string Operator
        {
            get { return nodeOperator; }
        }

        /// <summary>
        /// This method will subtract the left child value and right child value and return the result.
        /// </summary>
        /// <returns>Result as a double.</returns>
        public override double Evaluate()
        {
            return this.Left.Evaluate() - this.Right.Evaluate();
        }
    }
}
