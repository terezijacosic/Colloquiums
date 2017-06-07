using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projekt
{
    public class RasporedView : Form1
    {
        public ToolTip tooltip = new ToolTip() { IsBalloon = true };
        public Color pocetna_boja = Color.Transparent;
        public Dictionary<string, Color> zauzeta_boja = new Dictionary<string, Color>(){ //boje zuta:fcee21, roza:ffaaac, zelena:c9d439, 9eb11f
            { "Softversko inženjerstvo", ColorTranslator.FromHtml("#fcee21") },
            { "Interpretacija programa", ColorTranslator.FromHtml("#ffaaac") },
            { "RP3", ColorTranslator.FromHtml("#c9d439") },
            { "Android", ColorTranslator.FromHtml("#9eb11f") },
            { "Uvod u složeno pretraživanje podataka", ColorTranslator.FromHtml("#ffe6e6") }}; 
       
        public Student[,] podaci;
        public int zamjena = -1;
        public int red_zamjena = 0;
        public int stupac_zamjena = 0;
        public Form form1 = Application.OpenForms["Form1"];  
        public ComboBox lista_dvorana;
        public Button btn = new Button();
        DateTimePicker date;
        TextBox termin;
       

        public RasporedView()
        {
            
            panel.Location = new Point(ClientRectangle.Width / 2 - panel.Width / 2, 190);

            SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\projekt.mdf;Integrated Security=True");
            con.Open();
            string upit = "SELECT Ime,Redaka,Stupaca FROM Dvorane";
            SqlCommand cmd = new SqlCommand(upit, con);
            SqlDataReader dr = cmd.ExecuteReader();
            List<string> dvorane = new List<string>();
            while (dr.Read()) { dvorane.Add(dr["Ime"].ToString()); }
            dr.Close();

            date = new DateTimePicker();
            date.Location = new Point(ClientRectangle.Width / 2 - date.Width / 2, 100);
            //{ Location = new Point(550, 150) };
            form1.Controls.Add(date);

            Label termin_label = new Label() { Text = "Termin:", BackColor = Color.Transparent };
            termin_label.Location = new Point(date.Left, 130); 
            termin = new TextBox() { Location = new Point(termin_label.Right + 10, termin_label.Top), Text = "15" };
            form1.Controls.Add(termin_label); form1.Controls.Add(termin);

            Label lista_dvorana_label = new Label() { Text = "Izaberite dvoranu:" , BackColor = Color.Transparent };
            lista_dvorana_label.Location = new Point(date.Left, 160);
            lista_dvorana = new ComboBox();
            lista_dvorana.Location = new Point(lista_dvorana_label.Right+10, 160);
            foreach(string dv in dvorane)
                lista_dvorana.Items.Add(dv);
            lista_dvorana.SelectedIndex = 0;
            lista_dvorana.SelectedIndexChanged += new System.EventHandler(promjeni_dvoranu);
            form1.Controls.Add(lista_dvorana); form1.Controls.Add(lista_dvorana_label);

            if (profesor == 1)
            {
                btn.Text = "Zamijeni studente";
                btn.Location = new Point(date.Left, 400);
                btn.MouseClick += new MouseEventHandler(zamjenaMjesta);
                form1.Controls.Add(btn);
            }
            


            napraviUcionicu(5,4);
            popuni_ucionicu(5,4);
            con.Close();

        }


        public void promjeni_dvoranu(object sender, EventArgs e)
        {           
            form1.SuspendLayout();          
            panel.Controls.Clear();
            form1.ResumeLayout();

            SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\projekt.mdf;Integrated Security=True");
            con.Open();
            string upit = "SELECT Redaka,Stupaca FROM Dvorane WHERE Ime=@ime";
            SqlCommand cmd = new SqlCommand(upit, con);
            cmd.Parameters.AddWithValue("@ime", lista_dvorana.SelectedItem.ToString());
            SqlDataReader dr = cmd.ExecuteReader();
            dr.Read();
            napraviUcionicu( Convert.ToInt32(dr["Redaka"]), Convert.ToInt32(dr["Stupaca"]));
            popuni_ucionicu( Convert.ToInt32(dr["Redaka"]), Convert.ToInt32(dr["Stupaca"]));
            dr.Close();
            con.Close();
        }
            

        public void napraviUcionicu(int stupci, int redovi)
        {        
            int row = redovi;
            int col = stupci;
            //Image img = Image.FromFile(@"C:\Users\tcosic\Downloads\stolica.png");
            Image img = Image.FromFile(@"C:\Users\Racunalo\Documents\Visual Studio 2015\Projects\Projekt\Projekt\Resources\stolica.png");
            podaci = new Student[row, col];        
           
            panel.Height = 50 * row;
            panel.Width = 50 * col;            
            form1.Controls.Add(panel);
            panel.SuspendLayout();

            panel.ColumnCount = col;
            panel.RowCount = 1;
            for (int i = 0; i < col; ++i)
            {
                panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
                panel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            }

            for (int j = 0; j < row; ++j)
            {
                if (j != row - 1)
                    panel.RowCount = panel.RowCount + 1;
                panel.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
                for (int i = 0; i < col; ++i)
                    panel.Controls.Add(new Label() { Image = img, Dock = DockStyle.Fill }, i, j);

            }
            
            foreach (Label space in panel.Controls)
            {
                space.MouseClick += new MouseEventHandler(clickOnSpace);
                space.MouseHover += new EventHandler(HoverData);
            }

            this.panel.ResumeLayout();

        }

        public void popuni_ucionicu( int stupaca, int redaka )
        {
            SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\projekt.mdf;Integrated Security=True");
            con.Open();
            string upit = "SELECT Kolegij FROM Rezervacije WHERE Dvorana=@dvorana AND Termin=@termin AND Datum=@datum";
            SqlCommand cmd = new SqlCommand(upit, con);
            cmd.Parameters.AddWithValue("@dvorana", lista_dvorana.SelectedItem.ToString() );
            cmd.Parameters.AddWithValue("@termin", termin.Text);
            cmd.Parameters.AddWithValue("@datum", date.Value.ToShortDateString());

            SqlDataReader dr = cmd.ExecuteReader();
            List<string> kolegiji = new List<string>();
            while (dr.Read())
                kolegiji.Add( Convert.ToString( dr["Kolegij"] ) );
            dr.Close();

            int koliko = kolegiji.Count;
            int k = 0;
            foreach( string kol in kolegiji )
            {
                int i = 0;  int j = 0+2*k;
                upit = "SELECT Ime,Prezime FROM Studenti WHERE Kolegij='"+kol+"'"+" AND Vrijeme=@termin AND Datum=@datum";
                cmd = new SqlCommand(upit, con);
                cmd.Parameters.AddWithValue("@termin", termin.Text);
                cmd.Parameters.AddWithValue("@datum", date.Value.ToShortDateString());
                dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Student s = new Student(Convert.ToString(dr["Ime"]), Convert.ToString(dr["Prezime"]), kol);

                    if( !jeliZauzeto(i%redaka, j%stupaca) )
                        zauzmiSjedalo(i % redaka, j % stupaca, s);
                    else
                    {
                        for (int red = 0; red < redaka; ++red)
                            for (int stup = 0; stup < stupaca; stup = stup + 2)
                                if (!jeliZauzeto(red, stup))
                                    zauzmiSjedalo(red, stup, s);
                    }

                    for (int x = 0; x < koliko; ++x)
                    {
                        if (j == stupaca - 1) { ++i; j = 0; }
                        else if (j == stupaca - 2) { ++i; j = 0; } //za cik-cak j = 1
                        else { ++j; ++j; }
                    }
                }

                ++k;
                dr.Close();
            }          
            con.Close();

        }

        public bool zauzmiSjedalo(int i, int j, Student s)
        {
            if (i < 0 || panel.RowCount <= i || j < 0 || panel.ColumnCount <= j)
            {
                //mozda izbaciti error ili samo poruku o greški??
                MessageBox.Show("Indeksi ne valjaju!");
            }
            else if (panel.GetControlFromPosition(j, i).BackColor == pocetna_boja) //(col,row) iz nekog razloga
            {
                podaci[i, j] = s;
                panel.GetControlFromPosition(j, i).BackColor = zauzeta_boja[s.Kolegij];
                return true;
            }

            return false;
        }

        public bool jeliZauzeto(int i, int j)
        {
            if (panel.GetControlFromPosition(j, i).BackColor == pocetna_boja)
                return false;
            return true;
        }


        public void clickOnSpace(object sender, MouseEventArgs e)
        {
            if (zamjena == 0 || zamjena == 1)
            {
                int row = panel.GetRow((Label)sender);
                int col = panel.GetColumn((Label)sender);
                // MessageBox.Show("Cell chosen: (" + row + ", " + col + ")");

                if (jeliZauzeto(row, col))
                {
                    if (zamjena == 0)
                    {
                        red_zamjena = row;
                        stupac_zamjena = col;
                        zamjena = 1;
                    }
                    else if (zamjena == 1)
                    {
                        Student temp = podaci[red_zamjena, stupac_zamjena];
                        podaci[red_zamjena, stupac_zamjena] = podaci[row, col];
                        podaci[row, col] = temp;
                        zamjena = -1;
                        Color tem = panel.GetControlFromPosition(stupac_zamjena, red_zamjena).BackColor;
                        panel.GetControlFromPosition(stupac_zamjena, red_zamjena).BackColor = panel.GetControlFromPosition(col, row).BackColor;
                        panel.GetControlFromPosition(col, row).BackColor = tem;
                        MessageBox.Show("Studenti zamjenjeni");
                    }
                }
            }
        }

        public void HoverData(object sender, EventArgs e)
        {
            int row = panel.GetRow((Label)sender);
            int col = panel.GetColumn((Label)sender);

            if (jeliZauzeto(row, col))
            {
                Student student = podaci[row, col];
                Control celija = panel.GetControlFromPosition(col, row);
                tooltip.SetToolTip(celija, student.ToString() );
               
            }
        }

        public void zamjenaMjesta(object sender, MouseEventArgs e)
        {
            zamjena = 0;
            //sova prica??? :D
            MessageBox.Show("Klikni na studente koje želiš zamjeniti");
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // RasporedView
            // 
            this.ClientSize = new System.Drawing.Size(1008, 561);
            this.Name = "RasporedView";
            this.Load += new System.EventHandler(this.RasporedView_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void RasporedView_Load(object sender, EventArgs e)
        {

        }
    }
}
