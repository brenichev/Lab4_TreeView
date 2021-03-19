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

namespace Lab4_TreeView
{
    public partial class EditForm : Form
    {
        string connectionString = @"Data Source=DESKTOP-P8ARIFA\SQLEXPRESS;Initial Catalog=KPO_Lab4;Integrated Security=True";
        string currentTable;
        string id;
        int id2;
        public EditForm()
        {
            InitializeComponent();
        }

        public EditForm(string Table, string idstr, int idnum)
        {
            currentTable = Table;
            id = idstr;
            id2 = idnum;
            InitializeComponent();
            LoadData();
        }


        private void LoadData()
        {
            using (var cnn = new SqlConnection())
            {
                cnn.ConnectionString = connectionString;
                cnn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand($"select * from {currentTable} where {id} = @id", cnn);
                    cmd.Parameters.AddWithValue("@id", id2);


                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            dataGridView1.Columns.Add(reader.GetName(i), reader.GetName(i));
                        }
                        dataGridView1.Rows.Add();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            dataGridView1.Rows[0].Cells[i].Value = reader.GetValue(i);
                        }
                        dataGridView1.Columns[0].ReadOnly = true;
                    }

                }
                catch
                {
                    MessageBox.Show("Не удалось выполнить чтение данных", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                }
            }
        }
    }
}
