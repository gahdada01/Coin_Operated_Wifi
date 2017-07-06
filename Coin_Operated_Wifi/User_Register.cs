using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO.Ports;
using System.Data.OleDb;

namespace Coin_Operated_Wifi
{
    public partial class frmRegister : Form
    {
        
        public Boolean check = false;
        public delegate void UpdateTextCallback(string message);
        public int InpAddress = 889;
        public int valueOfPin = 0;
        public static int c = 0;
        public string availed = "";
        private string tblname = "";
        private String connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=dbVoucher.mdb";
        private Thread thread;
        private OleDbConnection voucherDatabase;

        public frmRegister()
        {
            InitializeComponent();
            this.clearFields();                 //calls function clearFields for initialization
            //StartThread();                    //calls function StartThread
        }//end frmRegister

        //function to Start the Thread
        private void StartThread()
        {
            thread = new Thread(new ThreadStart(TestThread));     //creates new Thread with name thread and parameter with new Threadstart calling function TestThread
            thread.Start();                                       //starts thread  
        }//end StartThread

        //function TestThread
        public void TestThread()
        {
            while (true)                                        //continuous loop
            {
                Thread.Sleep(100);                              //calls Sleep for 100 ms
                valueOfPin = PortAccess.Input(InpAddress);     //gets port value
                if (valueOfPin == 127)                          //if statement when pulse is detected
                {
                    c = c + 1;                                 //increments c for coin inserted or pulse detected
                    //txtCredits is updating everytime c is incremented and updateTextCallback with function UpdateText
                    txtCredits.Invoke(new UpdateTextCallback(this.UpdateText), new object[] { c.ToString() });
                }
            }
        }//end TestThread

        //function UpdateText with parameter message
        private void UpdateText(string message)
        {
            txtCredits.Text = message;     //txtCredits' value is message
        }//end updateText

        //function to clear all textFields
        public void clearFields()
        {

            radDay.Checked = false;
            radWeek.Checked = false;
            radMonth.Checked = false;
            txtName.Text = "";
            txtCost.Text = "";
            txtConsume.Text = "";
            txtCredits.Text = c.ToString();
        }//end clearFields

        //function if button button2 or clear button is clicked
        private void button2_Click(object sender, EventArgs e)
        {
            clearFields();      //calls function clearFields
        }//end button2

        //function if radio button radDay is clicked
        private void radDay_CheckedChanged(object sender, EventArgs e)
        {
            txtCost.Text = "30";                //
            txtConsume.Text = "24";             //initializes textboxes
            this.availed = "1 Day";             //and datas
            this.tblname = "daily";             //
        }//end radDay

        //function if radio button radWeek is clicked
        private void radWeek_CheckedChanged(object sender, EventArgs e)
        {
            txtCost.Text = "100";               // 
            txtConsume.Text = "168";            //initializes textboxes
            this.availed = "1 Week";            //and datas
            this.tblname = "weekly";            //
        }//end radWeek

        //function if radio button radMonth is clicked
        private void radMonth_CheckedChanged(object sender, EventArgs e)
        {
            txtCost.Text = "250";               //
            txtConsume.Text = "720";            //initializes textboxes
            this.availed = "1 Month";           //and datas
            this.tblname = "monthly";           //
        }//end radMonth

        //function if toolStripMenuItem login as Administrator is clicked
        private void loginAsAdministratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //thread.Abort();                   //aborts thread
            frmAdmin adlog = new frmAdmin();    //creates new frmAdmin called adlog
            adlog.Show();                       //show adlog
            this.Hide();                        //hides current form
        }//end loginAsAdministratorToolStripMenuItem

        //function that initializes c to zero
        public void ctozero()
        {
            c = 0;                  //initialize c to zero
            clearFields();          //calls function clearFields
        }

