// <copyright file="ExponentNode.cs" company="Omar Urbano-Rendon">
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
    /// The exponent class inherits from binary node and implements the evaluate method to show the result of an
    /// exponent.
    /// </summary>
    public class ExponentNode : BinaryNode
    {
        /// <summary>
        /// Static operator of ExponentNode.
        /// </summary>
        private static string nodeOperator = "^";

        /// <summary>
        /// Initializes a new instance of the <see cref="ExponentNode"/> class.
        /// </summary>
        public ExponentNode()
            : base()
        {
            this.Value = "^";
            this.NodePrecedence = 3;
        }

        /// <summary>
        /// Gets the operator of ExponentNode.
        /// </summary>
        public static string Operator
        {
            get { return nodeOperator; }
        }

        /// <summary>
        /// This method will take the left child to the power of the right child value and return the result.
        /// </summary>
        /// <returns>Result as a double.</returns>
        public override double Evaluate()
        {
            return Math.Pow(this.Left.Evaluate(), this.Right.Evaluate());
        }
    }
}
