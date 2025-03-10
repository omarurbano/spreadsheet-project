// <copyright file="BinaryNode.cs" company="Omar Urbano-Rendon">
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
    /// This class is in charge of holding operator types, it is the only node that can hold children.
    /// </summary>
    public abstract class BinaryNode : Node
    {
        /// <summary>
        /// The Left pointer to Binary Node.
        /// </summary>
        private Node pLeft;

        /// <summary>
        /// The Right pointer to BinaryNode.
        /// </summary>
        private Node pRight;

        /// <summary>
        /// This is the precedence of the operator within the object.
        /// </summary>
        private int mPrecedence;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryNode"/> class.
        /// </summary>
        public BinaryNode()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryNode"/> class.
        /// </summary>
        /// <param name="binaryValue">Operator string.</param>
        public BinaryNode(string binaryValue)
        {
            this.Value = binaryValue;
            this.pRight = this.pLeft = null;
            this.mPrecedence = NodeFactory.CheckPrecedence(binaryValue);
        }

        /// <summary>
        /// Gets or sets the left pointer within BinaryNode class.
        /// </summary>
        public Node Left
        {
            get { return this.pLeft; } set { this.pLeft = value; }
        }

        /// <summary>
        /// Gets or sets the Right pointer within BinaryNode class.
        /// </summary>
        public Node Right
        {
            get { return this.pRight; } set { this.pRight = value; }
        }

        /// <summary>
        /// Gets or sets the operator precedence within the class.
        /// </summary>
        public int NodePrecedence
        {
            get { return this.mPrecedence; }
            set { this.mPrecedence = value; }
        }
    }
}
