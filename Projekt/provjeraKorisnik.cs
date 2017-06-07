using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projekt
{
    public class provjeraKorisnik
    {
        public provjeraKorisnik()
        { }

        public void postaviPrijavljen(String username)
        {
            try
            {
              
                SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\projekt.mdf;Integrated Security=True");
                //SqlCommand cmd = new SqlCommand("Select * from Korisnici where username=@username and password=@password", con);
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "UPDATE Korisnici SET prijavljen = @jedan Where username = @user";
                cmd.Parameters.AddWithValue("@jedan", 1);
                cmd.Parameters.AddWithValue("@user", username);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public int provjeraStudent()
        {
            try
            {
                SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\projekt.mdf;Integrated Security=True");
                //SqlCommand cmd = new SqlCommand("Select * from Korisnici where username=@username and password=@password", con);
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "Select username from Korisnici where prijavljen = @jedan";
                cmd.Parameters.AddWithValue("@jedan", 1);
                con.Open();
                //cmd.ExecuteNonQuery();
                SqlDataAdapter adapt = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapt.Fill(dt);
                con.Close();

                if (dt.Rows.Count > 0)
                    foreach (DataRow dr in dt.Rows)
                        if (dr["username"].ToString().Equals("student"))
                            return 0;
                
            }              
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
            return 1;
        }

        public void postaviOdjavu( )
        {
            try
            {
                SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\projekt.mdf;Integrated Security=True");
                //SqlCommand cmd = new SqlCommand("Select * from Korisnici where username=@username and password=@password", con);
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = "UPDATE Korisnici SET prijavljen = @nula Where prijavljen = @jedan";
                cmd.Parameters.AddWithValue("@nula", 0);
                cmd.Parameters.AddWithValue("@jedan", 1);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


    }
}
