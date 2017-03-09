using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using ExpressionEngine;


namespace SpreadSheet
{
    public class SS
    {
        //members
        private Cell[,] Cells;

        private Dictionary<string, HashSet<string>> DependencyDict; //dictionary to keep track of dependencies


        //spreadsheet changing event handler
        public event PropertyChangedEventHandler CellPropertyChanged;

        //private class that inherits from Cell. SetValue is defined here.
        private class ss_cell : Cell
        {
            public ss_cell(int row, int col) : base(row, col) { }
            public void SetValue(string value) { Value = value; }
        }

        //constructor accepts a row index and a column index
        public SS(int row, int col)
        {
            Cells = new Cell[row, col];
            DependencyDict = new Dictionary<string, HashSet<string>>();

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    Cells[i, j] = new ss_cell(i, j);
                    Cells[i, j].PropertyChanged += OnPropertyChanged;   //subscribe to PropertyChanged event
                }
            }
        }

        //return number of rows
        public int RowCount
        {
            get { return Cells.GetLength(0); }
        }

        //return number of columns
        public int ColumnCount
        {
            get { return Cells.GetLength(1); }
        }

        //returns a cell at a specific index (row, column)
        public Cell GetCell(int row, int col)
        {
            return Cells[row, col];
        }

        public Cell GetCell(string location)
        {
            char letter = location[0];
            Int16 number;
            Cell result;

            if (!Char.IsLetter(letter)) // Doesn't begin with letter
            {
                return null;
            }

            if (!Int16.TryParse(location.Substring(1), out number)) // Doesn't have number
            {
                return null;
            }

            //assuming that we do not get a variable that is out of range. otherwise, we would need a try catch block that 
            //would be pretty easy to set up.
            result = GetCell(number - 1, letter - 'A');

            return result;  // Return cell
        }

        private void setVariable(ExpTree exp, string var)
        {
            Cell c = GetCell(var);
            double value;
            if (var == "" || var == null) //cant be a variable if empty or null
            {
                exp.SetVar(c.name, 0);
            }

            //not a value
            else if (!double.TryParse(c.value, out value))
            {
                exp.SetVar(var, 0);

            }

            else
            {
                exp.SetVar(var, value);
            }
        }

        //evaluate cell accepts a cell as an argument and checks to see if it is an expression
        // or just a string. if it begins with '=' it is an expression and will be evaluated as such.
        //otherwise the text will just be reassigned.
        private void EvaluateCell(Cell cell)
        {
            ss_cell c = cell as ss_cell;

            //cell represents an expression
            if (c.text[0] == '=')
            {
                //chop off  '='
                string expression = c.text.Substring(1);
                string[] ABC = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };

                //creates an expression tree based off of the expression
                ExpTree exp = new ExpTree(expression);

                //get a list of the variables
                string[] vars = exp.getVars();

                foreach (string variableName in vars)
                {
                    if (GetCell(variableName) == null) //does not reference an actual cell
                    {
                        break;
                    }
                    setVariable(exp, variableName);

                }
                c.SetValue(exp.Eval().ToString());
                CellPropertyChanged(cell, new PropertyChangedEventArgs("value"));




                //trigger cellpropertychanged
                CellPropertyChanged(cell, new PropertyChangedEventArgs("text"));

            }
            //not an expression
            else
            {
                c.SetValue(c.text);
                CellPropertyChanged(cell, new PropertyChangedEventArgs("value"));
            }

            if (DependencyDict.ContainsKey(c.name))
            {
                foreach (string dependentCell in DependencyDict[c.name])
                {
                    EvaluateCell(GetCell(dependentCell));
                }
            }
            //else
            //{
            //    //just set the value to the text of the node passed in
            //    c.SetValue(c.text);

            //    //event trigger.
            //    CellPropertyChanged(cell, new PropertyChangedEventArgs("value"));

            //}
        }

        //creates dependencies when one cell depends on another.
        private void MakeDependencies(string name, string[] vars)
        {
            foreach (string variable in vars)
            {
                if (!DependencyDict.ContainsKey(variable))
                {
                    DependencyDict[variable] = new HashSet<string>();
                }

                //add to the list of dependenies
                DependencyDict[variable].Add(name);
            }
        }

        //will remove a dependency that is no longer needed.
        private void RemoveDependencies(string name)
        {
            List<string> ListOfDependencies = new List<string>();

            foreach (string str in DependencyDict.Keys)  //check every variable
            {
                if (DependencyDict[str].Contains(name))
                {
                    ListOfDependencies.Add(str);
                }
            }

            foreach (string str in ListOfDependencies)
            {
                HashSet<string> DependencySet = DependencyDict[str];
                if (DependencySet.Contains(name))
                {
                    DependencySet.Remove(name);
                }
            }
        }

        //OnPropertyChanged is what is happening when a cell's PropertyChanged event is triggered. 
        //it simply evaluates the cell and sends the update to the next layer.
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "text")
            {
                ss_cell c = sender as ss_cell;
                RemoveDependencies(c.name);//remove dependencies here
                if (c.text != "" && c.text[0] == '=')
                {
                    ExpTree exp = new ExpTree(c.text.Substring(1));
                    MakeDependencies(c.name, exp.getVars());
                }

                EvaluateCell(sender as Cell);
            }

        }

        //test to make sure processing a cell such as '=A1' works and that placing data in cells works.
        public void RandomSpreadSheet()
        {
            int i = 0;
            Random rand = new Random();
            while (i < 50)
            {
                int randCol = rand.Next(0, 25);
                int randRow = rand.Next(0, 49);
                i++;

                Cell c = this.GetCell(randRow, randCol);
                c.text = "hello!";
                this.Cells[randRow, randCol] = c;


            }
            for (i = 0; i < 50; i++)
            {
                this.Cells[i, 1].text = "this is Cell B" + (i + 1).ToString();
            }
            for (int j = 0; j < 50; j++)
            {
                this.Cells[j, 0].text = "=B" + (j + 1).ToString();
            }
        }
    }

}


