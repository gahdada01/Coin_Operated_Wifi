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

namespace Coin_Operated_Wifi
{
    public partial class frmAdLogin : Form
    {
        private OleDbConnection voucherDatabase;
        String connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=dbVoucher.mdb";

        public frmAdLogin()
        {
            InitializeComponent();
            connectToVoucherDb();           //calls function connectToVoucherDb
        }//end frmAdLogin

        //function to connect to Database
        private void connectToVoucherDb()
        {
            voucherDatabase = new OleDbConnection(connectionString);       //creates new database connection
            voucherDatabase.Open();                                        //opens database connection
            //prompts a messagebox that connection was successful
            MessageBox.Show("Connection Successful", "Authentication", MessageBoxButtons.OK, MessageBoxIcon.Information);
            voucherDatabase.Close();                                        //closes database connection
        }//end connectToVoucherDb

        //function to checkUser if it exist
        private void checkUser()
        {
            voucherDatabase.Open();                                                  //opens database connection
            //command in database stored in querystr
            String queryStr = "SELECT count(*) FROM useraccount WHERE username='" + txtUser.Text + "' AND userpass ='" + txtPass.Text + "'";
            OleDbCommand oleDbCmd = new OleDbCommand(queryStr, voucherDatabase);     //creates new oledbCommand with parameters queryStr and voucherDatabase

            int count = (int)oleDbCmd.ExecuteScalar();                               //stores executescalar if a username and password exist as 1 or 0
            if (count > 0)                                                           //if statement if a match was found
            {                    
                voucherDatabase.Close();                                             //closes database connection
                frmRegister formReg = new frmRegister();                             //creates new frmRegister called frmReg
                formReg.Show();                                                      //show frmReg 
                this.Hide();                                                         //hides the current form
            }
            else
            {
                MessageBox.Show("Invalid Account");                                 //prompts a messagebox that indicates no username and password exists
                txtUser.Text = "";                                                  //clears txtUser textbox
                txtPass.Text = "";                                                  //clears txtPass textBox
                voucherDatabase.Close();                                            //closes database connection
            }
        }//end checkUser

        //function if button btnLogin is clicked
        private void btnLogin_Click(object sender, EventArgs e)
        {
            checkUser();                //calls function checkUser
        }//end btnLogin

        //function if button btnExit is clicked
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();               //closes current form
        }

      
    }
}
