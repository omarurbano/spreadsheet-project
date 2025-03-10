// <copyright file="DivisionNode.cs" company="Omar Urbano-Rendon">
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
    /// This will represent the Division operator and inherits from BinaryNode.
    /// </summary>
    public class DivisionNode : BinaryNode
    {
        /// <summary>
        /// Static value of division node.
        /// </summary>
        private static string nodeOperator = "/";

        /// <summary>
        /// Initializes a new instance of the <see cref="DivisionNode"/> class.
        /// </summary>
        public DivisionNode()
            : base()
        {
            this.Value = "/";
            this.NodePrecedence = 2;
        }

        /// <summary>
        /// Gets the operator of DivisionNode.
        /// </summary>
        public static string Operator
        {
            get { return nodeOperator; }
        }

        /// <summary>
        /// This method will divide the left child value and right child value and return the result.
        /// </summary>
        /// <returns>Result as a double.</returns>
        public override double Evaluate()
        {
            return this.Left.Evaluate() / this.Right.Evaluate();
        }
    }
}
