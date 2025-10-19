using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private string connectionString = "Server=localhost;Database=demka;port=3306;username=root;password=root;";
        private MySqlConnection connection;

        public Form1()
        {
            InitializeComponent();
        }
        
        private void OpenConnection()
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();
        }
        private void CloseConnection()
        {
            if (connection != null && connection.State == System.Data.ConnectionState.Open)
                connection.Close();
        }
        private void LoadProducts()
        {
            flowLayoutPanelProducts.Controls.Clear();
            OpenConnection();

            string query = @"SELECT p.idProduct, p.artikul, p.naimenovanie, p.shirina_rulona, 
                            p.min_stoimost_partner, t.nazvanie AS tip, 
                            COALESCE(SUM(m.stoimost_za_ed * o.kolichestvo), 0) AS stoimost
                    FROM product p
                    JOIN tipy_produkcii t ON p.tipy_prod = t.idtipy_produkcii
                    LEFT JOIN osnovy_produkcii o ON o.Product_id = p.idProduct
                    LEFT JOIN materialy m ON m.idMaterials = o.materialy_id
                    GROUP BY p.idProduct";

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Panel productPanel = CreateProductPanel(reader);
                    flowLayoutPanelProducts.Controls.Add(productPanel);
                }
            }

            CloseConnection();
        }
        private Panel CreateProductPanel(MySqlDataReader reader)
        {
            Panel panel = new Panel();
            panel.Width = 600;
            panel.Height = 150;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Margin = new Padding(10);

            Label lblTop = new Label();
            lblTop.Text = reader["tip"].ToString() + " / " + reader["naimenovanie"].ToString();
            lblTop.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblTop.Dock = DockStyle.Top;

            Label lblCost = new Label();
            lblCost.Text = "Стоимость: " + reader["stoimost"].ToString() + " ₽";
            lblCost.TextAlign = ContentAlignment.MiddleRight;
            lblCost.Dock = DockStyle.Top;

            Label lblArtikul = new Label();
            lblArtikul.Text = "Артикул: " + reader["artikul"].ToString();
            lblArtikul.Dock = DockStyle.Top;

            Label lblMinPrice = new Label();
            lblMinPrice.Text = "Мин. цена партнёр: " + reader["min_stoimost_partner"].ToString();
            lblMinPrice.Dock = DockStyle.Top;

            Label lblWidth = new Label();
            lblWidth.Text = "Ширина рулона: " + reader["shirina_rulona"].ToString() + " м";
            lblWidth.Dock = DockStyle.Top;

            panel.Controls.Add(lblWidth);
            panel.Controls.Add(lblMinPrice);
            panel.Controls.Add(lblArtikul);
            panel.Controls.Add(lblCost);
            panel.Controls.Add(lblTop);

            Button btnEdit = new Button();
            btnEdit.Text = "Редактировать";
            btnEdit.Dock = DockStyle.Bottom;
            btnEdit.Height = 30;
            btnEdit.Tag = reader["idProduct"]; // сохраняем id для передачи

            btnEdit.Click += BtnEdit_Click;

            panel.Controls.Add(btnEdit);

            return panel;
        }
        private void BtnEdit_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            int productId = Convert.ToInt32(btn.Tag);

            // Открываем форму редактирования, передаём ID товара
            FormEditProduct form = new FormEditProduct(productId);
            form.ShowDialog();

            // После закрытия — перезагружаем список
            LoadProducts();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadProducts();

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void btnAddProduct_Click(object sender, EventArgs e)
        {
            FormEditProduct form = new FormEditProduct(); 
            form.ShowDialog();
            LoadProducts();
        }
    }
}
