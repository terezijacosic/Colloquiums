using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projekt
{
    public partial class LogIn : Form
    {
        public LogIn()
        {
            InitializeComponent();
            label1.Location = new Point(ClientRectangle.Width / 2 - label1.Width / 2, 100);
            label1.BackColor = Color.Transparent;
            txt_UserName.Location = new Point(ClientRectangle.Width / 2 - txt_UserName.Width / 2, 120);

            label2.Location = new Point(ClientRectangle.Width / 2 - label2.Width / 2, 160);
            label2.BackColor = Color.Transparent;
            txt_Password.Location = new Point(ClientRectangle.Width / 2 - txt_Password.Width / 2, 180);

            btn_login.Location = new Point(ClientRectangle.Width / 2 - btn_login.Width / 2, 230);
        }

        //funcija za prijavljivanje
        private void otvori()
        {
            if (txt_UserName.Text == "" || txt_Password.Text == "")
            {
                MessageBox.Show("Please provide UserName and Password");
                return;
            }
            try
            {
                //"Data Source = (LocalDB)\MSSQLLocalDB; AttachDbFilename = C:\Users\mbarisi\Documents\Visual Studio 2015\Projects\Projekt\Projekt\projekt.mdf; Integrated Security = True"

                //Create SqlConnection
                
                SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\projekt.mdf;Integrated Security=True");                
                SqlCommand cmd = new SqlCommand("Select username from Korisnici where username=@username and password=@password", con);
                cmd.Parameters.AddWithValue("@username", txt_UserName.Text);
                cmd.Parameters.AddWithValue("@password", txt_Password.Text);
                con.Open();
                SqlDataAdapter adapt = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapt.Fill(ds);
                con.Close();
                int count = ds.Tables[0].Rows.Count;
                //If count is equal to 1, than show frmMain form
                if (count == 1)
                {
                    
                    MessageBox.Show("Pozdrav, " + txt_UserName.Text + "!");
                    provjeraKorisnik provjera = new Projekt.provjeraKorisnik();
                    provjera.postaviPrijavljen(txt_UserName.Text);
                    this.Hide();
                    Form1 fm = new Form1();
                    fm.Show();
                }
                else
                {
                    MessageBox.Show("Neuspješna prijava!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //prijavljivanje s gumbom
        private void btn_login_Click(object sender, EventArgs e)
        {
            otvori();
        }

        //prijavljivanje s enterom
        private void txt_Password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                otvori();

            }
        }

        //prijavljivanje s enterom
        private void txt_UserName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                otvori();

            }
        }
    }
}
