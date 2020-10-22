using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Happy_Cake
{
    public class Botones_dinamic
    {
        funcionesSQL funciones = new funcionesSQL();
        Ventana_Tamaño ventana_tamaño = new Ventana_Tamaño();
        FlowLayoutPanel panel_responsivo;
        string cadena = null;
        bool promo;  
        public delegate void capturar_articulo(string id_articulo,bool promo);
        public event capturar_articulo recibir_info;
       
        public Botones_dinamic(FlowLayoutPanel panel_responsivo, string cadena , bool promo)
        {
            this.panel_responsivo = panel_responsivo;
            this.cadena = cadena;
            this.promo = promo;
        } 
        public void botones_dinamicamente(int [] color)
        {         
            List<string> prod = new List<string>();
            List<string> id_producto = new List<string>();
            List<string> clase = new List<string>();
            string productos = cadena;
            using (MySqlConnection conexion = new MySqlConnection(ConfigurationManager.ConnectionStrings["conexionGeneral"].ConnectionString))
            {
                conexion.Open();
                using (MySqlCommand consulta = new MySqlCommand(productos,conexion))
                {
                    using (MySqlDataReader leer_productos = consulta.ExecuteReader())
                    {
                        while (leer_productos.Read())
                        {
                            prod.Add(leer_productos[0].ToString());
                            id_producto.Add(leer_productos[1].ToString());
                            clase.Add(leer_productos[2].ToString());
                        }
                    }
                }
            }
            for (int i = 0; i < prod.Count; i++)
            {
                Button boton = new Button();
                boton.Name = id_producto[i]; // el nombre de los botones es el id_producto para poder realizar las combinaciones entre tamaños
                boton.Size = new Size(195, 145);
                boton.Text = prod[i];
                boton.Font = new Font(FontFamily.GenericSansSerif, 12.0F, FontStyle.Bold);
                boton.BackColor = Color.FromArgb(color[0], color[1], color[2]);
                if (clase[i] == "4" || clase[i] == "5") { boton.BackColor = Color.FromArgb(108, 12, 147); } // solo para cambiarle el color a los producto de categoria cafe
                boton.FlatStyle = FlatStyle.Flat;
                boton.ForeColor = Color.Black;
                boton.FlatAppearance.BorderSize = 0;
                boton.Click += new System.EventHandler(abrir);
                panel_responsivo.Controls.Add(boton);
            }
        }
        private void abrir(object sender, System.EventArgs e)
        {
            Button botonManual = (Button)sender;    // obtener el nombre del boton que ejecuta el evento
            ventana_tamaño = new Ventana_Tamaño(botonManual.Name , promo); //  
            panel_responsivo.Size = new Size(800, 502);   //tamaño al abrir el formulario
            ventana_tamaño.StartPosition = FormStartPosition.Manual;    // forzar el cambio de posicion
            ventana_tamaño.Location = new Point(900, 140);
            ventana_tamaño.recibir_articulo += new Ventana_Tamaño.enviar_articulo(recibir_articulo); //iniciar el delegado
            ventana_tamaño.ShowDialog();
            panel_responsivo.Size = new Size(872, 502);// regresar a sus tamaño normal
        }
        public void recibir_articulo(string id_articulo,bool promo)
        {
            recibir_info(id_articulo,promo);
        }
    }
}
