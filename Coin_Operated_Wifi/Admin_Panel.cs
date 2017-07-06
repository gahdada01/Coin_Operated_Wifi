using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Coin_Operated_Wifi
{
    public partial class frmAdminPanel : Form
    {
        private String connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=dbVoucher.mdb";
        private OleDbConnection voucherDatabase;
        private string idnumber;

        public frmAdminPanel()
        {
            InitializeComponent();
            getDataTable();               //Display Database userregister on form load
            btnReprint.Enabled = false;   // btnReprint disabled
            btnDelete.Enabled = false;    // btnDelete disabled
        }//end frmAdminPanel()

        //function to display contents of userregister table from database to Datagrid
        private void getDataTable()
        {
            Users_Datagrid.DataSource = null;  //
            Users_Datagrid.Rows.Clear();       //initialize the Datagrid
            Users_Datagrid.Refresh();          //

            string query = "SELECT * FROM userregister";                     //command for database stored to string
            using (voucherDatabase = new OleDbConnection(connectionString))  //use voucherDatabse
            {
                using (OleDbDataAdapter adapter = new OleDbDataAdapter(query, voucherDatabase))  //use adapter
                {
          
                        DataTable datatable = new DataTable();              //set new datatable
                        adapter.Fill(datatable);
                        for (int i = 0; i < datatable.Rows.Count; i++)      //loop for display of contents from database to datagrid table
                        {
                            //syntax for filling cells with information from database
                            Users_Datagrid.Rows.Add(datatable.Rows[i][0],datatable.Rows[i][1],datatable.Rows[i][2],datatable.Rows[i][3],datatable.Rows[i][4],datatable.Rows[i][5]);  
                        }
                }
            }
            
        }//end getDataTable

        //logout from admin page
        private void btnLogout_Click(object sender, EventArgs e)
        {
            this.Hide();                           //hide the current form
            frmRegister reg = new frmRegister();   //set new frmRegister called reg
            reg.ctozero();                         //call function ctozero to initialize c from frmRegister
            reg.Show();                            //show reg
        }//end btnLogout

        //function for printing
        private void printreceipt()
        {
            PrintDialog printdialog = new PrintDialog();         //set new PrintDialog called printdialog
            PrintDocument printdocument = new PrintDocument();   //set new PrintDocument called printdocument
            printdialog.Document = printdocument;                //set printdocument to be the document inside printdialog
            //call printDocument_PrintPage function so that it will be printed
            printdocument.PrintPage += new PrintPageEventHandler(printDocument_PrintPage);  

            DialogResult result = printdialog.ShowDialog();      //store dialogresult if ok is pressed

            if (result == DialogResult.OK)                       //check if OK button is pressed in dialog box
            {
                printdocument.Print();                           //prints document to printer
            }

        }//end printreceipt()

        //function for the format of the print page
        void printDocument_PrintPage(Object sender, PrintPageEventArgs e)
        {
            Graphics graphic = e.Graphics;   

            Font font = new Font("Courire New", 12);

            float fontHeight = font.GetHeight();

            int startX = 10;
            int startY = 10;
            int offset = 10;

            graphic.DrawString("Welcome to The Coin", new Font("Courier New", 10), new SolidBrush(Color.Black), startX, startY);                            //
            graphic.DrawString("   Operated Wifi", new Font("Courier New", 10), new SolidBrush(Color.Black), startX, startY + offset);                      //
            graphic.DrawString("Name   : " + txtName.Text, new Font("Courier New", 8), new SolidBrush(Color.Black), startX, startY + offset + 32);          //
            graphic.DrawString("Availed: " + txtAvailed.Text, new Font("Courier New", 8), new SolidBrush(Color.Black), startX, startY + offset + 44);       //  this part are
            graphic.DrawString("Payment: " + txtPayment.Text, new Font("Courier New", 8), new SolidBrush(Color.Black), startX, startY + offset + 56);       //  the ones to be printed
            graphic.DrawString("Time   : " + txtConsume.Text, new Font("Courier New", 8), new SolidBrush(Color.Black), startX, startY + offset + 68);       //  in the receipt
            graphic.DrawString("Code   : " + txtVoucher.Text, new Font("Courier New", 8), new SolidBrush(Color.Black), startX, startY + offset + 80);       //
            graphic.DrawString("Thank You!Come Again.",new Font("Courier New", 10), new SolidBrush(Color.Black), startX, startY + offset +100);             //


        }//end printDocument_PrintPage()

        //function to clear all textboxes
        private void clrTxtBox()
        {
            txtName.Text = "";
            txtAvailed.Text = "";
            txtConsume.Text = "";
            txtPayment.Text = "";
            txtVoucher.Text = "";
            btnReprint.Enabled = false;  //disables btnReprint
            btnDelete.Enabled = false;   //disables btnDelete
        }//end clrTextBox

        //print the chosen user
        private void btnReprint_Click(object sender, EventArgs e)
        {
            printreceipt();   //calls function printreceipt
            clrTxtBox();      //calls function clrTxtBox
        }//end btnReprint

        //if cell is double clicked shows contents to the textboxes
        private void Users_Datagrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            btnReprint.Enabled = true;   //enables btnReprint
            btnDelete.Enabled = true;    //enables btnDelete
            foreach (DataGridViewRow row in Users_Datagrid.SelectedRows)  //gets every data from the row selected
            {
                idnumber = row.Cells[0].Value.ToString();           // 
                txtName.Text = row.Cells[1].Value.ToString();       //  stores/display content
                txtPayment.Text = row.Cells[3].Value.ToString();    //  to its respective fields
                txtConsume.Text = row.Cells[4].Value.ToString();    //
                txtAvailed.Text = row.Cells[2].Value.ToString();    //
                txtVoucher.Text = row.Cells[5].Value.ToString();    //
            }
        }//end Users_Datagrid_CellDoubleClick

        //deletes selected record
        private void btnDelete_Click(object sender, EventArgs e)
        {
            voucherDatabase = new OleDbConnection(connectionString);              //create new oledbconnection called voucherDatabase with parameters connectionString
            OleDbDataAdapter oledbAdapter = new OleDbDataAdapter();               //create new oledbdataadapter called oldbadapter
            string queryStr = "DELETE FROM userregister WHERE id = " + idnumber;  //command in database stored in queryStr
            voucherDatabase.Open();                                               //Opens Database connection
            oledbAdapter.DeleteCommand = voucherDatabase.CreateCommand();         //creates a new command for database 
            oledbAdapter.DeleteCommand.CommandText = queryStr;                    // sends queryStr to be executed in Database
            int rows = oledbAdapter.DeleteCommand.ExecuteNonQuery();              //stores executenonquery to row if it was executed or not
            if (rows > 0)                                                         //if statement if the command was executed for verification
            {
                //prompts a messagebox that delete of record was successfull
                MessageBox.Show("User Record Successfully Deleted from Database!", "User Delete Record", MessageBoxButtons.OK, MessageBoxIcon.Information);
                voucherDatabase.Close();                                          //closes database connection
                getDataTable();                                                   //calls function getDataTable
                clrTxtBox();                                                      //calls function clrTxtBox
            }
            else
                voucherDatabase.Close();                                          //closes database connection
        }//end btnDelete

    }//end of class
}
