using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projekt
{
    public class DvoraneView : Form1
    {
        Form form1 = Application.OpenForms["Form1"];
        Panel dvorane;
        Label dvorane_label, ime_dvorane_label, termin_label, broj_studenata_label, lista_kolegija_label;
        DateTimePicker datetimepicker;
        Button rezerviraj, pokazi;
        TextBox ime_dvorane, termin;
        ComboBox lista_kolegija;
        NumericUpDown broj_studenata;
        new DataGridView dataGridView = new DataGridView()
        {
//            Location = new Point(210, 150),
            BackgroundColor = Color.White,
            //MaximumSize = new Size(500, 500),
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        };

        public DvoraneView()
        {
           
            dvorane = new Panel() { Location = new Point(0, 20), BackColor = Color.Transparent, Size = new Size(ClientRectangle.Width, ClientRectangle.Height) };
           
            dvorane_label = new Label()
            {
                Location = new Point(80, 100),
                MaximumSize = new Size(100, 0),
                AutoSize = true,
                BackColor = Color.Transparent,
                Text = "Izaberite datum za koji želite vidjeti zauzeća dvorana."
            };
            dvorane.Controls.Add(dvorane_label);
            //tooltip.SetToolTip( labela_oblak, "Izaberite datum za koji želite vidjeti zauzeća dvorana.");

            datetimepicker = new DateTimePicker();
            datetimepicker.Location = new Point(ClientRectangle.Width/2 - datetimepicker.Width/2, 100);
            dvorane.Controls.Add(datetimepicker);           

            pokazi = new Button() { Text = "Pokazi rezervacije" };
            pokazi.Location = new Point(ClientRectangle.Width/2 - pokazi.Width/2, 150);
            pokazi.Click += new EventHandler(this.pokazi_dvorane);
            dvorane.Controls.Add(pokazi);

            form1.Controls.Add(dvorane);
        }
        

        private void rezerviraj_dvoranu(object sender, EventArgs e)
        {
            try
            {
                SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\projekt.mdf;Integrated Security=True");
                con.Open();

                //provjera koliko je mjesta vec rezervirano u dvorani
                string upit = "SELECT Zauzeto_mjesta FROM Rezervacije WHERE Dvorana=@dvorana AND Termin=@termin AND Datum=@datum AND Kolegij=@kolegij";
                SqlCommand cmd = new SqlCommand(upit, con);
                cmd.Parameters.AddWithValue("@dvorana", ime_dvorane.Text);
                cmd.Parameters.AddWithValue("@termin", termin.Text);
                cmd.Parameters.AddWithValue("@datum", datetimepicker.Value.ToShortDateString());
                cmd.Parameters.AddWithValue("@kolegij", lista_kolegija.Text);
                SqlDataReader dr = cmd.ExecuteReader();
                int zauzeto = 0;
                while (dr.Read())
                {
                    zauzeto += Convert.ToInt32(dr["Zauzeto_mjesta"]);
                }
                dr.Close();

                //provjera koliko ima mjesta u dvorani i ima li mjesta i za ovu rezervaciju
                upit = "SELECT Mjesta FROM Dvorane WHERE Ime=" + ime_dvorane.Text;
                cmd = new SqlCommand(upit, con);
                dr = cmd.ExecuteReader();

                dr.Read();

                if (broj_studenata.Value+zauzeto > Convert.ToInt32(dr["Mjesta"])/2)
                {
                    MessageBox.Show("Nema dovoljno mjesta!");
                    return;
                }

                dr.Close();

                //ako je sve ok, rezerviraj i ubaci u tablicu
                string insert = "Insert into Rezervacije (Dvorana, Datum, Termin, Kolegij, Zauzeto_mjesta) values (@dvorana, @datum, @termin, @kolegij, @zauzeto)";
                DataTable dt = new DataTable();
                SqlDataAdapter sda = new SqlDataAdapter();
                cmd = new SqlCommand(insert, con);
                cmd.Parameters.AddWithValue("@dvorana", ime_dvorane.Text);
                cmd.Parameters.AddWithValue("@datum", datetimepicker.Value.ToShortDateString());
                cmd.Parameters.AddWithValue("@termin", termin.Text);
                cmd.Parameters.AddWithValue("@kolegij", lista_kolegija.Text);
                cmd.Parameters.AddWithValue("@zauzeto", broj_studenata.Value);
                cmd.ExecuteNonQuery();
                
                con.Close();                
                MessageBox.Show("Uspješno ste rezervirali termin. Za provjeru opet pritisnite Pokaži.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        // pokazuje tablicu s dvoranama
        public void pokazi_dvorane(object sender, EventArgs e)
        {
            // MessageBox.Show(" " + dateTimePicker.Value.ToShortDateString());
            try
            {
                SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\projekt.mdf;Integrated Security=True");
                /*   SqlDataAdapter sda = new SqlDataAdapter("Select Naziv,Mjesta,Datum,Termin from Rezervacije where Datum='" 
                                                             + datetimepicker.Value.ToShortDateString() + "'", con); */

                SqlDataAdapter sda = new SqlDataAdapter("Select * from Rezervacije where Datum='"
                                                              + datetimepicker.Value.ToShortDateString() + "'", con);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                BindingSource bSource = new BindingSource();

                bSource.DataSource = dt;
                dvorane.Controls.Add(dataGridView);

                dataGridView.DataSource = bSource;
                int height = 0;
                foreach (DataGridViewRow row in dataGridView.Rows)
                    height += row.Height;
                height += dataGridView.ColumnHeadersHeight;

                int width = 0;
                foreach (DataGridViewColumn col in dataGridView.Columns)
                    width += col.Width;
                width += dataGridView.RowHeadersWidth;
                dataGridView.ClientSize = new Size(width + 2, height + 2);
                dataGridView.Location = new Point(ClientRectangle.Width/2 - dataGridView.Width/2, pokazi.Bottom + 50);

                sda.Update(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}
