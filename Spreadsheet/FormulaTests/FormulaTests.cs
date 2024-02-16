using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using System.Collections.Generic;

namespace FormulaTests
{
    [TestClass]
    public class FormulaTests
    {
        [TestMethod]
        public void SimpleTest()
        {
            Formula test = new Formula("2+2");
            Assert.AreEqual(4.0,test.Evaluate(s => 3));
        }
        [TestMethod]
        public void VariableTest()
        {
            Formula test = new Formula("a1+2");
            Assert.AreEqual(5.0, test.Evaluate(s => 3));
        }
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void InvalidVariableTest()
        {
            Formula test = new Formula("1a+2");
        }
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void InvalidVariableTest2()
        {
            Formula test = new Formula("1a1+2");
        }
        [TestMethod]
        public void OperationsTest()
        {
            Formula test = new Formula("4+4*6-20/5");
            Assert.AreEqual(24.0, test.Evaluate(s => 3));
        }
        [TestMethod]
        public void ParenthesesTest()
        {
            Formula test = new Formula("(4*6)-4");
            Assert.AreEqual(20.0, test.Evaluate(s => 3));
        }
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ParenthesesErrorTest()
        {
            Formula test = new Formula("(4)+3+7");
        }
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ParethesesErrorTest2()
        {
            Formula test = new Formula("((4+4)");
        }
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ParethesesErrorTest3()
        {
            Formula test = new Formula("(4+4))");
        }
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ExtraFollowingRuleTest()
        {
            Formula test = new Formula("6 6+4");
        }
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ExtraFollowingRuleTest2()
        {
            Formula test = new Formula("a1 6+4");
        }
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ExtraFollowingRuleTest3()
        {
            Formula test = new Formula("(a1+6+4)4");
        }
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ParenthesisOperatorTest()
        {
            Formula test = new Formula("(+6+4)");
        }
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ParenthesisOperatorTest2()
        {
            Formula test = new Formula("6++4");
        }
        [TestMethod]
        public void GetVariablesTest()
        {
            Formula test = new Formula("a+b+c+a+d");
            IEnumerable<string> yee = test.GetVariables();
            IEnumerator<string> yee2 = yee.GetEnumerator();
            yee2.MoveNext();
            Assert.AreEqual("a", yee2.Current);
            yee2.MoveNext();
            Assert.AreEqual("b", yee2.Current);
            yee2.MoveNext();
            Assert.AreEqual("c", yee2.Current);
            Assert.IsTrue(yee2.MoveNext());
            Assert.AreEqual("d",yee2.Current);

        }
        [TestMethod]
        public void ToStringTest()
        {
            Formula test = new Formula("2+2");
            Assert.AreEqual("2+2", test.ToString());
        }
        [TestMethod]
        public void ToStringTest2()
        {
            Formula test = new Formula("2+2+4-(20+10)*20+ a1");
            Assert.AreEqual("2+2+4-(20+10)*20+a1", test.ToString());
        }
        [TestMethod]
        public void EqualsTest()
        {
            Formula test = new Formula("2+2+3+(60-3)");
            Formula test2 = new Formula("2+2+3+(60-3)");
            Assert.IsTrue(test.Equals(test2));
            List<string> nonFormulaTest = new List<string>();
            nonFormulaTest.Add("1");
            Assert.IsFalse(test.Equals(nonFormulaTest));
        }
        [TestMethod]
        public void OperatorsTest()
        {
            Formula test = new Formula("2+2");
            Formula test2 = new Formula("2+2"); 
            Assert.IsTrue(test == test2);
            Assert.IsFalse(test != test2);
        }
        [TestMethod]
        public void NormalizationTest()
        {
            Formula test = new Formula("a2+aB3", s => s.ToUpper(), s => true);
            Assert.AreEqual("A2+AB3", test.ToString());
        }
        [TestMethod]
        [ExpectedException(typeof(FormulaFormatException))]
        public void ValidityTest()
        {
            Formula test = new Formula("a2+_3", s => s, s => char.IsLetterOrDigit(s[0]));
        }
    }
}
