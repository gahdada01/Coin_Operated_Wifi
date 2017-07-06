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
    public partial class frmAdmin : Form
    {
        private OleDbConnection voucherDatabase;
        private String connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=dbVoucher.mdb";

        public frmAdmin()
        {
            InitializeComponent();
        }//end frmAdmin
        
        //function to checkUser if it exist
        private void checkUser()
        {
            voucherDatabase = new OleDbConnection(connectionString);          //creates new voucherDatabase connection
            voucherDatabase.Open();                                           //opens database connection
            //command in database stored on queryStr
            String queryStr = "SELECT count(*) FROM useraccount WHERE username='" + txtUser.Text + "' AND userpass ='" + txtPass.Text + "'";
            OleDbCommand oleDbCmd = new OleDbCommand(queryStr, voucherDatabase);  //creates new oledbCommand with parameters queryStr and voucherDatabase

            int count = (int)oleDbCmd.ExecuteScalar();                         //stores executescalar if a username and password exist as 1 or 0
            if (count > 0)                                                     //if statement if a match was found
            {
                this.Hide();                                                   //hides this form
                voucherDatabase.Close();                                       //closes database connection
                frmAdminPanel adPa = new frmAdminPanel();                      //creates new frmAdminPanel as adPa
                adPa.Show();                                                   //shows form adPa
            }
            else
            {
                MessageBox.Show("Invalid Account");                            //prompts a messagebox that indicates no username and password exists
                txtUser.Text = "";                                             //clears txtUser textbox
                txtPass.Text = "";                                             //clears txtPass textBox
                voucherDatabase.Close();                                       //closes database connection
            }
        }//end checkUser

        //function if button btnLog is clicked
        private void btnLog_Click(object sender, EventArgs e)
        {
            checkUser();            //calls function checkUser
        }//end btnLog

        //function if button btnExit is clicked
        private void btnExit_Click_1(object sender, EventArgs e)
        {
            this.Hide();                                //hides the current form
            frmRegister frmReg = new frmRegister();     //creates a new frmRegister called frmReg
            frmReg.ctozero();                           //calls function ctozero from frmReg to initialize c
            frmReg.Show();                              //shows frmReg
        }//end btnExit

        //function if button button1 or close Program is clicked
        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();    //terminates all forms and exits application
        }//end close program button
    }
}
