using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;

namespace ado.net_project
{
    public partial class Form1 : Form
    {
        string conString = @"Data Source = COMP505\SQLEXPRESS; Initial Catalog = Shop; Integrated Security = true;";//ConfigurationManager.ConnectionStrings["my"].ConnectionString;

        SqlConnection conn = null;

        public Form1()
        {
            InitializeComponent();

            conn = new SqlConnection(conString);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SqlCommand com = null;
            SqlTransaction tran = null;
            SqlDataReader reader = null;

            try
            {
                conn.Open();
                tran = conn.BeginTransaction();
                com = conn.CreateCommand();
                com.Transaction = tran;

                com.CommandText = "select name from [Customers]";
                reader = com.ExecuteReader();

                while (reader.Read())
                {
                    comboBox1.Items.Add(reader[0]);
                }

                reader.Close();


                com.CommandText = "select name from [Sellers]";
                reader = com.ExecuteReader();


                while (reader.Read())
                {
                    comboBox2.Items.Add(reader[0]);
                }


                reader.Close();
                tran.Commit();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                tran.Rollback();
            }
            finally
            {
                conn?.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlCommand com = null;
            SqlTransaction tran = null;


            listBox1.Items.Clear();

            try
            {
                conn.Open();
                tran = conn.BeginTransaction();
                com = conn.CreateCommand();
                com.Transaction = tran;


                com.CommandText = "update [Customers] set [Count] = [Count] + @count where name = @cus_name";
                com.Parameters.AddWithValue("@count", textBox3.Text);
                com.Parameters.AddWithValue("@cus_name", comboBox1.Items[comboBox1.SelectedIndex].ToString());
                com.ExecuteNonQuery();

                com.CommandText = "update [Sellers] set [Count] = [Count] + @count where name = @sel_name";
                com.Parameters.AddWithValue("@sel_name", comboBox2.Items[comboBox1.SelectedIndex].ToString());
                com.ExecuteNonQuery();


                listBox1.Items.Add("Date of sale: " + DateTime.Now);
                listBox1.Items.Add("Customer: " + comboBox1.Items[comboBox1.SelectedIndex].ToString());
                listBox1.Items.Add("Seller: " + comboBox2.Items[comboBox1.SelectedIndex].ToString());
                listBox1.Items.Add("Count: " + textBox3.Text);


                //reader.Close();
                tran.Commit();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                tran.Rollback();
            }
            finally
            {
                conn?.Close();
            }
        }
    }
}
