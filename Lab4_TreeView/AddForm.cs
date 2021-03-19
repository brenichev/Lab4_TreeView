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
    public partial class AddForm : Form
    {
        string currentTable;
        string connectionString = @"Data Source=DESKTOP-P8ARIFA\SQLEXPRESS;Initial Catalog=KPO_Lab4;Integrated Security=True";
        int idtag;

        public AddForm()
        {
            InitializeComponent();
        }

        public AddForm(string Table, int id)
        {
            currentTable = Table;
            idtag = id;
            InitializeComponent();
            dataGridView1.DataError += new DataGridViewDataErrorEventHandler(DataGridView1_DataError);
            LoadData();
            
        }

        private void LoadData()
        {
            using (var cnn = new SqlConnection())
            {
                cnn.ConnectionString = connectionString;
                cnn.Open();
                string combo = "";
                try
                {
                    SqlCommand cmd = new SqlCommand($"select * from {currentTable}", cnn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        for (int i = 1; i < reader.FieldCount; i++)
                        {
                            if (reader.GetName(i) == "idEventType" || reader.GetName(i) == "idEvent")
                            {
                                combo = reader.GetName(i);
                                using (var cnn2 = new SqlConnection())
                                {
                                    string col1;
                                    string col2;
                                    if (reader.GetName(i) == "idEventType")
                                    {
                                        col1 = "idType";
                                        col2 = "EventTypeTable";
                                    }
                                    else
                                    {
                                        col1 = "idName";
                                        col2 = "EventsTable";
                                    }
                                    cnn2.ConnectionString = connectionString;
                                    cnn2.Open();
                                    string sql = $"SELECT {col1} FROM {col2}";
                                    SqlCommand cmd2 = new SqlCommand(sql, cnn2);
                                    DataGridViewComboBoxColumn column = new DataGridViewComboBoxColumn();
                                    using (var reader2 = cmd2.ExecuteReader())
                                    {
                                        if (reader2.HasRows)
                                        {
                                            column.Items.Clear();
                                            while (reader2.Read())
                                            {
                                                int row = reader2.GetInt32(0);
                                                column.Items.Add(row);
                                            }
                                        }
                                    }
                                    column.Name = reader.GetName(i);                                    
                                    dataGridView1.Columns.Add(column);
                                    
                                }
                            }
                            else
                            dataGridView1.Columns.Add(reader.GetName(i), reader.GetName(i));
                        }
                    }
                    dataGridView1.Rows.Add();
                    if(combo != "")
                    dataGridView1.Rows[0].Cells[combo].Value = idtag.ToString();
                }
                catch
                {
                    MessageBox.Show("Не удалось загрузить данные таблицы", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                }
            }
        }

        private void ComboLoad(ComboBox comboBox, string table, string col)
        {
            using (var cnn = new SqlConnection())
            {
                cnn.ConnectionString = connectionString;
                cnn.Open();
                try
                {
                    string sql = $"SELECT {col} FROM {table}";
                    SqlCommand cmd = new SqlCommand(sql, cnn);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            int ColIndex = reader.GetOrdinal(col);
                            comboBox.Items.Clear();
                            while (reader.Read())
                            {
                                string row = reader.GetString(ColIndex);
                                comboBox.Items.Add(row);
                            }
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Не удалось загрузить данные", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                }
            }
        }
        void DataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            object value = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            if (!((DataGridViewComboBoxColumn)dataGridView1.Columns[e.ColumnIndex]).Items.Contains(value))
            {
                ((DataGridViewComboBoxColumn)dataGridView1.Columns[e.ColumnIndex]).Items.Add(value);
                e.ThrowException = false;
            }


        }
    }
}
