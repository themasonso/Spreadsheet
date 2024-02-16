using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpreadsheetUtilities;
using SS;
using System;
using System.Collections.Generic;

namespace SpreadsheetTests
{
    [TestClass]
    public class SpreadsheetTests
    {
        [TestMethod]
        public void TestSetCellContentsDouble()
        {
            Spreadsheet test = new Spreadsheet();
            Assert.IsTrue(test.SetCellContents("A1", 10).Contains("A1"));

        }
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsDoubleInvalidName()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetCellContents("1A", 10);

        }
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsStringInvalidName()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetCellContents("1A", "Yeah boy");

        }
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestSetCellContentsFormulaInvalidName()
        {
            Spreadsheet test = new Spreadsheet();
            Formula formula = new Formula("B1 + C1");
            test.SetCellContents("1A", formula);

        }
        [TestMethod]
        public void TestSetCellContentsFormulaRecursion()
        {
            Spreadsheet test = new Spreadsheet();
            Formula formula = new Formula("B1 + C1");
            Formula formula2 = new Formula("C1 + D1");
            test.SetCellContents("A1", formula);
            test.SetCellContents("B1", formula2);
            string tokens = "";
            foreach (string token in test.SetCellContents("B1", 10))
            {
                tokens += " "+token;
            }
            Assert.AreEqual(" B1 A1", tokens);

        }
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestSetCellContentsFormulaRecursionCircularException()
        {
            Spreadsheet test = new Spreadsheet();
            Formula formula = new Formula("B1 + C1");
            Formula formula2 = new Formula("C1 + D1");
            Formula formula3 = new Formula("B1 + D1");
            test.SetCellContents("A1", formula);
            test.SetCellContents("B1", formula2);
            test.SetCellContents("C1", formula3);

        }
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestSetCellContentsFormulaRecursionCircularException2()
        {
            Spreadsheet test = new Spreadsheet();
            Formula formula = new Formula("B1 + C1");
            Formula formula2 = new Formula("C1 + D1");
            Formula formula3 = new Formula("B1 + A1");
            test.SetCellContents("A1", formula);
            test.SetCellContents("B1", formula2);
            test.SetCellContents("C1", formula3);

        }
        [TestMethod]
        public void TestSetCellContentsDoubleRecursion()
        {
            Spreadsheet test = new Spreadsheet();
            Formula formula = new Formula("B1 + C1");
            Formula formula2 = new Formula("B1 + D1");
            Formula formula3 = new Formula("A1 + D1");
            test.SetCellContents("A1", formula);
            test.SetCellContents("B1", 2);
            test.SetCellContents("E1", formula2);
            test.SetCellContents("E1", formula3);
            string tokens = "";
            foreach (string token in test.SetCellContents("B1", 10))
            {
                tokens += " " + token;
            }
            Assert.AreEqual(" B1 A1 E1", tokens);

        }
        [TestMethod]
        public void TestSetCellContentsStringRecursion()
        {
            Spreadsheet test = new Spreadsheet();
            Formula formula = new Formula("B1 + C1");
            Formula formula2 = new Formula("A1 + D1");
            test.SetCellContents("A1", formula);
            test.SetCellContents("E1", formula2);
            string tokens = "";
            foreach (string token in test.SetCellContents("B1", "Yeah boy"))
            {
                tokens += " " + token;
            }
            Assert.AreEqual(" B1 A1 E1", tokens);

        }
        [TestMethod]
        public void TestGetCellContents()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetCellContents("A1", 6);
            Assert.AreEqual(6.0 ,test.GetCellContents("A1"));
        }
        [TestMethod]
        public void TestGetCellNameOfAllNonEmptyCells()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetCellContents("A1", 6);
            test.SetCellContents("B1", 8);
            string cells = "";
            foreach(string current in test.GetNamesOfAllNonemptyCells())
            {
                cells += " " + current;
            }
            Assert.AreEqual(" A1 B1", cells);
        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSetCellContentsFormulaArgumentNullException()
        {
            Spreadsheet test = new Spreadsheet();
            Formula formula = null;
            test.SetCellContents("A1", formula);

        }
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSetCellContentsStringArgumentNullException()
        {
            Spreadsheet test = new Spreadsheet();
            string yeeyee = null;
            test.SetCellContents("A1", yeeyee);

        }
        [TestMethod]
        public void TestGetCellContentsEmpty()
        {
            Spreadsheet test = new Spreadsheet();
            Assert.AreEqual("", test.GetCellContents("A1"));
        }
        [ExpectedException(typeof(InvalidNameException))]
        [TestMethod]
        public void InvalidNameTest()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetCellContents("1A",6);
        }
        [ExpectedException(typeof(InvalidNameException))]
        [TestMethod]
        public void InvalidNameTest2()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetCellContents(null, 6);
        }
        [ExpectedException(typeof(InvalidNameException))]
        [TestMethod]
        public void InvalidNameTest3()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetCellContents("A111$", 6);
        }
        [TestMethod]
        public void InvalidNameTest4()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetCellContents("_1", 6);
        }
        [TestMethod]
        public void TestSetCellContentsString()
        {
            Spreadsheet test = new Spreadsheet();
            test.SetCellContents("A1", 6);
            test.SetCellContents("A1", "yeet");
            Assert.AreEqual("yeet", test.GetCellContents("A1"));
        }
        }
}
