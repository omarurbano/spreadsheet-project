// <copyright file="AdditionNode.cs" company="Omar Urbano-Rendon">
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
    /// This will represent the addition operator and inherits from BinaryNode.
    /// </summary>
    public class AdditionNode : BinaryNode
    {
        /// <summary>
        /// Static value of addition node.
        /// </summary>
        private static string nodeOperator = "+";

        /// <summary>
        /// Initializes a new instance of the <see cref="AdditionNode"/> class.
        /// </summary>
        public AdditionNode()
            : base()
        {
            this.Value = "+";
            this.NodePrecedence = 1;
        }

        /// <summary>
        /// Gets the static operator of the addition node.
        /// </summary>
        public static string Operator
        {
            get { return nodeOperator; }
        }

        /// <summary>
        /// This method will add the left child value and right child value and return the result.
        /// </summary>
        /// <returns>Result as a double.</returns>
        public override double Evaluate()
        {
            return this.Left.Evaluate() + this.Right.Evaluate();
        }
    }
}
