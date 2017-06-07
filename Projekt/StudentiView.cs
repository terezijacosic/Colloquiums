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
   public class StudentiView : Form1
    {
        Form form1 = Application.OpenForms["Form1"];
        Panel panel_studenti;
        Label label_studenti;
        DataGridView dataGridView = new DataGridView()
        {
            //Location = new Point(210, 150),
            BackgroundColor = Color.White,
            MaximumSize = new Size(600, 350),
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
        };
        public StudentiView()
        {
            panel_studenti = new Panel() { Location = new Point(0, 20), BackColor = Color.Transparent, Size = new Size(ClientRectangle.Width, ClientRectangle.Height) };
            label_studenti = new Label() {
                Location = new Point(80, 100),
                MaximumSize = new Size(100, 100),
                AutoSize = true,
                BackColor = Color.Transparent,
                Text = "Ovdje možete vidjeti popis svih studenata i kolokvija koje pišu." };
            panel_studenti.Controls.Add(label_studenti);

            form1.Controls.Add(panel_studenti);            

            //tooltip.SetToolTip( labela_oblak, "Izaberite datum za koji želite vidjeti zauzeća dvorana.");

            try
            {
                SqlConnection con = new SqlConnection("Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\\projekt.mdf;Integrated Security=True");
                SqlDataAdapter sda = new SqlDataAdapter("Select * from Studenti", con);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                BindingSource bSource = new BindingSource();

                bSource.DataSource = dt;
                panel_studenti.Controls.Add(dataGridView);

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
                dataGridView.Location = new Point(ClientRectangle.Width/2 - dataGridView.Width/2, 100);

                sda.Update(dt);
                //active_panel.Visible = true;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
