using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    public static class Evaluator
    {
        public delegate int Lookup(String v);

        /// <summary>
        /// Used for evaluatiing basic arithmetic math with order of operations.
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="variableEvaluator"></param>
        /// <returns>The answer to the question you input</returns>
        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
            Stack<int> values = new Stack<int>();
            Stack<string> operators = new Stack<string>();
            // I would comment what this does but... I have no idea.
            string[] substrings = Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            //iterate through an array of number and operation strings, remove whitespace as we reach them, then perform operations and output the answer to your question.
            for (int i = 0; i <= substrings.Length - 1; i++)
            {
                substrings[i] = substrings[i].Trim();
                if (int.TryParse(substrings[i], out int currentNum))
                {
                    if (checkForSign("*", operators))
                    {
                        multiply(values, operators, currentNum);
                    }
                    else if (checkForSign("/", operators))
                    {
                        divide(values, operators, currentNum);
                    }
                    else
                    {
                        values.Push(int.Parse(substrings[i]));
                    }
                }
                else if (substrings[i] != "" && Char.IsLetter(substrings[i][0]) && CheckForVariable(substrings[i]))
                {
                    int varNum = variableEvaluator(substrings[i]);
                    if (checkForSign("*", operators))
                    {
                        multiply(values, operators, varNum);
                    }
                    else if (checkForSign("/", operators))
                    {
                        divide(values, operators, varNum);
                    }
                    else
                    {
                        values.Push(varNum);
                    }
                }
                else if (substrings[i] == "+" || substrings[i] == "-")
                {
                    if (checkForSign("+", operators))
                    {
                        add(values, operators);
                    }
                    else if (checkForSign("-", operators))
                    {
                        subtract(values, operators);
                    }
                    operators.Push(substrings[i]);
                }
                else if (substrings[i] == "*" || substrings[i] == "/")
                {
                    operators.Push(substrings[i]);
                }
                else if (substrings[i] == "(")
                {
                    operators.Push(substrings[i]);
                }
                else if (substrings[i] == ")")
                {
                    if (checkForSign("+", operators))
                    {
                        add(values, operators);
                    }
                    else if (checkForSign("-", operators))
                    {
                        subtract(values, operators);
                    }
                    // Some error handling
                    if (operators.Count == 0 || operators.Pop() != "(")
                    {
                        throw new ArgumentException("You forgot a parentheses somewhere.");
                    }
                    if (checkForSign("*", operators))
                    {
                        multiply(values, operators);
                    }
                    else if (checkForSign("/", operators))
                    {
                        divide(values, operators);
                    }
                }
            }
            if (operators.Count == 0 && values.Count != 0)
            {
                return values.Pop();
            }
            else if(values.Count == 0)
            {
                throw new ArgumentException("Values stack is empty, did you put in anything?.");
            }
            else
            {
                if (checkForSign("+", operators))
                {
                    add(values, operators);
                    return values.Pop();
                }
                else if (checkForSign("-",operators))
                {
                    subtract(values, operators);
                    return values.Pop();
                }
            }
            return 0;
        }

        /// <summary>
        /// checks for a particular operation sign.
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="operators"></param>
        /// <returns>true if the operation matches the top of the operators stack, otherwise false.</returns>
        private static bool checkForSign(string operation, Stack<String> operators)
        {
            if(operators.Count != 0 && operators.Peek() == operation)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// pops values twice and operators once, then subtracts the values and pushes back their difference.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="operators"></param>
        private static void subtract(Stack<int> values, Stack<string> operators)
        {
            if (values.Count <= 1)
            {
                throw new ArgumentException("Value stack needs 2 numbers.");
            }
            operators.Pop();
            int pop1 = values.Pop();
            int pop2 = values.Pop();
            values.Push(pop2 - pop1);
        }
        /// <summary>
        /// adds together 2 popped values, and pushes back their sum.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="operators"></param>
        private static void add(Stack<int> values, Stack<string> operators)
        {
            if (values.Count <= 1)
            {
                throw new ArgumentException("Value stack needs 2 numbers.");
            }
            operators.Pop();
            int pop1 = values.Pop();
            int pop2 = values.Pop();
            values.Push(pop2 + pop1);
        }
        /// <summary>
        /// divides 2 popped values and pushes back their quotient.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="operators"></param>
        private static void divide(Stack<int> values, Stack<string> operators)
        {
            if (values.Count <= 1)
            {
                throw new ArgumentException("Value stack needs 2 numbers.");
            }
            operators.Pop();
            int pop1 = values.Pop();
            int pop2 = values.Pop();
            if (pop1 == 0)
            {
                throw new ArgumentException("Ya dun tried to divide by 0.");
            }
            values.Push(pop2 / pop1);
        }
        /// <summary>
        /// divides 2 popped values and pushes back their quotient. Overloaded for giggles.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="operators"></param>
        /// <param name="currentNum"></param>
        private static void divide(Stack<int> values, Stack<string> operators, int currentNum)
        {
            if (currentNum == 0)
            {
                throw new ArgumentException("Ya dun tried to divide by 0.");
            }
            if (values.Count == 0)
            {
                throw new ArgumentException("Value stack is empty.");
            }
            operators.Pop();
            int pop1 = values.Pop();
            values.Push(pop1 / currentNum);
        }
        /// <summary>
        /// multiplies 2 popped values and pushes back their product.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="operators"></param>
        /// <param name="currentNum"></param>
        private static void multiply(Stack<int> values, Stack<string> operators, int currentNum)
        {
            if (values.Count == 0)
            {
                throw new ArgumentException("Value stack is empty.");
            }
            operators.Pop();
                values.Push(currentNum * values.Pop());
        }
        /// <summary>
        /// multiplies 2 popped values and pushes back their product. Overloaded just for fun.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="operators"></param>
        private static void multiply(Stack<int> values, Stack<string> operators)
        {
            if (values.Count <= 1)
            {
                throw new ArgumentException("Value stack needs 2 numbers.");
            }
            operators.Pop();
            values.Push(values.Pop() * values.Pop());
        }
        /// <summary>
        /// Checks to make sure variables are input with our specifications.
        /// </summary>
        /// <param name="currString"></param>
        /// <returns>true if the variable matches our specifications past the first index being a letter, false otherwise.</returns>
        private static bool CheckForVariable(string currString)
        {
            bool numbies = false;
            for (int i = 0; i <= currString.Length - 1; i++)
            {
                if(Char.IsDigit(currString[i]))
                {
                    numbies = true;
                }
                if(numbies && !Char.IsDigit(currString[i]))
                {
                    throw new ArgumentException("Invalid variable entered.");
                }
            }
            return numbies;
        }
    }
}
