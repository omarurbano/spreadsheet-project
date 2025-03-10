// <copyright file="UnknownCharacterException.cs" company="Omar Urbano-Rendon">
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
    /// This class is meant to be thrown when we encounter an unknown character in our infix expression.
    /// </summary>
    public class UnknownCharacterException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownCharacterException"/> class.
        /// </summary>
        public UnknownCharacterException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownCharacterException"/> class.
        /// </summary>
        /// <param name="message">Message to let us know what caused the exception.</param>
        public UnknownCharacterException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownCharacterException"/> class.
        /// </summary>
        /// <param name="message">Message to let us know what caused the exception.</param>
        /// <param name="inner">If there was an inner exception.</param>
        public UnknownCharacterException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
