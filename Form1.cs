using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace TelefonRehberiSistemi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        SqlConnection conn;
        SqlDataAdapter adapter;
        SqlCommand cmd;
        System.Data.DataTable tablo;

        private bool AlanlarıKontrolEt()
        {
            if (txtad.Text == "" || txtsoyad.Text == "" || txttelefon.Text == "")
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz!");
                return false;
            }
            return true;
        }

        void KisiGetir()
        {
            conn = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=dbRehber;Integrated Security=True;");
            adapter = new SqlDataAdapter("SELECT * FROM Kisiler", conn);
            tablo = new System.Data.DataTable();
            conn.Open();
            adapter.Fill(tablo);
            dgv.DataSource = tablo;
            conn.Close();
        }
        void KisiSayısı()
        {
            lblsay.Text = "Telefon Rehberinde " + dgv.RowCount + " Kişi Var! ";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            KisiGetir();
            KisiSayısı();
        }

        private void btnkaydet_Click(object sender, EventArgs e)
        {
            if (!AlanlarıKontrolEt()) return ; // ünlemin amacı eğer  kullanıcı alanları doldurmadı ise çalıştır demektir. return ise kaydet butonuna bastıktan sonra boş alan eklenmemesi içindir. Eğer return yazmazsak çalıştırmaya devam eder ve boş alan ekler.

            string sorgu = ("Insert into Kisiler(Ad, Soyad, Telefon) values (@Ad, @Soyad, @Telefon)");
            cmd = new SqlCommand(sorgu, conn);
            cmd.Parameters.AddWithValue("@Ad", txtad.Text);
            cmd.Parameters.AddWithValue("@Soyad", txtsoyad.Text);
            cmd.Parameters.AddWithValue("@Telefon", txttelefon.Text);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            KisiGetir();
            MessageBox.Show(txtad.Text + " "+ txtsoyad.Text + "Eklendi!");
        }

        private void btngüncelle_Click(object sender, EventArgs e)
        {
            string sorgu = ("update Kisiler set ad=@Ad, soyad=@soyad, Telefon=@Telefon Where Id=@id");
            cmd = new SqlCommand(sorgu, conn);
            cmd.Parameters.AddWithValue("@Ad", txtad.Text);
            cmd.Parameters.AddWithValue("@Soyad", txtsoyad.Text);
            cmd.Parameters.AddWithValue("@Telefon", txttelefon.Text);
            cmd.Parameters.AddWithValue("@id", dgv.CurrentRow.Cells[0].Value);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();   
            KisiGetir();
            MessageBox.Show(txtad.Text + " " + txtsoyad.Text + "Güncellendi!");
        }

        private void btnsil_Click(object sender, EventArgs e)
        {
            string ad = dgv.CurrentRow.Cells[1].Value.ToString();
            string soyad = dgv.CurrentRow.Cells[2].Value.ToString();    
            string sorgu = "Delete Kisiler where Id=@id";
            cmd = new SqlCommand(sorgu, conn);
            cmd.Parameters.AddWithValue("@id", dgv.CurrentRow.Cells[0].Value);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            KisiGetir();
            MessageBox.Show(ad+" "+soyad+" Adlı Kişi Silindi!");

        }
        private void dgvKisiler_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            txtad.Text= dgv.CurrentRow.Cells[1].Value.ToString();
            txtsoyad.Text= dgv.CurrentRow.Cells[2].Value.ToString();
            txttelefon.Text= dgv.CurrentRow.Cells[3].Value.ToString();
        }

        private void btnara_Click(object sender, EventArgs e)
        {
            try
            {
                DataView dv = tablo.DefaultView;
                dv.RowFilter = "Ad Like'" + txtara.Text + "%'";
                dgv.DataSource = dv;
            }
            catch (Exception)
            {

                MessageBox.Show("Bulunamadı!");
            }
        }

        private void btnexcel_Click(object sender, EventArgs e)
        {
            // Nesne Üretildi
            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            // Excel Görünür Hale Getirme
            excel.Visible = true;
            // Excel de yeni bir çalışma sayfası açma.
            Workbook workbook = excel.Workbooks.Add(System.Reflection.Missing.Value);
            // Başlangıç yerini seçme. Yani excelde birinci sayfadan başladığımzı söylüyoruz.
            Worksheet sheet1 = (Worksheet)workbook.Sheets[1];
            // Başlangıç Sütununu Seç
            int StartCol = 1;
            // Başlangıç Satırnı Seç 
            int StartRow = 1;
            // DataGridView sütun sayıları kadar dönü ekle
            for (int j =0; j<dgv.ColumnCount; j++) // Sütun başlıklarını atma.
            {
                // Excel başlıklarını atma.
                Range myRange = (Range)sheet1.Cells[StartRow, StartCol + j]; // sayfamın hücrelerine satır ve j kadar sütun ekle.
                myRange.Value2 = dgv.Columns[j].HeaderText; // j den aldığım verileri değer olarak ekle.
            }
            StartRow++; // Colon sayısı bittiğinde satırları arttır.
            // DataGridView sütun sayıları kadar dönü ekle
            for (int i=0; i<dgv.RowCount; i++) // verileri yazdırdığımız bölüm
            {
                for (int j=0; j<dgv.Columns.Count; j++)
                {
                    Range myRange = (Range)sheet1.Cells[StartRow + i, StartRow + j];
                    myRange.Value2 = dgv.Rows[i].Cells[j].Value;
                    myRange.EntireColumn.AutoFit(); // Excelde alanların boyutunu otomatik oluşturur.
                }
            }

        }
    }
}
