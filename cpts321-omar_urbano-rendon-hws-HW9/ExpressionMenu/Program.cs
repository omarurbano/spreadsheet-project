// <copyright file="Program.cs" company="Omar Urbano-Rendon">
// Copyright (c) Omar Urbano-Rendon. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpressionMenu;

namespace SpreadSheetEngine
{
    /// <summary>
    /// Default class created when added to solution.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// This is the main program that will make the Expression Menu run.
        /// </summary>
        /// <param name="args">string of arguments.</param>
        private static void Main(string[] args)
        {
            MenuWrapper prgMenu = new MenuWrapper();

            prgMenu.RunIt();
        }
    }
}
