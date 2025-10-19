using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace WindowsFormsApp1
{
    public partial class FormEditProduct : Form
    {
        private int? productId;
        private string connectionString = "Server=localhost;Database=demka;port=3306;username=root;password=root;";


        public FormEditProduct()
        {
            InitializeComponent();
            productId = null;
            this.Text = "Добавление товара";

        }
        public FormEditProduct(int id)
        {
            InitializeComponent();
            productId = id;
            this.Text = "Редактирование товара";
         
        }
        private void LoadProductData()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT naimenovanie, artikul FROM product WHERE idProduct = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", productId);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        txtName.Text = reader["naimenovanie"].ToString();
                        txtArtikul.Text = reader["artikul"].ToString();
                    }
                }
            }
        }

        private void FormEditProduct_Load(object sender, EventArgs e)
        {
            if (productId != null)
                LoadProductData();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query;
                if (productId == null)
                {
                    query = "INSERT INTO product (naimenovanie, artikul, tipy_prod, shirina_rulona, min_stoimost_partner) VALUES (@name, @artikul, 1, 1.0, 0)";
                }
                else
                {
                    query = "UPDATE product SET naimenovanie = @name, artikul = @artikul WHERE idProduct = @id";
                }

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", txtName.Text);
                cmd.Parameters.AddWithValue("@artikul", txtArtikul.Text);

                if (productId != null)
                    cmd.Parameters.AddWithValue("@id", productId);

                cmd.ExecuteNonQuery();
            }

            this.Close();
        }
    }
}
