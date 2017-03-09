using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;


namespace SpreadSheet
{
    /* Cell is a class that is implemented to represent a singular cell
     * in the spreadsheet. It inherits from the INotifyPropertyChanged class
     * in order to update the UI when the contents of a cell are changed by the
     * user. */
    public abstract class Cell : INotifyPropertyChanged
    {
        //members
        private int rowIndex;
        private int colIndex;
        protected string Text;
        protected internal string Value;
        private readonly string Name;
        //Cell Property Changed Event
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        //constructor sets row and col index of the cell
        public Cell(int row_index, int col_index)
        {
            rowIndex = row_index;
            colIndex = col_index;
            Name += Convert.ToChar('A' + col_index);
            Name += (row_index + 1).ToString();
        }

        //getters return row and col index of cell
        public int row_index
        {
            get { return rowIndex; }
        }

        public string name
        {
            get { return Name; }
        }

        //return column index of current cell
        public int col_index
        {
            get { return colIndex; }
        }

        //can set and return the text property of the current cell
        public string text
        {
            get { return Text; }

            set
            {
                if (value != Text)  //if you are just reassigning the text do not do anything
                {
                    Text = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("text"));    //trigger property

                }
            }
        }

        //return value of current cell. setter to be defined later
        public string value
        {
            get { return Value; }
            //set
            //{
            //    Value = value;
            //    PropertyChanged(this, new PropertyChangedEventArgs("value"));
            //}
        }



    }
}
