// <copyright file="ICommand.cs" company="Omar Urbano-Rendon">
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
    /// Interface for it to be inherited from and implemented in the child class.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Represents an execution method to be implemented.
        /// </summary>
        void Execute();

        /// <summary>
        /// Represents an undo method to be implemented.
        /// </summary>
        void Undo();

        /// <summary>
        /// Represents a redo method to be implmented.
        /// </summary>
        void Redo();
    }
}
