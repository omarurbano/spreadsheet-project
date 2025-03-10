// <copyright file="MenuWrapper.cs" company="Omar Urbano-Rendon">
// Copyright (c) Omar Urbano-Rendon. All rights reserved.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using SpreadSheetEngine;

namespace ExpressionMenu
{
    /// <summary>
    /// This class is in charge of the Menu UI and supports various operations regarding displaying the menu and
    /// operations regarding to making the menu run based on user input.
    /// </summary>
    public class MenuWrapper
    {
        /// <summary>
        /// Hold the Expression Tree and its operations.
        /// </summary>
        private ExpressionTree mTree;

        /// <summary>
        /// Holds the default expression and updated expressions by the user.
        /// </summary>
        private string expression;

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuWrapper"/> class.
        /// </summary>
        public MenuWrapper()
        {
            this.mTree = new ExpressionTree();
            this.expression = "A1+B1+C1";
            this.mTree.GetTreeDictionary.Add("A1", 0.0);
            this.mTree.GetTreeDictionary.Add("B1", 0.0);
            this.mTree.GetTreeDictionary.Add("C1", 0.0);
        }

        /// <summary>
        /// This method is a helper method to wrap the get user input method and start the program.
        /// </summary>
        public void RunIt()
        {
            this.GetUserInput();
        }

        /// <summary>
        /// This will display the menu of the program, which will give the user 4 options to pick from and display
        /// the current expression.
        /// </summary>
        public void DisplayMenu()
        {
            Console.WriteLine("Menu (current expression = \"" + this.expression + "\")");
            Console.WriteLine("\t1 = Enter a new expression.");
            Console.WriteLine("\t2 = Set a variable value.");
            Console.WriteLine("\t3 = Evaluate tree.");
            Console.WriteLine("\t4 = Quit.");
        }

        /// <summary>
        /// Method will display the menu and retrieve the input from the user. Will loop if it is not a valid input
        /// from the menu displayed.
        /// </summary>
        public void GetUserInput()
        {
            int userInput = -1;
            bool validInput = false;

            // Will try to display menu and get user input. Has to be a number, if its a string, will catch the
            // the exception and run it again.
            do
            {
                try
                {
                    this.DisplayMenu();
                    userInput = Convert.ToInt32(Console.ReadLine());
                    validInput = this.EvaluateUserInput(userInput);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Error, please enter a number!");
                }
            }
            while (!validInput);
        }

        /// <summary>
        /// Evaluates the users input based on the menu displayed. Based on their input, it will decide whether to
        /// enter a new expression, set values, evaluate the tree, or quit the program.
        /// </summary>
        /// <param name="userInput">integer pick from the user.</param>
        /// <returns>boolean.</returns>
        public bool EvaluateUserInput(int userInput)
        {
            bool flag = false;

            switch (userInput)
            {
                case 1: // Entering new expression.
                    do
                    {
                        Console.WriteLine("Enter a new expression: ");
                        this.expression = Console.ReadLine();
                    }
                    while (!this.CheckEvenNumberofParenthesis());
                    this.CheckForVariables();
                    break;
                case 2: // Setting new variable values.
                    if (this.DisplayDictonary())
                    {
                        this.ChangeKeyandValue();
                    }
                    else
                    {
                        Console.WriteLine("No variables in expression");
                    }

                    break;
                case 3: // Evaluating expression tree.
                    this.mTree.BuildTree(this.mTree.ParseInput(this.expression));
                    this.mTree.SearchandAddVariableValues();
                    Console.WriteLine("Result: " + this.mTree.Evaluate());
                    break;
                case 4: // Ending program.
                    Console.WriteLine("Done");
                    flag = true;
                    break;
                default:
                    flag = false;
                    break;
            }

            return flag;
        }

        /// <summary>
        /// This method checks to see if the new expression has variables that we need to update in our dictionary.
        /// It will parse through the expression and if it finds a variable, will add to the dictionary with default
        /// 0.0 as the value.
        /// </summary>
        private void CheckForVariables()
        {
            string ssBuilder = string.Empty;
            this.mTree.GetTreeDictionary.Clear();

            // Will go through for loop to parse expression and check for variable in order to update our dictionary.
            for (int i = 0; i < this.expression.Length; i++)
            {
                // Once we reach operator, need to check if its a variable or not.
                if (this.expression[i] == '+' || this.expression[i] == '-' || this.expression[i] == '*' || this.expression[i] == '/')
                {
                    if (!string.IsNullOrEmpty(ssBuilder))
                    {
                        if (/*!char.IsDigit(ssBuilder[0])*/char.IsLetter(ssBuilder[0]))
                        {
                            this.mTree.GetTreeDictionary.Add(ssBuilder, 0.0);
                        }
                    }

                    ssBuilder = string.Empty;
                }
                else // If its not an operator, will build string to ensure we get variables as well.
                {
                    ssBuilder += this.expression[i];
                }
            }

            if (!char.IsDigit(ssBuilder[0]))
            {
                this.mTree.GetTreeDictionary.Add(ssBuilder, 0.0);
            }
        }

        /// <summary>
        /// This method will display the key of dictionary if it is not empty. Will list them for user to quickly
        /// see what variable they want to change. Returns true if not empty and false if it is.
        /// </summary>
        /// <returns>boolean.</returns>
        private bool DisplayDictonary()
        {
            int counter = 1;

            if (this.mTree.GetTreeDictionary.Count > 0)
            {
                Console.WriteLine("Type the variable (exactly as listed): ");

                // Going through each dictionary key to display to user.
                foreach (var value in this.mTree.GetTreeDictionary)
                {
                    Console.WriteLine("Variable " + counter++ + ": " + $"{value.Key}");
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Takes the user input based on the variable they selected and uses it to select the key to change the
        /// value. Converts to an array to quickly index it, as not recommended to do in a dictionary, to retrieve
        /// the key and then we change the value for the specific key.
        /// </summary>
        private void ChangeKeyandValue()
        {
            string userInput = string.Empty;
            int newValue = -1;

            do // gets the key typed in from the user
            {
                userInput = Console.ReadLine();
            }
            while (!this.mTree.GetTreeDictionary.ContainsKey(userInput)); // Checks to see if its a valid key, if not repeats

            Console.WriteLine("Enter new value: ");

            // retrieve new value and assign new value.
            newValue = Convert.ToInt32(Console.ReadLine());
            this.mTree.SetVariable(userInput, newValue);
        }

        /// <summary>
        /// This method is checking to see if there is a pair of parenthesis, if not then it will be an invalid expression.
        /// If yes then will return true.
        /// </summary>
        /// <returns>Returns bool true if even pairs, false if mismatch.</returns>
        private bool CheckEvenNumberofParenthesis()
        {
            int leftCounter = 0;
            int rightCounter = 0;
            for (int i = 0; i < this.expression.Length; i++)
            {
                if (this.expression[i] == '(')
                {
                leftCounter++;
                }

                if (this.expression[i] == ')')
                {
                    rightCounter++;
                }
            }

            if (leftCounter == rightCounter)
            {
                return true;
            }
             else
            {
                Console.WriteLine("Invalid Expression, try again");
                return false;
            }
        }
    }
}
