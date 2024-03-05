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
                OsveziIzlozbe1();
                OsveziIzlozbe2();
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
        private void OsveziIzlozbe1()
        {
            try
            {
                SqlCommand komanda = new SqlCommand(
                    "SELECT IzlozbaID, " +
                    "CONCAT(IzlozbaID,' - ',Mesto,' - ',Datum)" +
                    "AS NazivIzlozbe " +
                    "FROM Izlozba " +
                    "WHERE Datum>=GETDATE()", konekcija);
                SqlDataAdapter adapter = new SqlDataAdapter(komanda);
                DataTable tabela = new DataTable();
                adapter.Fill(tabela);
                comboBoxIzlozba.DataSource = tabela;
                comboBoxIzlozba.DisplayMember = "NazivIzlozbe";
                comboBoxIzlozba.ValueMember = "IzlozbaID";

            }
            catch (Exception ex)
            {
                MessageBox.Show("Greska u citanju izlozbe:" + ex.Message);
            }
        }
        private void OsveziIzlozbe2()
        {
            try
            {
                SqlCommand komanda = new SqlCommand(
                    "SELECT IzlozbaID, " +
                    "CONCAT(IzlozbaID,' - ',Mesto,' - ',Datum)" +
                    "AS NazivIzlozbe " +
                    "FROM Izlozba " +
                    "WHERE Datum<=GETDATE()", konekcija);
                SqlDataAdapter adapter = new SqlDataAdapter(komanda);
                DataTable tabela = new DataTable();
                adapter.Fill(tabela);
                comboBox1.DataSource = tabela;
                comboBox1.DisplayMember = "NazivIzlozbe";
                comboBox1.ValueMember = "IzlozbaID";

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
                konekcija.Close();
                return;
            }
            konekcija.Close();
            // prijava psa na izlozbu
            string sqlPrijava= "INSERT INTO Rezultat " +
                "(PasID, IzlozbaID, KategorijaID) " +
                "VALUES (@PasID, @IzlozbaID, @KategorijaID)";
            SqlCommand komandaPrijava = new 
                SqlCommand(sqlPrijava, konekcija);
            komandaPrijava.Parameters.AddWithValue
                ("@PasID", comboBoxPas.SelectedValue);
            komandaPrijava.Parameters.AddWithValue
                ("@IzlozbaID", comboBoxIzlozba.SelectedValue);
            komandaPrijava.Parameters.AddWithValue
                ("@KategorijaID", comboBoxKategorija.SelectedValue);
            try
            {
                konekcija.Open();
                komandaPrijava.ExecuteNonQuery();
                MessageBox.Show("Pas je uspesno prijavljen za izlozbu");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greska pri prijavi:" + ex.Message);
            }
            finally
            {
                konekcija.Close();
            }
        }

        private void buttonIzadji_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonPrikazi_Click(object sender, EventArgs e)
        {
            // prikazivanje rezultata
            DataTable tabela= new DataTable();
            try
            {
                string upit="SELECT k.KategorijaID AS Sifra, " +
                    "k.Naziv AS NazivKategorije, " +
                    "COUNT(*) AS BrojPasa " +
                    "FROM Kategorija k, Rezultat r " +
                    "WHERE k.KategorijaID=r.KategorijaID " +
                    "AND r.IzlozbaID=@IzlozbaID " +
                    "AND LEN(r.Napomena)>0 " +
                    "GROUP BY k.KategorijaID, k.Naziv";
                SqlDataAdapter adapter = new 
                    SqlDataAdapter(upit, konekcija);
                adapter.SelectCommand.Parameters.AddWithValue
                    ("@IzlozbaID", comboBox1.SelectedValue);
                adapter.Fill(tabela);
                dataGridView1.DataSource = tabela;
                chart1.DataSource = tabela;
                chart1.Series[0].XValueMember = "NazivKategorije";
                chart1.Series[0].YValueMembers = "BrojPasa";
                chart1.Series[0].IsValueShownAsLabel = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Greska!" + ex.Message);
            }
        }
    }
}