        //function if button btnAdd is clicked
        private void btnAdd_Click(object sender, EventArgs e)
        {
            check = OpenAccept();        //boolean check is given value by function openaccept
            string voucher = "";         //new string called voucher
            int randomnum = 0;           //new int called randomnum
            //thread.Abort();            //aborts thread
            c = Convert.ToInt32(txtCredits.Text);       //wa ni labot
            Random random = new Random();           //creates new random called random

            //if statement if all textboxes are filled and if a payment was made
            if (check == true && Convert.ToInt32(txtCredits.Text) >= Convert.ToInt32(txtCost.Text))
            {   
                //for loop intialized with i as 0, i++ and i<=200
                for (int i = 0; i <= 2000; i++)
                {
                    randomnum = random.Next(1, 2000);               //generates a random number from 1 to 2000
                    if (isAvailable(randomnum) == true)             //if statement if function isAvailable is true
                    {
                        voucher = Voucher(randomnum);               //value of voucher is from function voucher with parameter randomnum
                        this.Hide();                                //hides current form
                        //creates new frmConfim called confirm with parameters textname,availed,txtcredits,voucher and randomnum
                        frmConfirm confirm = new frmConfirm(txtName.Text, availed, txtCredits.Text, voucher,randomnum);
                        confirm.Show();                             //shows confirm
                        break;                                      //ends for loop
                    }
                    else
                    {
                        Console.WriteLine("no id found");           //writes to console if no id is found
                    }
                }// end for loop
            }
            //else if statement if payment is lacking
            else if (check == true && Convert.ToInt32(txtCredits.Text) < Convert.ToInt32(txtCost.Text)) 
            {
                //prompts a messagebox that indicates to add more coin according to cost
                MessageBox.Show("Please Insert Exact Coin Value According to Cost","Insert Coin",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
            //else statement if some fields are missing
            else
            {
                //prompts a messagebox that some text are missing
                MessageBox.Show("Some field/s missing!", "Register Form", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }//end btnAdd

        //function if randomNumber generated is available in database
        public Boolean isAvailable(int randomNUM) {
            Boolean bol = false;                                         //create new boolean called bool initialized as false
              
            voucherDatabase = new OleDbConnection(connectionString);     //creates new voucherDatabase connection
            voucherDatabase.Open();                                      //Opens database connection
            //database command stored in queryStr
            String queryStr = "SELECT count(*) FROM " + tblname + " WHERE ID =" + randomNUM + "";
            OleDbCommand oleDbCmd = new OleDbCommand(queryStr, voucherDatabase);        //new Oledbcommand called oldbcmd with parameters queryStr and voucherDatabase

            int count = (int)oleDbCmd.ExecuteScalar();                  //checks executescalar if it is existing and stores to count as 1 and 0
            if (count > 0)                                              //if statement if count is greater than 0
            {   
                bol = true;                                             //bol is equal to true
            }
            voucherDatabase.Close();                                    //closes database connection
                
            return bol;                                                 //return value of bol
        }//end isAvailable

        //function to get voucher code from database
        public string Voucher(int randomNUM)
        {
            voucherDatabase = new OleDbConnection(connectionString);            //create new voucherDatabase connection         
            voucherDatabase.Open();                                             //opens database connection
            //database command stored in queryStr with initialization from tblname and randomnum
            String queryStr = "SELECT " + tblname + " FROM " + tblname + " WHERE ID =" + randomNUM + "";
            //new oleDbCommand called oleDbCmd with parameters queryStr and voucherDatabase
            OleDbCommand oleDbCmd = new OleDbCommand(queryStr, voucherDatabase);
            //oleDbCmd = new OleDbCommand(queryStr, voucherDatabase);
            OleDbDataReader oldDbReader = oleDbCmd.ExecuteReader();             //create new OleDbDataReader called oldDbreader with OleDbCmd.ExecuterReader
            oldDbReader.Read();                                                 //reads the command given by oledbcmdexecutereader
            String vouchercode = oldDbReader.GetValue(0).ToString();            //stores the value to string voucher code
            voucherDatabase.Close();                                            //closes database connection

            return vouchercode;                                                 //returns vouchercode
        }//end voucher

        //function if All txtFields are filled and returns value check
        public Boolean OpenAccept()
        {
            //if statement for every textboxes
            if (txtConsume.Text == "")
                check = false;
            else if (txtCost.Text == "")
                check = false;
            else if (txtCredits.Text == "")
                check = false;
            else if (txtName.Text == "")
                check = false;
            else
                check = true;

            return check;
        }

      

    }

    class PortAccess         //new class called portAccess
    {

        [DllImport("inpout32.dll", EntryPoint = "Inp32")]           //imports our library called inpout32.dll
        public static extern int Input(int adress);                 //initializes input as the address of our data in parallel port
    }//end class
}
