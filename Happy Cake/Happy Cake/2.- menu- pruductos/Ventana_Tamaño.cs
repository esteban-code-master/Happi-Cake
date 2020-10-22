using System;
using System.Collections.Generic;
using System.Drawing;
using System.Configuration;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Happy_Cake
{
    public partial class Ventana_Tamaño : Form
    {
        string id_producto = null;
        bool promo_activada;
        List<string> articulos = new List<string>();
        List<string> categoria = new List<string>();
        public delegate void enviar_articulo(string id_articulo,bool promo);
        public event enviar_articulo recibir_articulo;
        enum clase
        {
           Grande=1,Mediano=2,Chico=3,Individual=4,Rebanda=5,Pieza=6
        }
        public Ventana_Tamaño(string id_producto , bool promo)
        {
            this.id_producto = id_producto;
            this.promo_activada = promo;
            InitializeComponent();            
            seleccionar_articulo();
        }
        public Ventana_Tamaño()
        {
            InitializeComponent();
        }

        public void seleccionar_articulo()
        {
            string buscar_articulos;
            if(promo_activada == false ) buscar_articulos = "SELECT id_articulo,id_cat FROM articulos WHERE id_producto='" + id_producto + "'";            
            else buscar_articulos = "SELECT id_articulo,id_cat FROM articulos WHERE id_producto='" + id_producto + "' and id_cat between 4 and 5";
            using (MySqlConnection conexion = new MySqlConnection(ConfigurationManager.ConnectionStrings["conexionGeneral"].ConnectionString))
            {
                conexion.Open();
                using (MySqlCommand consulta = new MySqlCommand(buscar_articulos,conexion)) 
                {
                    using (MySqlDataReader leer_articulos = consulta.ExecuteReader())
                    {
                        while (leer_articulos.Read())
                        {
                            articulos.Add(leer_articulos[0].ToString());
                            categoria.Add(leer_articulos[1].ToString());
                        }
                    }
                }
            }
        }
        private void Tamaño_Load(object sender, EventArgs e)
        {     
            bloquear(b1);
            bloquear(b2);
            bloquear(b3);
            bloquear(b4);
            bloquear(b5);
            bloquear(b6);
        }
        public void bloquear(Button boton)
        {
            boton.Enabled = false;
            boton.BackColor = Color.FromArgb(199, 199, 199);
            for (int i = 0; i < categoria.Count; i++)
            {
                if (boton.Name == "b" + categoria[i])
                {
                    boton.Enabled = true;
                    boton.BackColor = Color.FromArgb(51, 67, 115);
                    boton.Cursor = Cursors.Hand;
                }               
            }
        }
        public void busqueda(int id_categoria,bool promo)
        {           
            // buscando el articulo para enviarlo a la ventana principal
            string buscar_articulo = "SELECT id_articulo FROM articulos  WHERE id_producto = '" + id_producto + "' and id_cat = '" + id_categoria + "'";           
            using (MySqlConnection conexion = new MySqlConnection(ConfigurationManager.ConnectionStrings["conexionGeneral"].ConnectionString))
            {
                conexion.Open();
                using (MySqlCommand consulta = new MySqlCommand(buscar_articulo, conexion))
                {
                    using (MySqlDataReader leer_articulo = consulta.ExecuteReader())
                    {
                        while (leer_articulo.Read())
                        {
                            recibir_articulo(leer_articulo[0].ToString(), promo);
                        }
                    }
                }
            }
            this.Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            busqueda((int)clase.Grande, false);
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            busqueda((int)clase.Mediano, false);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            busqueda((int)clase.Chico, false);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            busqueda((int)clase.Individual,promo_activada);  // articulo que se aplia en la promocion
        }
        private void button5_Click(object sender, EventArgs e)
        {
            busqueda((int)clase.Rebanda, promo_activada);     // articulo que se aplia en la promocion
        }
        private void b6_Click(object sender, EventArgs e)
        {
            busqueda((int)clase.Pieza,false);
        }       
        private void button7_Click(object sender, EventArgs e)
        {
            this.Close();
        }       
    }
}
