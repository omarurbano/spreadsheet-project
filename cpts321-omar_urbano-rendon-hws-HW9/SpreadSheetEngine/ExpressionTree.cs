// <copyright file="ExpressionTree.cs" company="Omar Urbano-Rendon">
// Copyright (c) Omar Urbano-Rendon. All rights reserved.
// </copyright>

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpreadSheetEngine
{
    /// <summary>
    /// This class is in charge of operations and building of the expression tree which is made up of binarynodes,
    /// numericalnodes, and variablenodes.
    /// </summary>
    public class ExpressionTree
    {
        /// <summary>
        /// Represents the root node of the expression tree.
        /// </summary>
        private Node expressionTree;

        /// <summary>
        /// Represents what each variable is worth.
        /// </summary>
        private Dictionary<string, double> varValues;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTree"/> class.
        /// </summary>
        public ExpressionTree()
        {
            this.expressionTree = null;
            this.varValues = new Dictionary<string, double>();
        }

        /// <summary>
        /// Gets the root node of the expression tree.
        /// </summary>
        public Node GetRoot
        {
            get { return this.expressionTree; }
        }

        /// <summary>
        /// Gets or sets the private dictionary in the expressionTree class.
        /// </summary>
        public Dictionary<string, double> GetTreeDictionary
        {
            get { return this.varValues; }
            set { this.varValues = value; }
        }

        /// <summary>
        /// Sets the dictionary variables of the expression that is being evaluated.
        /// </summary>
        /// <param name="variableName">variable name.</param>
        /// <param name="variableValue">value of variable.</param>
        public void SetVariable(string variableName, double variableValue)
        {
            this.varValues[variableName] = variableValue;
        }

        /// <summary>
        /// A helper method that encapsulates the sequence of evaluating an expression.
        /// </summary>
        /// <param name="expression">infix expression as a string.</param>
        /// <param name="isThereVariables">Will update the boolean once evaluated within method.</param>
        /// <returns>result of expression as double.</returns>
        public double EvaluateExpression(string expression, out bool isThereVariables)
        {
            isThereVariables = false;

            this.BuildTree(this.ParseInput(expression));
            if (this.varValues.Count > 0)
            {
                isThereVariables = true;
            }

            // When we have a division node, we need to build the tree but not evaluate.
            return expression.Contains("/") ? double.NaN : this.Evaluate();
        }

        /// <summary>
        /// This method will be in charge of evaluating the expression tree in its entirety, used as a helper function
        /// to call the private method which will use recursion to evaluate.
        /// </summary>
        /// <returns>result of private Evaluate.</returns>
        public double Evaluate()
        {
            if (this.expressionTree != null)
            {
                return this.Evaluate(this.expressionTree);
            }
            else
            {
                return 0.0;
            }
        }

        /// <summary>
        /// This will parse through a string expression and change the order of the expression to a postfix expression
        /// an order to get the correct order that we need to calculate the expression. It uses the shunting yard
        /// algorithm to achieve this using two stacks of strings, and once we parse through the whole expression,
        /// we empty one stack to another and reverse the order.
        /// </summary>
        /// <param name="expression">infix expression of type string.</param>
        /// <returns>postfix expression or null.</returns>
        public string[] ParseInput(string expression)
        {
            Stack<string> postFixStack = new Stack<string>();
            Stack<string> opStack = new Stack<string>();
            string ssBuilder = string.Empty;

            // Parsing the string using shunting yard algorithm to ensure expression is in the correct order and
            // Stack is in post order.
            for (int i = 0; i < expression.Length; i++)
            {
                if (NodeFactory.CheckOp(expression[i].ToString())) // Checking to see if operator is in dictionary.
                {
                    if (!string.IsNullOrEmpty(ssBuilder))
                    {
                        postFixStack.Push(ssBuilder);
                        ssBuilder = string.Empty;
                    }

                    // Will evaluate the precedence of the current operator to whats in the stack.
                    this.EvaluateStackOrder(postFixStack, opStack, expression[i].ToString());
                }
                else if (expression[i] == '(') // Encounter left, add to operator stack to keep track what should go inside.
                {
                    opStack.Push("(");
                }
                else if (expression[i] == ')') // We must add operators that appeared after the left parenthesis
                {
                    if (!string.IsNullOrEmpty(ssBuilder))
                    {
                        postFixStack.Push(ssBuilder);
                        ssBuilder = string.Empty;
                    }

                    this.HandleParenthesis(opStack, postFixStack);
                }
                else if (char.IsDigit(expression[i]) || char.IsLetter(expression[i]) || expression[i] == '.') // If its not an operator, will build string to ensure we get variables as well.
                {
                    ssBuilder += expression[i];
                }
                else
                {
                    throw new UnknownCharacterException("Unknown Character found in expression");
                }
            }

            // Adding last string from the expression.
            if (!string.IsNullOrEmpty(ssBuilder))
            {
                postFixStack.Push(ssBuilder);
            }

            // Emptying opStack into the postFixStack.
            while (this.EmptyOpStack(postFixStack, opStack))
            {
            }

            var postFixString = postFixStack.Reverse().ToArray();

            return postFixString.Length == 0 ? null : postFixString; // If true returns null, if not, returns array of strings.
        }

        /// <summary>
        /// This method will build the tree working from the bottom up to the root node, based on the stack of nodes.
        /// It will parse through the postfix expression and set left and right pointers once it encounters an operator.
        /// </summary>
        /// <param name="postFixTokens">array of tokens from on the infix expression converted to postfix.</param>
        public void BuildTree(string[] postFixTokens)
        {
            // Nodes of operators to build the tree.
            Stack<Node> operatorNodeStack = new Stack<Node>();
            NodeFactory factory = new NodeFactory();

            if (postFixTokens != null)
            {
                // Parsing through the postfix expression and making nodes according to the type they are.
                for (int i = 0; i < postFixTokens.Length; i++)
                {
                    // var newNode = NodeFactory.GetNodeType(postFixTokens[i]);
                    var newNode = factory.CreateOperatorNode(postFixTokens[i].ToString());

                    if (newNode is BinaryNode binaryNode) // Checks to see if its an operator
                    {
                        binaryNode.Right = operatorNodeStack.Pop();
                        binaryNode.Left = operatorNodeStack.Pop();
                        operatorNodeStack.Push(binaryNode);
                    }
                    else // Not an operator, will be pushed to NodeStack
                    {
                        if (newNode is VariableNode varNode) // Added this to add to dictionary without menu
                        {
                            this.varValues.Add(varNode.VariableName, varNode.VariableValue);
                        }

                        operatorNodeStack.Push(newNode);
                    }
                }

                // Checking to ensure there is a node to make the root, otherwise should remain null.
                if (operatorNodeStack.Count > 0)
                {
                    this.expressionTree = operatorNodeStack.Pop();
                }
            }
        }

        /// <summary>
        /// This method will go through the whole dictionary and make sure to update each variable value to what
        /// is listed in the dictionary.
        /// </summary>
        public void SearchandAddVariableValues()
        {
            if (this.varValues.Count > 0)
            {
                foreach (var keys in this.varValues)
                {
                    this.SeachForKey(this.expressionTree, keys.Key);
                }
            }
        }

        /// <summary>
        /// This will go through each cellValue in the List and look through the Dictionary based on the key and
        /// will update the value of the variable values. Once it has gone through the whole list, it will invoke
        /// the SearchandAddVariableValues, which will search through the tree and update them in there as well.
        /// </summary>
        /// <param name="cellValues">List of (string, double, string).</param>
        public void GatherCellValuesToTree(List<(string, double, string)> cellValues)
        {
            // Since we gathered the variables names from tree, they are 100% the keys in dictionary.
            foreach (var cell in cellValues)
            {
                this.varValues[cell.Item1] = cell.Item2;
            }

            // Ensures we update values in Tree, it is assumed that the Tree is already built.
            this.SearchandAddVariableValues();
        }

        /// <summary>
        /// Helper method to call private GetVariable name that will be able to update the List passed in with the
        /// names of the variable nodes in the tree.
        /// </summary>
        /// <returns>List that is of (string, double, string).</returns>
        public List<(string, double, string)> GetVariableNames()
        {
            List<(string, double, string)> allVarNames = new List<(string, double, string)>();

            this.GetVariableNames(this.expressionTree, allVarNames);

            return allVarNames;
        }

        /// <summary>
        /// This method will traverse the tree in search of variable nodes and add the names to the List passed
        /// in the parameters, values will be set to empty string and NaN for double values.
        /// </summary>
        /// <param name="mTree">Node.</param>
        /// <param name="varResults">List of (string, double, string).</param>
        private void GetVariableNames(Node mTree, List<(string, double, string)> varResults)
        {
            if (mTree != null)
            {
                if (mTree is BinaryNode binNode)
                {
                    this.GetVariableNames(binNode.Left, varResults);
                    this.GetVariableNames(binNode.Right, varResults);
                }

                if (mTree is VariableNode varNode)
                {
                    varResults.Add((varNode.VariableName, double.NaN, string.Empty));
                }
            }
        }

        /// <summary>
        /// This will check the precedence of the operator being evaluated vs the operator at the top of the stack,
        /// if it is less than or equal to the operator, will be pushed to the postStack. If not, then it will get
        /// pushed to the operator Stack.
        /// </summary>
        /// <param name="postStack">Stack to hold number and variable strings.</param>
        /// <param name="operators">Stack to hold operator strings.</param>
        /// <param name="currOperator">string of current operator.</param>
        private void EvaluateStackOrder(Stack<string> postStack, Stack<string> operators, string currOperator)
        {
            while (operators.Count > 0 && operators.Peek() != "(" && NodeFactory.CheckPrecedence(currOperator) <= NodeFactory.CheckPrecedence(operators.Peek()))
            {
                postStack.Push(operators.Pop());
            }

            operators.Push(currOperator);
        }

        /// <summary>
        /// This method will empty the source stack to the destination stack, as long as the source stack is not
        /// empty.
        /// </summary>
        /// <param name="destStack">destination stack.</param>
        /// <param name="sourceStack">source stack.</param>
        /// <returns>true if top of stack was added to destination stack, false otherwise.</returns>
        private bool EmptyOpStack(Stack<string> destStack, Stack<string> sourceStack)
        {
            if (sourceStack.Count > 0)
            {
                destStack.Push(sourceStack.Pop());
                return true;
            }

            return false;
        }

        /// <summary>
        /// We will use recursion to evaluate the expression tree to ensure we get the correct result based on the
        /// order of operation. Part credit to Expression.cs file we had in the in class exercise to see how to evaluate
        /// the expression tree and see how to handle different types of nodes. Pre-Condition is that mTree must not
        /// be null, must have a check before running private method.
        /// </summary>
        /// <param name="mTree">Root Node and child nodes.</param>
        /// <returns>result of evaluations.</returns>
        private double Evaluate(Node mTree)
        {
            if (mTree is BinaryNode binaryNode) // If its an operator, will use recursion to get left child value and right child value
            {
                double resultLeft = this.Evaluate(binaryNode.Left);
                double resultRight = this.Evaluate(binaryNode.Right);

                if (binaryNode is DivisionNode divNode) // Keeping an eye on division nodes if right child is a 0.
                {
                    if (resultRight != 0)
                    {
                        return binaryNode.Evaluate();
                    }
                    else
                    {
                        throw new DivideByZeroException("Division by Zero, please ensure the denominator is not zero");
                    }
                }
                else // Call evaluate method which will invoke the evaluate method that is overwritten in each operator class.
                {
                    return binaryNode.Evaluate();
                }
            }

            // Each numerical and variable node has its own evaluate methods that will return their value to assign
            // to either result left or result right.
            else if (mTree is NumericalNode numericalNode)
            {
                return numericalNode.Evaluate();
            }
            else if (mTree is VariableNode variableNode)
            {
                return variableNode.Evaluate();
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// This method will search for the key in the expression tree using recursion and once it finds the the
        /// variable node matching the key, will update its value.
        /// </summary>
        /// <param name="mTree">Node.</param>
        /// <param name="key">string of variable name.</param>
        private void SeachForKey(Node mTree, string key)
        {
            if (mTree is BinaryNode binaryNode)
            {
                this.SeachForKey(binaryNode.Left, key);
                this.SeachForKey(binaryNode.Right, key);
            }
            else if (mTree is VariableNode variableNode)
            {
                if (variableNode.Value == key)
                {
                    variableNode.VariableValue = this.varValues[key];
                }
            }
        }

        /// <summary>
        /// This method will pop everything from the operator stack into the value stack until it encounters the left
        /// parenthesis, which indicates everything within the parenthesis has been added. Afterwards, we discard the
        /// left parenthesis from the operator stack.
        /// </summary>
        /// <param name="opStack">Stack with operators.</param>
        /// <param name="valStack">Stack with postfix values.</param>
        private void HandleParenthesis(Stack<string> opStack, Stack<string> valStack)
        {
            while (opStack.Count > 0 && opStack.Peek() != "(")
            {
                valStack.Push(opStack.Pop());
            }

            if (opStack.Count > 0 && opStack.Peek() == "(")
            {
                opStack.Pop();
            }
        }
    }
}
