// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {
        List<string> formulaTokens;
        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            IEnumerable<string> formulaTokensPreNormalization = GetTokens(formula);
            formulaTokens = new List<string>();
            bool restrictivePrevious = false;
            bool restrictivePrevious2 = false;
            bool openParentheses = false;
            int openParenthesesCount = 0;
            int closingParenthesesCount = 0;

            foreach (string token in formulaTokensPreNormalization)
            {
                if (Double.TryParse(token, out double dummy3))
                {
                    string convertedToken = normalize(Double.Parse(token).ToString());
                    formulaTokens.Add(convertedToken);
                }
                else
                {
                    string v = normalize(token);
                    formulaTokens.Add(v);
                }
            }
            //make sure there are tokens
            if (formulaTokens.Count == 0)
            {
                throw new FormulaFormatException("You have not put in any tokens. Try inputting something.");
            }
            //check first and last token to make sure they are numbers, variables, or a valid parentheses
            if (!Double.TryParse(formulaTokens.ElementAt(0), out double dummy) && !isValid(formulaTokens.ElementAt(0)) && formulaTokens.ElementAt(0) != "(")
            {
                throw new FormulaFormatException("your first token is not a number, variable, or opening parentheses. Change it to something valid and try again.");
            }
            if (!Double.TryParse(formulaTokens.ElementAt(0), out double dummy2) && !isValid(formulaTokens.ElementAt(0)) && formulaTokens.ElementAt(0) != ")")
            {
                throw new FormulaFormatException("your last token is not a number, variable, or closing parentheses. Change it to something valid and try again.");
            }
            //Whole lotta syntax checking, which is explained inside
            foreach (string token in formulaTokens)
            {
                // if there's an invalid token, throw an error
                if (!IsTokenValid(token) && !CheckForVariable(token))
                {
                    throw new FormulaFormatException("You have input an invalid token. Double check and make sure all tokens are valid.");
                }
                //if the previous variable was an operator or opening parenthesis, and is followed by an invalid token, throw an error
                if (restrictivePrevious && !Double.TryParse(token, out double unneeded) && !CheckForVariable(token) && token != "(")
                {
                    throw new FormulaFormatException("you have an invalid token after an operator or opening parenthesis. Check for mistakes.");
                }
                //else if the previous variable was a number, variable, or closing parenthesis, and is followed by an invalid token, throw an error.
                else if (restrictivePrevious2 && !CheckIfOperator(token) && token != ")")
                {
                    throw new FormulaFormatException("A token following a number, variable, or closing parenthesis is not an operator or closing parenthesis. Either delete the offending token or alter the way you've entered it.");
                }
                //if neither of those are true, reset the tracking variables
                else
                {
                    restrictivePrevious = false;
                    restrictivePrevious2 = false;
                }
                // detect if this token restricts what can come after it.
                if (Double.TryParse(token, out double yeehaw) || CheckForVariable(token))
                {
                    restrictivePrevious2 = true;
                }
                // detect if this token restricts what can come after it and if the parentheses are valid.
                if (token == "*" || token == "/" || token == "-" || token == "+")
                {
                    restrictivePrevious = true;
                    if (openParentheses)
                    {
                        openParentheses = false;
                    }
                }
                // detect if this token restricts what can come after it, ticks up the openParentheses counter and begin to detect if the expression in the parentheses is valid.
                else if (token == "(")
                {
                    openParenthesesCount++;
                    restrictivePrevious = true;
                    openParentheses = true;
                }
                // detect if this token restricts what comes after it, if you have too many closing parentheses, or if your expression inside of your parentheses is invalid.
                else if (token == ")")
                {
                    closingParenthesesCount++;
                    restrictivePrevious2 = true;
                    if (closingParenthesesCount > openParenthesesCount)
                    {
                        throw new FormulaFormatException("You have too many closing parentheses. Delete some or add more opening parentheses where needed.");
                    }
                }
            }
            // make sure we have an equal amount of parentheses.
            if (closingParenthesesCount != openParenthesesCount)
            {
                throw new FormulaFormatException("You do not have enough closing parentheses. Add more or delete unneeded opening parentheses.");
            }
            if (formulaTokens.Last() == "*" || formulaTokens.Last() == "+" || formulaTokens.Last() == "-"|| formulaTokens.Last() == "/")
            {
                throw new FormulaFormatException("You cannot have an operator be your last token.");
            }
            //check validity with their function
            foreach(string token in formulaTokens)
            {
                if(!isValid(token) && CheckForVariable(token))
                {
                    throw new FormulaFormatException("One of the tokens you have input fails your validity check.");
                }
            }
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            try
            {
                Stack<double> values = new Stack<double>();
                Stack<string> operators = new Stack<string>();
                //iterate through an array of number and operation strings, remove whitespace as we reach them, then perform operations and output the answer to your question.
                foreach (string token in formulaTokens)
                {
                    if (Double.TryParse(token, out double currentNum))
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
                            values.Push(Double.Parse(token));
                        }
                    }
                    else if (token != "" && Char.IsLetter(token[0]))
                    {
                        double varNum = lookup(token);
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
                    else if (token == "+" || token == "-")
                    {
                        if (checkForSign("+", operators))
                        {
                            add(values, operators);
                        }
                        else if (checkForSign("-", operators))
                        {
                            subtract(values, operators);
                        }
                        operators.Push(token);
                    }
                    else if (token == "*" || token == "/")
                    {
                        operators.Push(token);
                    }
                    else if (token == "(")
                    {
                        operators.Push(token);
                    }
                    else if (token == ")")
                    {
                        if (checkForSign("+", operators))
                        {
                            add(values, operators);
                        }
                        else if (checkForSign("-", operators))
                        {
                            subtract(values, operators);
                        }
                        operators.Pop();
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
                else
                {
                    if (checkForSign("+", operators))
                    {
                        add(values, operators);
                        return values.Pop();
                    }
                    else if (checkForSign("-", operators))
                    {
                        subtract(values, operators);
                        return values.Pop();
                    }
                }
                return 0;
            }
            catch 
            {
                return new FormulaError();
            }
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            HashSet<string> previousVariables = new HashSet<string>();
            foreach(string token in formulaTokens)
            {
                if(CheckForVariable(token) && !previousVariables.Contains(token))
                {
                    previousVariables.Add(token);
                    yield return token;
                }
            }
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            string formula = "";
            foreach(string token in formulaTokens)
            {
                formula += token;
            }
            return formula;
        }

        /// <summary>
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object obj)
        {
            if(!(obj is Formula) || obj == null)
            {
                return false;
            }
            return obj.ToString().Equals(this.ToString());
        }

        /// <summary>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return true.  If one is
        /// null and one is not, this method should return false.
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            if(ReferenceEquals(f1,null) && ReferenceEquals(f2,null))
            {
                return true;
            }
            else if (ReferenceEquals(f1, null) || ReferenceEquals(f2, null))
            {
                return false;
            }
            return f1.Equals(f2);
        }

        /// <summary>
        /// Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// Note that if both f1 and f2 are null, this method should return false.  If one is
        /// null and one is not, this method should return true.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            if (ReferenceEquals(f1, null) && ReferenceEquals(f2, null))
            {
                return false;
            }
            else if (ReferenceEquals(f1, null) || ReferenceEquals(f2, null))
            {
                return true;
            }
            return !(f1.Equals(f2));
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashcode = 1430287;
                foreach (string current in formulaTokens)
                {
                    hashcode = hashcode * 7302013 ^ current.GetHashCode();
                }
                return hashcode;
            }
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }

        private static bool checkForSign(string operation, Stack<String> operators)
        {
            if (operators.Count != 0 && operators.Peek() == operation)
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
        private static void subtract(Stack<double> values, Stack<string> operators)
        {
            if (values.Count <= 1)
            {
                throw new ArgumentException("Value stack needs 2 numbers.");
            }
            operators.Pop();
            double pop1 = values.Pop();
            double pop2 = values.Pop();
            values.Push(pop2 - pop1);
        }
        /// <summary>
        /// adds together 2 popped values, and pushes back their sum.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="operators"></param>
        private static void add(Stack<double> values, Stack<string> operators)
        {
            if (values.Count <= 1)
            {
                throw new ArgumentException("Value stack needs 2 numbers.");
            }
            operators.Pop();
            double pop1 = values.Pop();
            double pop2 = values.Pop();
            values.Push(pop2 + pop1);
        }
        /// <summary>
        /// divides 2 popped values and pushes back their quotient.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="operators"></param>
        private static void divide(Stack<double> values, Stack<string> operators)
        {
            if (values.Count <= 1)
            {
                throw new FormulaFormatException("Value stack needs 2 numbers.");
            }
            operators.Pop();
            double pop1 = values.Pop();
            double pop2 = values.Pop();
            if (pop1 == 0)
            {
                throw new FormulaFormatException("Ya dun tried to divide by 0.");
            }
            values.Push(pop2 / pop1);
        }
        /// <summary>
        /// divides 2 popped values and pushes back their quotient. Overloaded for giggles.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="operators"></param>
        /// <param name="currentNum"></param>
        private static void divide(Stack<double> values, Stack<string> operators, double currentNum)
        {
            if (currentNum == 0)
            {
                throw new FormulaFormatException("Ya dun tried to divide by 0.");
            }
            if (values.Count == 0)
            {
                throw new FormulaFormatException("Value stack is empty.");
            }
            operators.Pop();
            double pop1 = values.Pop();
            values.Push(pop1 / currentNum);
        }
        /// <summary>
        /// multiplies 2 popped values and pushes back their product.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="operators"></param>
        /// <param name="currentNum"></param>
        private static void multiply(Stack<double> values, Stack<string> operators, double currentNum)
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
        private static void multiply(Stack<double> values, Stack<string> operators)
        {
            if (values.Count <= 1)
            {
                throw new ArgumentException("Value stack needs 2 numbers.");
            }
            operators.Pop();
            values.Push(values.Pop() * values.Pop());
        }
        /// <summary>
        /// Checks if a nonvariable token is valid.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private bool IsTokenValid(string token)
        {
            if(token != "*" && token != "/" && token != "+" && token != "-" && token != "(" && token != ")" && !Double.TryParse(token,out double yee))
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Checks if a given string is an operator.
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        private static bool CheckIfOperator(string operation)
        {
            if (operation == "+" || operation == "*" || operation == "/" || operation == "-")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Checks to make sure variables are input with our specifications.
        /// </summary>
        /// <param name="currString"></param>
        /// <returns>true if the variable matches our specifications past the first index being a letter, false otherwise.</returns>
        private static bool CheckForVariable(string currString)
        {
            bool validity = false;
            if(Char.IsLetter(currString[0]) || currString[0] == '_')
            {
                validity = true;
            }
            if (currString.Length > 1)
            {
                for (int i = 1; i < currString.Length; i++)
                {
                    if (!Char.IsLetterOrDigit(currString[i]) && currString[i] != '_')
                    {
                        return false;
                    }
                }
            }
            return validity;
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}
