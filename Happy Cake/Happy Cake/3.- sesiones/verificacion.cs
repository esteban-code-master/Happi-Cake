using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Happy_Cake
{
    public partial class verificacion : Form
    {
        string usuario = null, contra = null;
        public verificacion()
        {
            InitializeComponent();
        }
        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text=="") {
                textBox1.Text = "Ingrese usuario";
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                textBox2.Text = "Ingrese clave";
                textBox2.UseSystemPasswordChar = false;
            }
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == "Ingrese clave")
            {
                textBox2.Text = "";
                textBox2.UseSystemPasswordChar = true;
            }
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "Ingrese usuario")
            {
                textBox1.Text = "";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar ==13 )
            {
                button1_Click(sender,e);
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                button1_Click(sender, e);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string Identificar = "SELECT*FROM administrador";
            using (MySqlConnection conexion = new MySqlConnection(ConfigurationManager.ConnectionStrings["conexionGeneral"].ConnectionString))
            {
                conexion.Open();
                using (MySqlCommand consulta = new MySqlCommand(Identificar,conexion))
                {
                    using (MySqlDataReader buscar_usuario = consulta.ExecuteReader())
                    {
                        while (buscar_usuario.Read())
                        {
                            usuario = buscar_usuario[0].ToString();
                            contra = buscar_usuario[1].ToString();
                        }
                    }
                }
            }
            if (textBox1.Text==usuario && textBox2.Text==contra) {
                Nuevoproducto nuevoproducto = new Nuevoproducto();
                nuevoproducto.StartPosition = FormStartPosition.Manual;
                nuevoproducto.Location = new Point(213, 110);
                nuevoproducto.ShowDialog();
                this.Close();
            }
            else {
                MessageBox.Show("Usuario o contraseña incorrecto","Revise",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
        }
    }
}
