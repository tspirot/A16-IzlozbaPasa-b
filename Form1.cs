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

namespace A16_IzlozbaPasa_b
{
    public partial class Form1 : Form
    {
        SqlConnection konekcija;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                konekcija = new SqlConnection
                    (@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\A16.mdf;Integrated Security=True");
                //citanje iz baze
                OsveziPse();
                OsveziIzlozbe();
                OsveziKategorije();
            }
            catch (Exception)
            {
                MessageBox.Show("Greska u konekciji");
            }
        }
        // popunjavanje comboboxa pasa
        private void OsveziPse()
        {
            try
            {
                SqlCommand komanda = new SqlCommand("SELECT PasID, " +
                    "CONCAT(PasID,' - ',Ime) AS ImePsa FROM Pas", konekcija);
                SqlDataAdapter adapter = new SqlDataAdapter(komanda);
                DataTable tabela = new DataTable();
                adapter.Fill(tabela);
                comboBoxPas.Items.Clear();
                comboBoxPas.DataSource = tabela;
                comboBoxPas.DisplayMember = "ImePsa";
                comboBoxPas.ValueMember = "PasID";

            }
            catch (Exception ex)
            {
                MessageBox.Show("Greska u citanju pasa:" + ex.Message);
            }
        }
        // popunjavanje comboboxa za Izlozbe
        private void OsveziIzlozbe()
        {
            try
            {
                SqlCommand komanda = new SqlCommand("SELECT IzlozbaID, " +
                    "CONCAT(IzlozbaID,' - ',Mesto,' - ',Datum)" +
                    "AS NazivIzlozbe FROM Izlozba", konekcija);
                SqlDataAdapter adapter = new SqlDataAdapter(komanda);
                DataTable tabela = new DataTable();
                adapter.Fill(tabela);
                comboBoxIzlozba.Items.Clear();
                comboBoxIzlozba.DataSource = tabela;
                comboBoxIzlozba.DisplayMember = "NazivIzlozbe";
                comboBoxIzlozba.ValueMember = "IzlozbaID";

            }
            catch (Exception ex)
            {
                MessageBox.Show("Greska u citanju izlozbe:" + ex.Message);
            }
        }
        // popunjavanje comboboxa kategorija
        private void OsveziKategorije()
        {
            try
            {
                SqlCommand komanda = new SqlCommand("SELECT KategorijaID, " +
                    "CONCAT(KategorijaID,' - ',Naziv) AS ImeKategorije FROM Kategorija", 
                    konekcija);
                SqlDataAdapter adapter = new SqlDataAdapter(komanda);
                DataTable tabela = new DataTable();
                adapter.Fill(tabela);
                comboBoxKategorija.Items.Clear();
                comboBoxKategorija.DataSource = tabela;
                comboBoxKategorija.DisplayMember = "ImeKategorije";
                comboBoxKategorija.ValueMember = "KategorijaID";

            }
            catch (Exception ex)
            {
                MessageBox.Show("Greska u citanju kategorija:" + ex.Message);
            }
        }

        private void buttonPrijava_Click(object sender, EventArgs e)
        {
            if(comboBoxPas.SelectedIndex == -1 
                || comboBoxIzlozba.SelectedIndex == -1 
                || comboBoxKategorija.SelectedIndex == -1)
            {
                MessageBox.Show("Morate izabrati psa, izlozbu i kategoriju");
                return;
            }
            string provera = "select count(*) from Rezultat " +
                "where PasID=@Pas " +
                "and IzlozbaID=@Izlozba "+
                "and KategorijaID=@Kategorija";
            SqlCommand komandaProvera = new 
                SqlCommand(provera, konekcija);
            komandaProvera.Parameters.AddWithValue
                ("@Pas", comboBoxPas.SelectedValue);
            komandaProvera.Parameters.AddWithValue
                ("@Izlozba", comboBoxIzlozba.SelectedValue);
            komandaProvera.Parameters.AddWithValue
                ("@Kategorija", comboBoxKategorija.SelectedValue);
            konekcija.Open();
            int broj = (int)komandaProvera.ExecuteScalar();
            if(broj > 0)
            {
                MessageBox.Show("Pas je vec prijavljen za izlozbu u toj kategoriji");
                return;
            }
            konekcija.Close();

            // prijava psa na izlozbu
        }
    }
}
