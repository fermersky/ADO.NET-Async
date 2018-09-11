using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace async_ado_app
{
    public partial class Form1 : Form
    {
        SqlConnection conn = null;
        SqlConnection conn1 = null;
        string cs = @"Data Source = COMP505\SQLEXPRESS; Initial Catalog = PublishingHouse; Integrated Security = true;";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            const string AsyncEnabled = "Asynchronous Processing = true;";

            if (!cs.Contains(AsyncEnabled))
                cs = $"{cs} {AsyncEnabled}";

            conn = new SqlConnection(cs);
            conn1 = new SqlConnection(cs);

            SqlCommand com = conn.CreateCommand();
            com.CommandText = "SELECT * FROM book.Books; SELECT sum(len(NameBook)) as length FROM book.Books;";
            com.CommandTimeout = 30;

            SqlCommand com1 = conn1.CreateCommand();
            com1.CommandText = "SELECT * FROM book.Authors; SELECT sum(len(FirstName + LastName)) as length FROM book.Authors;";
            com1.CommandTimeout = 30;

            try
            {
                conn.Open();
                conn1.Open();

                // #

                AsyncCallback call = new AsyncCallback(ExecQueryOne);
                com.BeginExecuteReader(call, com);

                //MessageBox.Show("Test");

                AsyncCallback call1 = new AsyncCallback(ExecQueryTwo);
                com1.BeginExecuteReader(call1, com1); 



                // #
            }

            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void ExecQueryOne(IAsyncResult ar)
        {
            //Thread.Sleep(3000);
            try
            {
                SqlCommand c = ar.AsyncState as SqlCommand;
                SqlDataReader r = c.EndExecuteReader(ar);
                DataTable table = new DataTable();

                int line = 0;
                int c_count = 0;
                string str = "";

                do
                {
                    if (c_count == 0)
                    {
                        while (r.Read())
                        {
                            if (line == 0) // column names
                                for (int i = 0; i < r.FieldCount; i++)
                                    table.Columns.Add(r.GetName(i));
                            line++;

                            DataRow row = table.NewRow();

                            for (int i = 0; i < r.FieldCount; i++) // rows
                                row[i] = r[i];
                            table.Rows.Add(row);
                        }
                    }
                    else
                    {
                        r.Read();
                        str = r[0].ToString();
                    }
                    c_count++;
                } while (r.NextResult());




                dataGridView1.Invoke(new Action(() => {
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = table;
                }));

                label2.Invoke(new Action(() => {
                    label2.Text = str;
                }));

            }

            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void ExecQueryTwo(IAsyncResult ar)
        {
            //Thread.Sleep(3000);
            try
            {
                SqlCommand c = ar.AsyncState as SqlCommand;
                SqlDataReader r = c.EndExecuteReader(ar);
                DataTable table = new DataTable();

                int line = 0;
                int c_count = 0;
                string str = "";

                do
                {
                    if (c_count == 0)
                    {
                        while (r.Read())
                        {
                            if (line == 0) // column names
                                for (int i = 0; i < r.FieldCount; i++)
                                    table.Columns.Add(r.GetName(i));
                            line++;

                            DataRow row = table.NewRow();

                            for (int i = 0; i < r.FieldCount; i++) // rows
                                row[i] = r[i];
                            table.Rows.Add(row);
                        }
                    }
                    else
                    {
                        r.Read();
                        str = r[0].ToString();
                    }
                    c_count++;
                } while (r.NextResult());




                dataGridView2.Invoke(new Action(() => {
                    dataGridView2.DataSource = null;
                    dataGridView2.DataSource = table;
                }));

                label2.Invoke(new Action(() => {
                    label4.Text = str;
                }));

            }

            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}
