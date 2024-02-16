using System;
using static FormulaEvaluator.Evaluator;

namespace EvaluatorTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Lookup f = yeet;
            Console.WriteLine(FormulaEvaluator.Evaluator.Evaluate("(2+2+4+6+7)*3", f));
        }
        public static int yeet(String v)
        {
            return 20;
        }
    }
}
