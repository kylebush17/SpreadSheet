using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpreadSheet;

//Programmer: Kyle Bush
//Project: spreadsheet application
//Date last modified: 3/8/2017
//changes made: added dependency functionality
//collaborators: Evan Olds
namespace SpreadSheet_KyleBush
{
    public partial class Form1 : Form
    {
        private SS ss = new SS(50, 26);   //spreadsheet declaration.
        public Form1()
        {
            InitializeComponent();


        }


        void CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            int row = e.RowIndex, col = e.ColumnIndex;
            Cell c = ss.GetCell(row, col);
            dataGridView1.Rows[row].Cells[col].Value = c.text;
        }

        void CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex, col = e.ColumnIndex;
            string txt;

            Cell c = ss.GetCell(row, col);

            try
            {
                txt = dataGridView1.Rows[row].Cells[col].Value.ToString();
            }
            catch (NullReferenceException)
            {
                txt = "";
            }

            c.text = txt;

        }
        //what happens when CellPropertyChanged is triggered. this is what
        //is directly changing the visible UI
        private void OnCellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Cell current_cell = sender as Cell;
            if (e.PropertyName == "value" && current_cell != null)   //make sure cell isn't null to avoid an exception
            {
                //physically change the UI
                dataGridView1.Rows[current_cell.row_index].Cells[current_cell.col_index].Value = current_cell.value;
            }

        }

        //button to test the RandomSpreadSheet method defined in SpreadSheet.cs
        private void button1_Click(object sender, EventArgs e)
        {
            ss.RandomSpreadSheet();


        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            ss.CellPropertyChanged += OnCellPropertyChanged;    //subscribe to cellpropertychanged
            dataGridView1.CellBeginEdit += CellBeginEdit;
            dataGridView1.CellEndEdit += CellEndEdit;
            //initialize the visible UI
            this.dataGridView1.Columns.Clear();
            string[] ABC = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            for (int i = 0; i < 50; i++)
            {
                if (i < 26)
                    this.dataGridView1.Columns.Add(ABC.ElementAt(i).ToString(), ABC.ElementAt(i).ToString());

                this.dataGridView1.Rows.Add();
                this.dataGridView1.Rows[i].HeaderCell.Value = (i + 1).ToString();

            }
            //end UI initialization
        }
    }
}

