using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Happy_Cake
{
    public partial class Nuevoproducto : Form
    {      
        List<string> tamaños = new List<string>();
        List<double> precios_art = new List<double>();
        List<string> id_articulo = new List<string>();
        public Nuevoproducto()
        {
            InitializeComponent();
        }
        private void NuevoEmpleado_Load(object sender, EventArgs e)
        {
            button3.Enabled = false;            
        }       
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void textBox3_KeyPress(object sender, KeyPressEventArgs e) // buscador
        {
            validador.solo_numeros(e);
            string cadena = "SELECT a.id_producto,nombre,categoria,precio,a.id_cat,a.id_articulo FROM producto p,articulos a,categoria c WHERE  a.id_cat=c.id_cat AND a.id_producto=p.id_producto AND a.id_producto='" + textBox3.Text + "'";
            if (e.KeyChar==13)
            {
                if (textBox3.Text.Trim() !="")
                {
                    using (MySqlConnection conexion = new MySqlConnection(ConfigurationManager.ConnectionStrings["conexionGeneral"].ConnectionString))
                    {
                        conexion.Open();
                        using (MySqlCommand consulta = new MySqlCommand(cadena,conexion))
                        {
                            using (MySqlDataReader leer = consulta.ExecuteReader())
                            {
                                while (leer.Read())
                                {
                                    textBox1.Text = leer[1].ToString(); // nombre
                                    textBox4.Text = leer[0].ToString();
                                    tamaños.Add(leer[2].ToString());
                                    id_articulo.Add(leer[5].ToString());
                                    precios_art.Add(Convert.ToDouble(leer[3]));
                                    button3.Enabled = true;
                                    button1.Enabled = false;
                                }
                            }
                        }
                    }
                    
                    bloquear(Grande,precios_art,textBox2,id_articulo,label13);
                    bloquear(Mediano,precios_art,textBox5,id_articulo,label12);
                    bloquear(Chico,precios_art,textBox6,id_articulo,label11);
                    bloquear(Individual,precios_art,textBox7,id_articulo,label10);
                    bloquear(Rebanada,precios_art,textBox9,id_articulo,label9);
                    bloquear(Pieza,precios_art,textBox10,id_articulo,label8);
                    textBox3.Text = null;
                }
            }
        }
        public void bloquear(CheckBox chec, List<double> precio,TextBox text, List<string> id_articulo,Label id)
        {
            chec.Enabled = true;
            for (int i = 0; i < tamaños.Count; i++)
            {
                if (chec.Name == tamaños[i])
                {
                    text.Text = precio[i].ToString();
                    chec.Enabled = false;
                    chec.Cursor = Cursors.Hand;
                    id.Text = id_articulo[i];
                }
            }
        }
        public void articulos(string id_producto,TextBox precio,int tamaño,MySqlCommand ingresar_articulo ,MySqlConnection conexion,MySqlTransaction transaction)
        {
            string nuevo_articulo = "INSERT INTO articulos(id_producto,id_cat,precio) VALUES((" + id_producto + "),'" + tamaño + "','" + precio.Text + "')";
            ingresar_articulo.CommandText = nuevo_articulo;        
            ingresar_articulo.Connection = conexion;
            ingresar_articulo.Transaction = transaction;
            ingresar_articulo.ExecuteNonQuery();
        }
        public void actualizar_articulos(TextBox precio,MySqlCommand actualizar_art,MySqlConnection conexion, MySqlTransaction transaction,Label ID)
        {
            string actualiza = "UPDATE articulos SET precio='" + precio.Text + "' WHERE id_articulo='" + ID.Text + "'";
            actualizar_art.CommandText = actualiza;           
            actualizar_art.Connection = conexion;
            actualizar_art.Transaction = transaction;
            actualizar_art.ExecuteNonQuery();
        }       
        private void button1_Click(object sender, EventArgs e)  // guardar
        {
            using (MySqlConnection conexion = new MySqlConnection(ConfigurationManager.ConnectionStrings["conexionGeneral"].ConnectionString))
            {
                conexion.Open();
                using (MySqlCommand ingresar_producto = new MySqlCommand())
                {
                    using (MySqlTransaction transaction = conexion.BeginTransaction())
                    {
                        try
                        {
                            if (Grande.Checked || Mediano.Checked || Chico.Checked || Individual.Checked || Rebanada.Checked || Pieza.Checked)
                            {
                                string nuevo_producto = "INSERT INTO producto(nombre,clase) VALUES('" + textBox1.Text + "',(SELECT id_clase FROM clasificacion WHERE descripcion='" + comboBox1.Text + "'))";
                                ingresar_producto.CommandText = nuevo_producto;
                                ingresar_producto.Connection = conexion;
                                ingresar_producto.Transaction = transaction;
                                ingresar_producto.ExecuteNonQuery();
                                string id_prod = "(SELECT id_producto FROM producto  ORDER BY id_producto DESC limit 1)";
                                if (Grande.Checked) articulos(id_prod, textBox2, 1, ingresar_producto, conexion, transaction);
                                if (Mediano.Checked) articulos(id_prod, textBox5, 2, ingresar_producto, conexion, transaction);
                                if (Chico.Checked) articulos(id_prod, textBox6, 3, ingresar_producto, conexion, transaction);
                                if (Individual.Checked) articulos(id_prod, textBox7, 4, ingresar_producto, conexion, transaction);
                                if (Rebanada.Checked) articulos(id_prod, textBox9, 5, ingresar_producto, conexion, transaction);
                                if (Pieza.Checked) articulos(id_prod, textBox10, 6, ingresar_producto, conexion, transaction);
                                transaction.Commit();
                                MessageBox.Show("Producto ingresado correctamente");
                                textBox1.Text = null; textBox2.Text = null; textBox5.Text = null; textBox6.Text = null; textBox7.Text = null; textBox9.Text = null; textBox10.Text = null;
                            }
                            else MessageBox.Show("no ha registrado tamaño de producto");
                        }
                        catch
                        {
                            MessageBox.Show("error al ingresar el producto intente de nuevo", "");
                            transaction.Rollback();
                        }
                    }
                }
            }
        }        
        private void button3_Click(object sender, EventArgs e)  // actualizar
        {
            using (MySqlConnection conexion = new MySqlConnection(ConfigurationManager.ConnectionStrings["conexionGeneral"].ConnectionString))
            {
                conexion.Open();
                using (MySqlCommand actualizar_producto = new MySqlCommand())
                {
                    using (MySqlTransaction transaction = conexion.BeginTransaction())
                    {
                        try
                        {
                            string actualizar = "UPDATE producto SET nombre='" + textBox1.Text + "' WHERE id_producto='" + textBox4.Text + "'";
                            actualizar_producto.CommandText = actualizar;
                            actualizar_producto.Connection = conexion;
                            actualizar_producto.Transaction = transaction;
                            actualizar_producto.ExecuteNonQuery();
                            if (label13.Text != "none") actualizar_articulos(textBox2, actualizar_producto, conexion, transaction, label13);
                            if (label12.Text != "none") actualizar_articulos(textBox5, actualizar_producto, conexion, transaction, label12);
                            if (label11.Text != "none") actualizar_articulos(textBox6, actualizar_producto, conexion, transaction, label11);
                            if (label10.Text != "none") actualizar_articulos(textBox7, actualizar_producto, conexion, transaction, label10);
                            if (label9.Text != "none") actualizar_articulos(textBox9, actualizar_producto, conexion, transaction, label9);
                            if (label8.Text != "none") actualizar_articulos(textBox10, actualizar_producto, conexion, transaction, label8);
                            /*---agregar nuevo articulo si es seleccionado --*/
                            if (Grande.Checked) articulos(textBox4.Text, textBox2, 1, actualizar_producto, conexion, transaction);
                            if (Mediano.Checked) articulos(textBox4.Text, textBox5, 2, actualizar_producto, conexion, transaction);
                            if (Chico.Checked) articulos(textBox4.Text, textBox6, 3, actualizar_producto, conexion, transaction);
                            if (Individual.Checked) articulos(textBox4.Text, textBox7, 4, actualizar_producto, conexion, transaction);
                            if (Rebanada.Checked) articulos(textBox4.Text, textBox9, 5, actualizar_producto, conexion, transaction);
                            if (Pieza.Checked) articulos(textBox4.Text, textBox10, 6, actualizar_producto, conexion, transaction);
                            transaction.Commit();
                            MessageBox.Show("Producto actualizado correctamente");
                            textBox1.Text = null; textBox2.Text = null; textBox5.Text = null; textBox6.Text = null; textBox7.Text = null; textBox9.Text = null; textBox10.Text = null; textBox4.Text = null;
                            label13.Text = null; label12.Text = null; label11.Text = null; label10.Text = null; label9.Text = null; label8.Text = null;
                            button1.Enabled = true; button3.Enabled = false;
                            Grande.Enabled = true; Mediano.Enabled = true; Chico.Enabled = true; Individual.Enabled = true; Rebanada.Enabled = true; Pieza.Enabled = true;
                        }
                        catch
                        {
                            MessageBox.Show("error al actualizar el producto intente de nuevo");
                            transaction.Rollback();
                        }                       
                    }
                }
            }
        }
        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            validador.solo_numeros(e);
        }
    }
}
