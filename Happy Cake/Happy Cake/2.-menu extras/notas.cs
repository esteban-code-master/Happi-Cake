using System;
using System.Windows.Forms;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace Happy_Cake
{
    public partial class Entradas_Salidas : Form
    {
        funcionesSQL funciones = new funcionesSQL();
        public delegate void pasar(string descripcion, string precio, string id_articulo);
        public event pasar enviar;
        public Entradas_Salidas()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)// ingresar entradas
        {
            if(textBox1.Text.Trim() !="" && textBox2.Text.Trim() != "")
            {
                DialogResult resultado = MessageBox.Show("Descripcion :"+textBox1.Text+"\n Cantidad $ "+textBox2.Text,"VERIFICAR", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (resultado == DialogResult.OK)
                {
                    string id = funciones.id_sesion();
                    string entradas = "INSERT INTO entrada (descripcion,cantidad,id_sesion) VALUES ('" + textBox1.Text + "','" + textBox2.Text + "','" + id + "');";
                    funciones.insertar(entradas);
                    textBox1.Text = null;
                    textBox2.Text = null;
                }   
            }
            else
            {
                MessageBox.Show("Campos vacios", "Intwnte de nuevo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e) // ingresar salidas
        {                      
            if (textBox1.Text.Trim() != "" && textBox2.Text.Trim() != "")
            {
                double venta = Verificar_efectivo();
                if (Convert.ToDouble(textBox2.Text) <= venta)
                {
                    DialogResult resultado = MessageBox.Show("Descripcion :" + textBox1.Text + " \nCantidad  $ " + textBox2.Text, "VERIFICAR", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    if (resultado == DialogResult.OK)
                    {
                        string id = funciones.id_sesion();

                        string entradas = "INSERT INTO salida (descripcion,cantidad,id_sesion) VALUES ('" + textBox1.Text + "','" + textBox2.Text + "','" + id + "');";
                        funciones.insertar(entradas);
                        textBox1.Text = null;
                        textBox2.Text = null;
                    }
                }
                else MessageBox.Show("No hay dinero disponible para realizar una salida de efectivo", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("Campos vacios", "Intente de nuevo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        } 
        private void button3_Click(object sender, EventArgs e)  // ingresar pedidos especiales
        {
            if (textBox1.Text.Trim() != "" && textBox2.Text.Trim() != "")
            {
                enviar(textBox1.Text, textBox2.Text, "272"); // 272 es el id articulo de las ventas especiales
                textBox1.Text = null;
                textBox2.Text = null;
            }
            else
            {
                MessageBox.Show("Campos vacios", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            validador.solo_numeros(e);
        }
        public double Verificar_efectivo()
        {
            string sesion_abierta = funciones.id_sesion();
            double venta = 0;
            string venta_sistema = "SELECT SUM(totalVenta) FROM detalles_pagos d,venta v WHERE d.folio_venta = v.folio_venta AND id_sesion ='" + sesion_abierta + "' GROUP by id_sesion";
            using (MySqlConnection conexion = new MySqlConnection(ConfigurationManager.ConnectionStrings["conexionGeneral"].ConnectionString))
            {
                conexion.Open();
                using (MySqlCommand consulta = new MySqlCommand(venta_sistema, conexion))
                {
                    using (MySqlDataReader leer = consulta.ExecuteReader())
                    {
                        while (leer.Read())
                        {
                            venta = Convert.ToDouble(leer[0]);
                        }
                    }
                }
            }
            return venta;
        }
       
    }
}
