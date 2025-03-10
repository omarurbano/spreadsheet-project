// <copyright file="NodeFactory.cs" company="Omar Urbano-Rendon">
// Copyright (c) Omar Urbano-Rendon. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SpreadSheetEngine
{
    /// <summary>
    /// Class is in charge of getting the type of node when given an operator to evaluate. Class is static, so it
    /// cannot be instantiated.
    /// </summary>
    public class NodeFactory
    {
        /// <summary>
        /// This is the dictionary of operators we are working with and their respective precedence level.
        /// </summary>
        private static Dictionary<string, int> opDetails = new Dictionary<string, int>
        { { "+", 1 }, { "-", 1 }, { "*", 2 }, { "/", 2 }, { "^", 3 } };

        /// <summary>
        /// Dictionary of operators that are gathered based on what BinaryNode inherits from. Will also have their type.
        /// </summary>
        private Dictionary<string, Type> operators = new Dictionary<string, Type>();

        /// <summary>
        /// Initializes a new instance of the <see cref="NodeFactory"/> class.
        /// </summary>
        public NodeFactory()
        {
            this.TraverseAvailableOperators((op, type) => this.operators.Add(op, type));
        }

        /// <summary>
        /// The OnOperator represents a method signature that point to method with matching signature.
        /// </summary>
        /// <param name="op">string.</param>
        /// <param name="type">type.</param>
        private delegate void OnOperator(string op, Type type);

        /// <summary>
        /// This will evaluate the string passed into the parameter and check the dictionary for an operator. If so
        /// will return a BinaryNode. If not, then it will check to see if the first index is a digit, if so, then
        /// it will return a NumericalNode. Else it will return a VariableNode.
        /// </summary>
        /// <param name="type">string to be an operator, number or variable.</param>
        /// <returns>Node type.</returns>
        public static Node GetNodeType(string type)
        {
            if (type == "+")
            {
                return new AdditionNode();
            }
            else if (type == "-")
            {
                return new SubtractionNode();
            }
            else if (type == "*")
            {
                return new MultiplicationNode();
            }
            else if (type == "/")
            {
                return new DivisionNode();
            }
            else if (type == "^")
            {
                return new ExponentNode();
            }
            else if (char.IsDigit(type[0]))
            {
                return new NumericalNode(type);
            }
            else if (type.Length > 1 && (char.IsLetter(type[0]) && char.IsDigit(type[1]))) // Checks to see if its a letter
            {
                return new VariableNode(type);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// This method will check if the string passed in is an operator in the dictionary.
        /// </summary>
        /// <param name="op">string.</param>
        /// <returns>true if it is in the dictionary, false if it is not.</returns>
        public static bool CheckOp(string op)
        {
            return opDetails.ContainsKey(op);
        }

        /// <summary>
        /// This method will take a string and will be used as a key in the dictionary to indicate what the precedence
        /// based on the key.
        /// </summary>
        /// <param name="op">operator string.</param>
        /// <returns>Precedence of type integer.</returns>
        public static int CheckPrecedence(string op)
        {
            return opDetails[op];
        }

        /// <summary>
        /// When called, will check the keys in the filled dictionary and check to see if the operator has been implemented.
        /// If found, will return an instance of that node and return the correct operator based on the parameter. If it is
        /// not an operator, will check to see if its a variable node or numerical node.
        /// </summary>
        /// <param name="op">string that could be an operator, variable or number.</param>
        /// <returns>Node.</returns>
        /// <exception cref="Exception">If the operator is not implemented.</exception>
        public Node CreateOperatorNode(string op)
        {
            if (this.operators.ContainsKey(op))
            {
                object operatorNodeObject = System.Activator.CreateInstance(this.operators[op]);
                if (operatorNodeObject is BinaryNode)
                {
                    return (BinaryNode)operatorNodeObject;
                }
            }
            else if (char.IsDigit(op[0]))
            {
                return new NumericalNode(op);
            }
            else if (op.Length > 1 && (char.IsLetter(op[0]) && char.IsDigit(op[1]))) // Checks to see if its a letter
            {
                return new VariableNode(op);
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// Will check assembly to see what has been implemented by seeing the child nodes of BinaryNode and add each
        /// to the Dictonary in the NodeFactory class. This will allow us to ensure what operators have been implmented
        /// and can refer to the dictionary instead of hardcoding and checking for operators.
        /// </summary>
        /// <param name="onOperator">.</param>
        private void TraverseAvailableOperators(OnOperator onOperator)
        {
            // get the type declaration of OperatorNode
            Type operatorNodeType = typeof(BinaryNode);

            // Iterate over all loaded assemblies:
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                // Get all types that inherit from our OperatorNode class using LINQ
                IEnumerable<Type> operatorTypes = assembly.GetTypes().Where(type => type.IsSubclassOf(operatorNodeType));

                // Iterate over those subclasses of OperatorNode
                foreach (var type in operatorTypes)
                {
                    // for each subclass, retrieve the Operator property
                    PropertyInfo operatorField = type.GetProperty("Operator");
                    if (operatorField != null)
                    {
                        // Get the character of the Operator
                        object value = operatorField.GetValue(type);

                        if (value is string)
                        {
                            string operatorSymbol = (string)value;

                            // And invoke the function passed as parameter
                            // with the operator symbol and the operator class
                            onOperator(operatorSymbol, type);
                        }
                    }
                }
            }
        }
    }
}
