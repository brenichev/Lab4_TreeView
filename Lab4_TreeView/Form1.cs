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
    public partial class Form1 : Form
    {
        string connectionString = @"Data Source=DESKTOP-P8ARIFA\SQLEXPRESS;Initial Catalog=KPO_Lab4;Integrated Security=True";
        public Form1()
        {
            InitializeComponent();
            LoadEventTypes();
        }

        private void LoadTree()
        {
            treeView1.Nodes.Clear();
            LoadEventTypes();
        }
        private void LoadEventTypes()
        {
            using (var cnn = new SqlConnection())
            {
                cnn.ConnectionString = connectionString;
                cnn.Open();
                var cmd = new SqlCommand("Select * from EventTypeTable", cnn);

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        TreeNode n = new TreeNode(dr["EventType"].ToString());
                        n.Tag = dr["idType"];
                        treeView1.Nodes.Add(n);
                        LoadEventNames(int.Parse(dr["idType"].ToString()), n);
                    }
                }
            }
        }

        private void LoadEventNames(int EventTypeId, TreeNode node)
        {
            using (var cnn = new SqlConnection())
            {
                cnn.ConnectionString = connectionString;
                cnn.Open();
                var cmd = new SqlCommand("Select * From EventsTable Where idEventType = @idType", cnn);

                cmd.Parameters.AddWithValue("@idType", EventTypeId);

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        TreeNode n = new TreeNode(dr["EventName"].ToString());
                        n.Tag = dr["idName"];
                        node.Nodes.Add(n);
                        LoadEventMember(int.Parse(dr["idName"].ToString()), n);
                    }
                }
            }
        }

        private void LoadEventMember(int EventTypeId, TreeNode node)
        {
            using (var cnn = new SqlConnection())
            {
                cnn.ConnectionString = connectionString;
                cnn.Open();
                var cmd = new SqlCommand("Select * From EventMember Where idEvent = @idEvent", cnn);

                cmd.Parameters.AddWithValue("@idEvent", EventTypeId);

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        TreeNode n = new TreeNode(dr["FIOMember"].ToString());
                        n.Tag = dr["id"];
                        node.Nodes.Add(n);
                    }
                }
            }
        }

        private void addValues(ref SqlCommand cmd, DataGridViewCellCollection collection)
        {
            int i = 1;
            string values = "(";
            foreach (DataGridViewCell cell in collection)
            {
                cmd.Parameters.AddWithValue(values, cell.Value);
                i++;
            }
            values += "(";
            string t = '@' + i.ToString();
            cmd.Parameters.AddWithValue(t, (int)treeView1.SelectedNode.Tag);

        }

        private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string table = "";
            string command = "INSERT INTO ";
            switch (treeView1.SelectedNode.Level)
            {
                case 0:
                    {
                        table = "EventTypeTable";
                        break;
                    }
                case 1:
                    {
                        table = "EventsTable";
                        break;
                    }
                case 2:
                    {
                        table = "EventMember";
                        break;
                    }
            }
            int tag = 1;
            if (treeView1.SelectedNode.Parent != null)
                tag = (int)treeView1.SelectedNode.Parent.Tag;
            AddForm form = new AddForm(table, tag);

            if (form.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (var cnn = new SqlConnection())
                    {
                        cnn.ConnectionString = connectionString;
                        cnn.Open();
                        string values = "(";
                        int res;
                        foreach (DataGridViewCell cell in form.dataGridView1.Rows[0].Cells)
                        {

                            if (int.TryParse(cell.Value.ToString(), out res))
                            {
                                values += cell.Value + ", ";
                            }
                            else
                            {
                                values += "'" + cell.Value + "', ";
                            }
                        }
                        values = values.Remove(values.Length - 2);
                        values += ")";
                        command += table + " VALUES " + values;
                        var cmd = new SqlCommand(command, cnn);
                        //addValues(ref cmd, form.dataGridView1.Rows[0].Cells);
                        cmd.ExecuteNonQuery();
                        LoadTree();
                    }
                }
                catch
                {
                    MessageBox.Show("Данные были введены некорректно", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DeleteEventType(int id, SqlConnection cnn)
        {

            var cmd = new SqlCommand("delete from EventTypeTable where idType = @id", cnn);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();

        }

        private void DeleteEvent(int id, SqlConnection cnn)
        {

            var cmd = new SqlCommand("delete from EventsTable where idName = @id", cnn);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();

        }
        private void DeleteMember(int id, SqlConnection cnn)
        {

            var cmd = new SqlCommand("delete from EventMember where id = @id", cnn);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();

        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null) return;
            TreeNode n = treeView1.SelectedNode;

            using (var cnn = new SqlConnection())
            {
                cnn.ConnectionString = connectionString;
                cnn.Open();
                switch (n.Level)
                {
                    case 0:
                        {
                            DeleteEventType((int)treeView1.SelectedNode.Tag, cnn);
                            treeView1.SelectedNode.Remove();
                            break;
                        }
                    case 1:
                        {
                            DeleteEvent((int)treeView1.SelectedNode.Tag, cnn);
                            treeView1.SelectedNode.Remove();
                            break;
                        }
                    case 2:
                        {
                            DeleteMember((int)treeView1.SelectedNode.Tag, cnn);
                            treeView1.SelectedNode.Remove();
                            break;
                        }
                }
            }
        }

        private void изменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string table = "";
            string command = "UPDATE ";
            string id = "";
            switch (treeView1.SelectedNode.Level)
            {
                case 0:
                    {
                        table = "EventTypeTable";
                        id = "idType";
                        break;
                    }
                case 1:
                    {
                        table = "EventsTable";
                        id = "idName";
                        break;
                    }
                case 2:
                    {
                        table = "EventMember";
                        id = "id";
                        break;
                    }
            }
            EditForm form = new EditForm(table, id, (int)treeView1.SelectedNode.Tag);
            if (form.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (var cnn = new SqlConnection())
                    {
                        cnn.ConnectionString = connectionString;
                        cnn.Open();
                        string values = "";
                        int res;
                        int i = 0;
                        foreach (DataGridViewCell cell in form.dataGridView1.Rows[0].Cells)
                        {
                            if (i != 0)
                                if (int.TryParse(cell.Value.ToString(), out res))
                                {
                                    values += form.dataGridView1.Columns[i].HeaderText.ToString() + " = " + cell.Value + ", ";
                                }
                                else
                                {
                                    values += form.dataGridView1.Columns[i].HeaderText.ToString() + " = " + "'" + cell.Value + "', ";
                                }
                            i++;
                        }
                        values = values.Remove(values.Length - 2);
                        command += table + " SET " + values + " WHERE " + id + " = " + (int)treeView1.SelectedNode.Tag;
                        var cmd = new SqlCommand(command, cnn);
                        //addValues(ref cmd, form.dataGridView1.Rows[0].Cells);
                        cmd.ExecuteNonQuery();
                        LoadTree();
                    }
                }
                catch
                {
                    MessageBox.Show("Данные были введены некорректно", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void добавитьВПунктToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string table = "";
            string command = "INSERT INTO ";
            switch (treeView1.SelectedNode.Level)
            {
                case 0:
                    {
                        table = "EventsTable";
                        break;
                    }
                case 1:
                    {
                        table = "EventMember";
                        break;
                    }
            }
            AddForm form = new AddForm(table, (int)treeView1.SelectedNode.Tag);
            if (form.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (var cnn = new SqlConnection())
                    {
                        cnn.ConnectionString = connectionString;
                        cnn.Open();
                        string values = "(";
                        int res;
                        foreach (DataGridViewCell cell in form.dataGridView1.Rows[0].Cells)
                        {

                            if (int.TryParse(cell.Value.ToString(), out res))
                            {
                                values += cell.Value + ", ";
                            }
                            else
                            {
                                values += "'" + cell.Value + "', ";
                            }
                        }
                        values = values.Remove(values.Length - 2);
                        values += ")";
                        command += table + " VALUES " + values;
                        var cmd = new SqlCommand(command, cnn);
                        //addValues(ref cmd, form.dataGridView1.Rows[0].Cells);
                        cmd.ExecuteNonQuery();
                        LoadTree();
                    }
                }
                catch
                {
                    MessageBox.Show("Данные были введены некорректно", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //Data Source=DESKTOP-P8ARIFA\SQLEXPRESS;Initial Catalog=KPO_Lab4;Integrated Security=True
    }
}
