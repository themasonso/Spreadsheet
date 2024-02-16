using System;
using System.Collections.Generic;
using SpreadsheetUtilities;

namespace SS
{
    public class Spreadsheet : AbstractSpreadsheet
    {
        //list of nonempty cells
        private Dictionary<string,Cell> cellList;
        //dependency graph to represent our relationships
        private DependencyGraph dependencies;
        /// <summary>
        /// Constructor that creates an empty Spreadsheet
        /// </summary>
        public Spreadsheet()
        {
            cellList = new Dictionary<string, Cell>();
            dependencies = new DependencyGraph();
        }

        public override object GetCellContents(string name)
        {
            nameChecker(name);
            if(cellList.TryGetValue(name, out Cell output))
            {
                return output.getCellContents();
            }
            return "";
        }

        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            List<string> names = new List<string>();
            return cellList.Keys;
        }

        public override IList<string> SetCellContents(string name, double number)
        {
            nameChecker(name);
            if(!cellList.TryGetValue(name, out Cell output))
            {
                Cell queriedCell = new Cell(number);
                cellList.Add(name,queriedCell);
            }
            else if(output.getCellContents() is Formula)
            {
                Formula oldFormula = (Formula)output.getCellContents();
                foreach (string current in oldFormula.GetVariables())
                {
                    dependencies.RemoveDependency(current,name);
                }
                output.setCellContents(number);
            }
            else
            {
                output.setCellContents(number);
            }
            List<string> allDependencies = new List<string>();
            foreach (string current in GetCellsToRecalculate(name))
            {
                allDependencies.Add(current);
            }
            return allDependencies;
        }

        public override IList<string> SetCellContents(string name, string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException();
            }
            nameChecker(name);
            if (!cellList.TryGetValue(name, out Cell output) && text != "")
            {
                Cell queriedCell = new Cell(text);
                cellList.Add(name, queriedCell);
            }
            else if (text == "")
            {
                return new List<string>();
            }
            else if (output.getCellContents() is Formula)
            {
                Formula oldFormula = (Formula)output.getCellContents();
                foreach (string current in oldFormula.GetVariables())
                {
                    dependencies.RemoveDependency(current, name);
                }
                output.setCellContents(text);
            }
            else
            {
                output.setCellContents(text);
            }
            List<string> allDependencies = new List<string>();
            foreach(string current in GetCellsToRecalculate(name))
            {
                allDependencies.Add(current);
            }
            return allDependencies;
        }

        public override IList<string> SetCellContents(string name, Formula formula)
        {
            if (formula == null)
            {
                throw new ArgumentNullException();
            }
            nameChecker(name);
            // this is being changed even when a circular exception is thrown. Ebic.
            foreach (string current in formula.GetVariables())
            {
                dependencies.AddDependency(current,name);
            }
            List<string> allDependencies = new List<string>();
            foreach (string current in GetCellsToRecalculate(name))
            {
                allDependencies.Add(current);
            }
            if (!cellList.TryGetValue(name, out Cell output))
            {
                Cell queriedCell = new Cell(formula);
                cellList.Add(name, queriedCell);
            }
            else if (output.getCellContents() is Formula)
            {
                Formula oldFormula = (Formula)output.getCellContents();
                foreach (string current in oldFormula.GetVariables())
                {
                    dependencies.RemoveDependency(current, name);
                }
                output.setCellContents(formula);
            }
            else
            {
                output.setCellContents(formula);
            }
            return allDependencies;
        }

        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            return dependencies.GetDependents(name);
        }
        /// <summary>
        /// Checks whether the inputted name follows our specifications. If it doesn't, throw an InvalidNameException.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private void nameChecker(string name)
        {
            if(name == null)
            {
                throw new InvalidNameException();
            }
            else if(name[0] != '_' && !Char.IsLetter(name[0]))
            {
                throw new InvalidNameException();
            }
            foreach(char c in name)
            {
                if(!Char.IsLetterOrDigit(c) && c != '_')
                {
                    throw new InvalidNameException();
                }
            }
        }
        /// <summary>
        /// Super epic Cell class that definitely works.
        /// </summary>
        private class Cell
        {
            object cellValue;
            object cellContents;
            /// <summary>
            /// constructs a cell with a formula.
            /// </summary>
            /// <param name="name"></param>
            /// <param name="Contents"></param>
            public Cell(Formula Contents)
            {
                cellContents = Contents;
                // need to compute value
            }
            /// <summary>
            /// constructs a cell with a string.
            /// </summary>
            /// <param name="Contents"></param>
            public Cell(string Contents)
            {
                cellContents = Contents;
                cellValue = Contents;
            }
            /// <summary>
            /// constructs a cell with a double.
            /// </summary>
            /// <param name="Contents"></param>
            public Cell(double Contents)
            {
                cellContents = Contents;
                cellValue = Contents;
            }
            public object getCellContents()
            {
                return cellContents;
            }
            public void setCellContents(object newContents)
            {
                cellContents = newContents;
            }

        }
    }
}
