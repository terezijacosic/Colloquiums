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
    public class RezervirajView : Form1
    {
        Form form1 = Application.OpenForms["Form1"];
        Panel rezervacije;
        Label dvorane_label, ime_dvorane_label, termin_label, broj_studenata_label, lista_kolegija_label;
        DateTimePicker datetimepicker2;
        Button rezerviraj, pokazi;
        TextBox ime_dvorane, termin;
        ComboBox lista_kolegija;
        NumericUpDown broj_studenata;
        DataGridView dataGridView = new DataGridView()
        {
            //Location = new Point(210, 150),
            BackgroundColor = Color.White,
            MaximumSize = new Size(500, 500),
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        };

        public RezervirajView()
        {
            rezervacije = new Panel()
            {
                Location = new Point(0, 20),
                BackColor = Color.Transparent,
                Size = new Size(ClientRectangle.Width, ClientRectangle.Height)
            };

            dvorane_label = new Label()
            {
                Location = new Point(80, 100),
                MaximumSize = new Size(100, 0),
                AutoSize = true,
                BackColor = Color.Transparent,
                Text = "Izaberite datum rezervacije, dvoranu, termin, kolegij" +
                 "koji će se pisati u njoj i broj studenata koji pišu kolokvij iz tog kolegija."
            };
            rezervacije.Controls.Add(dvorane_label);
            //tooltip.SetToolTip( labela_oblak, "Izaberite datum za koji želite vidjeti zauzeća dvorana.");
            datetimepicker2 = new DateTimePicker();
            datetimepicker2.Location = new Point(ClientRectangle.Width / 2 - datetimepicker2.Width / 2, 100);
            rezervacije.Controls.Add(datetimepicker2);


            rezerviraj = new Button() { Text = "Rezerviraj" };
            rezerviraj.Location = new Point(ClientRectangle.Width / 2 - rezerviraj.Width / 2, 420);
            rezerviraj.Click += new EventHandler(this.rezerviraj_dvoranu);
            rezervacije.Controls.Add(rezerviraj);

            /* pokazi = new Button() { Text = "Pokazi rezervacije", Location = new Point(110, 400) };
             pokazi.Click += new EventHandler(this.pokazi_dvorane);
             rezervacije.Controls.Add(pokazi); */

            ime_dvorane_label = new Label() { Text = "Ime dvorane:" };
            ime_dvorane_label.Location = new Point(404, 300);
            ime_dvorane = new TextBox() { Location = new Point(504, 300) };
            rezervacije.Controls.Add(ime_dvorane_label); rezervacije.Controls.Add(ime_dvorane);

            //    Label datum = new Label() { Location = new Point(10, 380), Text = "Datum:" };            
            //   active_panel.Controls.Add(datum_rezervacije); active_panel.Controls.Add(datum);

            termin_label = new Label() { Location = new Point(404, 330), Text = "Termin:" };
            termin = new TextBox() { Location = new Point(504, 330) };
            rezervacije.Controls.Add(termin_label); rezervacije.Controls.Add(termin);

            lista_kolegija_label = new Label() { Location = new Point(404, 360), Text = "Izaberite kolegij:" };
            lista_kolegija = new ComboBox() { Location = new Point(504, 360), Items = { "Android", "RP3", "Interpretacija programa" }, SelectedIndex = 0 };
            rezervacije.Controls.Add(lista_kolegija); rezervacije.Controls.Add(lista_kolegija_label);

            broj_studenata_label = new Label() { Location = new Point(404, 390), Text = "Izaberite broj studenata:" };
            broj_studenata = new NumericUpDown() { Location = new Point(504, 390) };
            rezervacije.Controls.Add(broj_studenata); rezervacije.Controls.Add(broj_studenata_label);

            form1.Controls.Add(rezervacije);
        }

        private void rezerviraj_dvoranu(object sender, EventArgs e)
        {
            try
            {
                //ne treba zbilja, treba samo minumum numerica pomaknuti na jedan!!!
                if (broj_studenata.Value == 0)
                    return;

                SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\projekt.mdf;Integrated Security=True");
                con.Open();

                //provjera koliko je mjesta vec rezervirano u dvorani
                string upit = "SELECT Zauzeto_mjesta FROM Rezervacije WHERE Dvorana=@dvorana AND Termin=@termin AND Datum=@datum AND Kolegij=@kolegij";
                SqlCommand cmd = new SqlCommand(upit, con);
                cmd.Parameters.AddWithValue("@dvorana", ime_dvorane.Text);
                cmd.Parameters.AddWithValue("@termin", termin.Text);
                cmd.Parameters.AddWithValue("@datum", datetimepicker2.Value.ToShortDateString());
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

                if (broj_studenata.Value + zauzeto > Convert.ToInt32(dr["Mjesta"]) / 2)
                {
                    MessageBox.Show("Nema dovoljno mjesta!"); //pogledati još 1/2 i jeli rezervirano
                    return;
                }

                dr.Close();

                //ako je sve ok, rezerviraj i ubaci u tablicu
                string insert = "Insert into Rezervacije (Dvorana, Datum, Termin, Kolegij, Zauzeto_mjesta) values ('" + ime_dvorane.Text + "','" + datetimepicker2.Value.ToShortDateString() + "','"
                                                        + termin.Text + "','" + lista_kolegija.Text + "','" + broj_studenata.Value + "')";
                DataTable dt = new DataTable();
                SqlDataAdapter sda = new SqlDataAdapter();
                cmd = new SqlCommand(insert, con);
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

                //SqlDataAdapter sda = new SqlDataAdapter("Select * from Dvorane" , con);
                SqlDataAdapter sda = new SqlDataAdapter("Select * from Rezervacije where Datum='"
                                                              + datetimepicker2.Value.ToShortDateString() + "'", con);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                BindingSource bSource = new BindingSource();

                bSource.DataSource = dt;
                rezervacije.Controls.Add(dataGridView);

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
                dataGridView.Location = new Point(210, 150);

                sda.Update(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}