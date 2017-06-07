using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;
using System.Drawing;

namespace Projekt
{
    // poanta ova klase je bila da služi za prikaz kolegija i za opciju Pregledaj i za Rasporedi
    // kako je zasad sve ružno kada se pokrene i daleko od funkcionalnosti, nigdje ju ne koristim.
    // ideja je da korisnik izabere kolegij(e) za koje želi napraviti raspored, i kao rezultat se nacrta raspored.
    // Međukoraci su dohvatiti dvoranu u kojoj se piše taj kolegij i dobiti parametre za crtanje (broj redaka, stupaca)

    public class PopisKolegija : Form1
    {
        public Form form1 = Application.OpenForms["Form1"];
        public Button generiraj_raspored = new Button() { Text = "Generiraj raspored" };
        SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\projekt.mdf;Integrated Security=True");
        DataTable dt = new DataTable();
        DataGridView dataGridView = new DataGridView();
        BindingSource bSource = new BindingSource();
        String value1 = "", value2 = "";
        List<Student> studenti;

        public PopisKolegija()
        {
            studenti = new List<Student>();
            dohvatiKolegije();
            
        }

        public void dohvatiKolegije()
        {           
            try
            {
                
                SqlDataAdapter sda = new SqlDataAdapter("Select distinct Kolegij, Datum from Studenti", con);                
                sda.Fill(dt);
                
                bSource.DataSource = dt;

                int nađeno = 0;
                foreach (Control item in this.Controls.OfType<Panel>())
                    if (item.Name == "active_panel")
                    {
                        nađeno = 1;
                        dataGridView.Location = new Point(210, 150);
                        //active_panel.Controls.Add(dataGridView);
                    }
                if (nađeno == 0)
                {
                    dataGridView.Location = new Point(400, 150);
                    form1.Controls.Add(dataGridView);
                }
                    
                dataGridView.DataSource = bSource;

                int height = 0;
                foreach (DataGridViewRow row in dataGridView.Rows)
                    height += row.Height;
                height += dataGridView.ColumnHeadersHeight;

                int width = 0;
                foreach (DataGridViewColumn col in dataGridView.Columns)
                    width += col.Width;
                width += dataGridView.RowHeadersWidth;

                //dataGridView.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                dataGridView.ClientSize = new Size(width + 2, height + 2);
                                 
                generiraj_raspored.Location = new Point(ClientRectangle.Width - 50, 100);
                form1.Controls.Add(generiraj_raspored);
                generiraj_raspored.Click += Generiraj_raspored_Click;

                sda.Update(dt);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Generiraj_raspored_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView.SelectedRows)
            {
                 value1 = row.Cells[0].Value.ToString();
                 value2 = row.Cells[1].Value.ToString();
                //MessageBox.Show( value1 + " " + value2 );
            }
            SqlDataAdapter sda = new SqlDataAdapter("Select Ime, Prezime from Studenti where Kolegij='" + value1 +"'", con);
            sda.Fill(dt);

            if (dt.Rows.Count > 0)
                foreach (DataRow dr in dt.Rows)
                {
                    Student osoba = new Student(dr["Ime"].ToString(), dr["Prezime"].ToString(), value1);
                    studenti.Add(osoba);
                }
            sda.Update(dt);            
        }
    }
}       

