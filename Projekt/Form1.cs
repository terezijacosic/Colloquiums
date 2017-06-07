using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;

namespace Projekt
{   /// <summary>
        /// napravite aplikaciju za kreiranje razmještaja studenata po predavaonicama u vrijeme pisanja kolokvija (na našem odsjeku).
        /// 
        ///  Aplikacija je povezana s bazom u kojoj se nalaze imena studenata te kolegij iz kojeg pišu kolokvij i u koliko sati, 
        /// raspoloživost dvorana u zadanom terminu i njihove karakteristike. Program bi trebao razmjestiti studente tako da studenti    
        /// koji pišu kolokvij iz istog kolegija ne sjede jedan do drugog i da, ako je ikako moguće, između dva studenta postoji barem jedno mjesto razmaka.
        ///  Aplikacija mora dozvoliti grafički pregled razmještaja studenata po predavaonicama (zauzete stolice neka bude obojene različitim bojama ovisno o kolegiju, 
        /// a za svako mjesto je moguće dobiti osnovne podatke o studentu). Korisnik aplikacije s višom razinom pristupa može ručno zamijeniti mjesto dva studenta.
        /// </summary>
        /// 
    public partial class Form1 : Form
    {
        public int profesor;
        public TableLayoutPanel panel = new TableLayoutPanel()
        {            
            BackColor = Color.Transparent,
            CellBorderStyle = TableLayoutPanelCellBorderStyle.Outset
        };
        public Label labela_oblak = new Label() { Location = new Point(80, 100), MaximumSize = new Size(100, 0), AutoSize = true, BackColor = Color.Transparent };             

        //string tkojeulogiran;
        public Form1()
        {
            InitializeComponent();
            provjeraKorisnik provjera1 = new provjeraKorisnik();
            profesor = provjera1.provjeraStudent();
            if(profesor==0)
            {
                toolsToolStripMenuItem.Enabled = false;
                zaProfesoreToolStripMenuItem.Enabled = false;

            }
            

        }

       /* public void Form1_Load(object sender, EventArgs e)
        {           
            th[Test]is.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
            
        }*/

        // koristimo za micanje svih kontrola osim menija i kako bi radio prikaz rasporeda
        public int controlsRemove()
        {
            Form form1 = Application.OpenForms["Form1"];
            int nađeno = 0;

            foreach (Control item in form1.Controls.OfType<TableLayoutPanel>())
                form1.Controls.Remove(item);           
            foreach (Control item in form1.Controls.OfType<Button>())
                if (item.Name != "btn_logout")
                    form1.Controls.Remove(item);
            foreach (Control item in form1.Controls.OfType<Panel>())
                form1.Controls.Remove(item);
            foreach (Control item in form1.Controls.OfType<Label>())
                form1.Controls.Remove(item);
            foreach (Control item in form1.Controls.OfType<TextBox>())
                form1.Controls.Remove(item);
            foreach (Control item in form1.Controls.OfType<ComboBox>())
                form1.Controls.Remove(item);
            foreach (Control item in form1.Controls.OfType<DateTimePicker>())
                form1.Controls.Remove(item);

            return nađeno;
        }
        
        // pregled dvorana
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            controlsRemove();
            DvoraneView dvoraneView = new DvoraneView();
        }

      
        // pregledaj studente    
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controlsRemove();          
            StudentiView studentiView = new StudentiView();                  
        }

        private void btn_logout_Click(object sender, EventArgs e)
        {
            provjeraKorisnik provjera = new provjeraKorisnik();
            provjera.postaviOdjavu();
            LogIn l = new LogIn();
            l.Show();
            this.Close();
        }

       
       

        // pregledaj kolegije
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controlsRemove();
            KolegijiView kolegijiView = new KolegijiView();           
        }
      
        // zatvaranje aplikacije
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            provjeraKorisnik provjera = new provjeraKorisnik();
            provjera.postaviOdjavu();
            Close();
        }

        //ovdje prebaciti rezervacije dvorana
        private void toolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controlsRemove();
            RezervirajView rezervirajView = new RezervirajView();

        }
        // nacrtaj raspored

        private void zaProfesoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
                controlsRemove();
                RasporedView raspred = new RasporedView();
            
        }

        private void zaStudenteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            controlsRemove();
            RasporedView raspred = new RasporedView();
        }
           
    } 
    }


