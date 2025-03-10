// <copyright file="UninitializedCellException.cs" company="Omar Urbano-Rendon">
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
    /// Class of a custom exception to let us know if we are referencing a uninitialized cell.
    /// </summary>
    public class UninitializedCellException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UninitializedCellException"/> class.
        /// </summary>
        public UninitializedCellException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UninitializedCellException"/> class.
        /// </summary>
        /// <param name="message">Message to let us know what caused the exception.</param>
        public UninitializedCellException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UninitializedCellException"/> class.
        /// </summary>
        /// <param name="message">Message to let us know what caused the exception.</param>
        /// <param name="inner">If there was an inner exception.</param>
        public UninitializedCellException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
