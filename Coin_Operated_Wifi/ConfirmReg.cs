using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Drawing.Printing;

namespace Coin_Operated_Wifi
{
    public partial class frmConfirm : Form
    {
        private OleDbConnection voucherDatabase;
        private String connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=dbVoucher.mdb";
        private string timetouse;
        private int rand;
        private string tblname = "";

        public frmConfirm(string name,string availed, string payment, string code,int random)     
        {
            InitializeComponent();
            txtName.Text = name;                        //initializes txtName as name
            txtAvailed.Text = availed;                  //initializes txtAvailed as availed
            txtPayment.Text = payment + " pesos";       //initializes txtPayment as payment
            txtVouch.Text = code;                       //initializes txtVouch as code
            timetouse = time2use(availed);              //initializes timetouse as function time2use with parameters availed
            rand = random;                              //sets int rand as random for id reference
        }//end frmConfirm
        
        //function for setting time to use base from availed offer
        private string time2use(string availed)
        {
            string a;                                           //new string called a
            //if statement for initialization of a and for tablename
            if (availed == "1 Day")                 
            { a = "24 hours";  this.tblname = "daily"; }
            else if (availed == "1 Week")
            { a = "168 hours"; this.tblname = "weekly"; }
            else
            { a = "720 hours"; this.tblname = "monthly"; }

            return a;                                           //returns string a
        }//end time2use

        //function if button btnPrint is clicked
        private void btnPrint_Click(object sender, EventArgs e)
        {
            //AddToDatabase();                           //calls function AddToDatabase
            //DeleteVoucher();                           //calls function DeleteVoucher
            //printreceipt();                            //calls function printreceipt
            frmRegister frmReg = new frmRegister();      //creates new frmRegister called frmReg
            frmReg.ctozero();                            //calls function ctozero from frmRegister to initialize c
            this.Hide();                                 //hides current form
            frmReg.Show();                               //show frmReg
        }//end btnPrint

        //funtion to Add record to database
        public void AddToDatabase()
        {
            voucherDatabase = new OleDbConnection(connectionString);                     //creates new voucherDatabase connection
            using ( OleDbConnection mysql = new OleDbConnection(connectionString))       //pwede raman guro neg wa 
            {
                voucherDatabase.Open();                                                  //opens database connection

                OleDbCommand cmd = new OleDbCommand();                                   //creates new oledbcommand called cmd

                cmd.CommandType = CommandType.Text;                                      //declares command type as text
                //database command stored in command text
                cmd.CommandText = "insert into userregister ([username],[availed_offer],[coin_inserted],[time_to_use],[voucherCode]) values (?,?,?,?,?)";
                cmd.Parameters.AddWithValue("@username",txtName.Text.ToString());               //
                cmd.Parameters.AddWithValue("@availed_offer", txtAvailed.Text.ToString());      //
                cmd.Parameters.AddWithValue("@coin_inserted", txtPayment.Text.ToString());      //adds value to database command
                cmd.Parameters.AddWithValue("@time_to_use", timetouse.ToString());              //
                cmd.Parameters.AddWithValue("@voucherCode", txtVouch.Text.ToString());          //
                cmd.Connection = voucherDatabase;                                         //declares connection to be voucherDatabase connection
                cmd.ExecuteNonQuery();                                                    
                voucherDatabase.Close();                                                  //closes database connection
            }

        }//end AddToDatabase

        //function to delete record from voucher
        public void DeleteVoucher()
        {
            voucherDatabase = new OleDbConnection(connectionString);                //creates new database connection
            Console.WriteLine(rand);                                                //writes rand to console
            OleDbDataAdapter oledbAdapter = new OleDbDataAdapter();                 //create new oledbAdapter called oledbadapter
            string queryStr = "DELETE FROM " + tblname + " WHERE id = " + rand;     //stores database command to string with tblname and rand as parameters
            voucherDatabase.Open();                                                 //opens database connection
            oledbAdapter.DeleteCommand = voucherDatabase.CreateCommand();           //creates a new command for database
            oledbAdapter.DeleteCommand.CommandText = queryStr;                      //sends queryStr to be executed in database
            oledbAdapter.DeleteCommand.ExecuteNonQuery();                           //pwede ra ne wala
            voucherDatabase.Close();                                                //closes database connection
        }//end deleteVoucher

        //function for printing
        private void printreceipt()
        {
            PrintDialog printdialog = new PrintDialog();                //set new PrintDialog called printdialog
            PrintDocument printdocument = new PrintDocument();          //set new PrintDocument called printdocument
            printdialog.Document = printdocument;                       //set printdocument to be the document inside printdialog
            //call printDocument_PrintPage function so that it will be printed
            printdocument.PrintPage += new PrintPageEventHandler(printDocument_PrintPage);

            DialogResult result = printdialog.ShowDialog();             //store dialogresult if ok is pressed

            if (result == DialogResult.OK)                              //check if OK button is pressed in dialog box
            {
                printdocument.Print();                                  //prints document to printer
            }
        }//end printreceipt

        //function for the format of the print page
        void printDocument_PrintPage(Object sender, PrintPageEventArgs e)
        {
            Graphics graphic = e.Graphics;

            Font font = new Font("Courire New", 12);

            float fontHeight = font.GetHeight();

            int startX = 10;
            int startY = 10;
            int offset = 10;

            graphic.DrawString("Welcome to The Coin", new Font("Courier New", 10), new SolidBrush(Color.Black), startX, startY);
            graphic.DrawString("   Operated Wifi", new Font("Courier New", 10), new SolidBrush(Color.Black), startX, startY + offset);
            graphic.DrawString("Name   : " + txtName.Text, new Font("Courier New", 8), new SolidBrush(Color.Black), startX, startY + offset + 32);
            graphic.DrawString("Availed: " + txtAvailed.Text, new Font("Courier New", 8), new SolidBrush(Color.Black), startX, startY + offset + 44);
            graphic.DrawString("Payment: " + txtPayment.Text, new Font("Courier New", 8), new SolidBrush(Color.Black), startX, startY + offset + 56);
            graphic.DrawString("Time   : " + timetouse, new Font("Courier New", 8), new SolidBrush(Color.Black), startX, startY + offset + 68);
            graphic.DrawString("Code   : " + txtVouch.Text, new Font("Courier New", 8), new SolidBrush(Color.Black), startX, startY + offset + 80);
            graphic.DrawString("Thank You!Come Again.",new Font("Courier New", 10), new SolidBrush(Color.Black), startX, startY + offset +100);


        }//end printDocument_PrintPage()
    }
}
