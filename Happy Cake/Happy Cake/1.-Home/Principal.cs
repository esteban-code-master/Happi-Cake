using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace Happy_Cake
{
    public partial class Principal : Form
    {           
        string impresora = null, impresora_seleccionada = null;
        double totalVenta = 0;
        public Principal()
        {
            InitializeComponent();
            AbrirFormHija(new Abrir_Corte());
        }        
        private void button1_Click(object sender, EventArgs e) // boton pasteles
        {
            Pasteles ventana_pasteles = new Pasteles();
            AbrirFormHija(ventana_pasteles);
            ventana_pasteles.recibir_info += new Pasteles.capturar_articulo(lista_articulo);
        }
        private void button6_Click(object sender, EventArgs e) // boton velas
        {
            Velas ventana_velas = new Velas();            
            AbrirFormHija(ventana_velas);
            ventana_velas.recibir_info += new Velas.capturar_articulo(lista_articulo);
        }       
        private void button7_Click(object sender, EventArgs e)  // boton galletas
        {
            Galletas ventana_galletas = new  Galletas();
            AbrirFormHija(ventana_galletas);
            ventana_galletas.recibir_info += new Galletas.capturar_articulo(lista_articulo);
        }
        private void button8_Click(object sender, EventArgs e) // boton de promociones 3 x 2
        {
            Cobros_Especiales ventana_cobros = new Cobros_Especiales();
            AbrirFormHija(ventana_cobros);
            ventana_cobros.recibir_info += new Cobros_Especiales.capturar_articulo(lista_articulo);
        }
        private void btnabrircorte_Click(object sender, EventArgs e)
        {
            AbrirFormHija(new Abrir_Corte());
        }
        private void button18_Click(object sender, EventArgs e)
        {
            AbrirFormHija(new Cerrar_Corte());
        }
        private void button19_Click(object sender, EventArgs e)
        {
            AbrirFormHija(new Historial());
        }
        private void button20_Click(object sender, EventArgs e)
        {
            AbrirFormHija(new Productos());
        }
        private void button5_Click(object sender, EventArgs e)
        {
            AbrirFormHija(new Ventana_Tamaño());           
        }       
        private void button10_Click(object sender, EventArgs e) // entras , salidas y pedidos especiales
        {
            Entradas_Salidas notas = new Entradas_Salidas();
            AbrirFormHija(notas);
            notas.enviar += new Entradas_Salidas.pasar(Pedidoespecial);
        }       
        private void button13_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void button14_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }       
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)  // pago combinados
        {
            validador.solo_numeros(e);
            if (e.KeyChar==32)
            {
                Pago_combinado pago_Combinado = new Pago_combinado(totalVenta, tabla);
                pago_Combinado.StartPosition = FormStartPosition.CenterScreen;
                pago_Combinado.ShowDialog();
                textBox1.Text = null;
            }
            else if (e.KeyChar == 13)
            {
                button15_Click(sender, e); //pago con efectivo
            }
        }        
        private void button16_Click(object sender, EventArgs e) //pago en tarjeta
        {
            if (ValidateChildren(ValidationConstraints.Enabled)) {
                if (tabla.Rows.Count > 0)
                {
                    if (Convert.ToInt32(textBox1.Text.Trim()) >= totalVenta)
                    {
                        operaciones operaciones = new operaciones(0, Convert.ToInt32(textBox1.Text),tabla,totalVenta);
                        operaciones.Realizar_venta();
                        textBox1.Text = null;
                    }
                    else MessageBox.Show("La cantidad ingresada es menor al total de la compra \n porfavor verifica", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else MessageBox.Show("No hay ningun producto cargado en la lista");
            }
        }        
        private void button15_Click(object sender, EventArgs e) //pago con efectivo
        {
            if (ValidateChildren(ValidationConstraints.Enabled)) {
                if (tabla.Rows.Count > 0)
                {
                    if (Convert.ToInt32(textBox1.Text.Trim()) >= totalVenta)
                    {
                        operaciones operaciones = new operaciones(Convert.ToInt32(textBox1.Text),0,tabla, totalVenta);
                        operaciones.Realizar_venta();
                        textBox1.Text = null;
                    }
                    else MessageBox.Show("La cantidad ingresada es menor al total de la compra \n porfavor verifica", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else MessageBox.Show("No hay ningun producto cargado en la lista");
            }
        }   
        private void Button11_Click(object sender, EventArgs e)  // boton abrir cajon
        {
            CrearTicket crearTicket = new CrearTicket();
            crearTicket.AbreCajon();
            //  crearTicket.CortaTicket();  
            select_impresora();
            crearTicket.ImprimirTicket(impresora);
        }
        private void Tabla_KeyPress(object sender, KeyPressEventArgs e)
        {
            int indice;
            if (tabla.Rows.Count > 0)
            {
                if (e.KeyChar == 27)
                {
                    indice = tabla.CurrentRow.Index;
                    tabla.Rows.RemoveAt(indice);
                }
            }
        } // borrar elementos seleccionados de la tabla
        private void TextBox1_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                cantidad_entrante_vacia.SetError(textBox1, "campo vacio");
                textBox1.Text = "0";
            }
            else
            {
                cantidad_entrante_vacia.SetError(textBox1, null);
            }
        }        
        private void Tabla_CellValueChanged_1(object sender, DataGridViewCellEventArgs e)  // suma del total de la venta automaticamnete
        {
            double cantidad_entrante = 0;
            double resultado;
            try
            {
                for (int i = 0; i < tabla.Rows.Count; i++)
                {
                    resultado = Convert.ToDouble(tabla[2, i].Value) * Convert.ToDouble(tabla[0, i].Value);
                    cantidad_entrante += resultado;
                }
                totalVenta = cantidad_entrante;
                label3.Text = cantidad_entrante.ToString();
                cantidad_entrante = 0; resultado = 0;
            }
            catch
            {
                MessageBox.Show("Contate con el desarrollador ", "Algo ha salido mal", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        private void Tabla_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            double cantidad_entrante = 0;
            try
            {
                for (int i = 0; i < tabla.Rows.Count; i++)
                {
                     cantidad_entrante += Convert.ToDouble(tabla[2, i].Value);
                }
                totalVenta = cantidad_entrante;
                label3.Text = cantidad_entrante.ToString();
                cantidad_entrante = 0;
            }
            catch
            {
                MessageBox.Show("Contate con el desarrollador ", "Algo ha salido mal", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        } // suma del total de la venta despues de eliminar algun elemento de la tabla        
        private void button9_Click(object sender, EventArgs e) // reimprime el ultimo ticket
        {
            select_impresora();
            Reeimprimirtiket();
        }      
        private void button2_Click(object sender, EventArgs e)  // boton configuraciones
        {
            configuraciones configuraciones = new configuraciones();
            configuraciones.StartPosition = FormStartPosition.CenterParent;
            configuraciones.ShowDialog();
        }   
        private void AbrirFormHija(object formhija)
        {
            if (this.PanelContenedor.Controls.Count > 0)
                this.PanelContenedor.Controls.RemoveAt(0);
            Form fh = formhija as Form;
            fh.TopLevel = false;
            fh.Dock = DockStyle.Fill;
            this.PanelContenedor.Controls.Add(fh);
            this.PanelContenedor.Tag = fh;
            fh.Show();
        }       
        public void lista_articulo(string id_articulo,bool promo)
        {
            // obteniendo datos del articulo para pasarlos al datagridview y al ticket 
            string nombre_producto = null;
            double precio_producto = 0;  
            string buscar_articulo = "SELECT nombre,precio FROM articulos a,producto p WHERE p.id_producto = a.id_producto AND id_articulo='" + id_articulo + "'";
            using (MySqlConnection conexion = new MySqlConnection(ConfigurationManager.ConnectionStrings["conexionGeneral"].ConnectionString))
            {
                conexion.Open();
                using (MySqlCommand consulta = new MySqlCommand(buscar_articulo,conexion))
                {
                    using (MySqlDataReader leer = consulta.ExecuteReader())
                    {
                        while (leer.Read())
                        {
                            nombre_producto = leer[0].ToString();
                            precio_producto = Convert.ToDouble(leer[1]);
                        }
                    }
                }
            }
            // funcion para comprobar si el articulo entrante ya esta en al lista y simplemente sumarlo
            agregar_lista(id_articulo, tabla, nombre_producto, precio_producto,promo);
        }
        public void agregar_lista(String id_articulo, DataGridView tabla, string nombre, double precio, bool promo)
        {
            if (promo == false)  // determina si el si esta activa la promocion de ventas
            {
                bool bandera = true;
                int actualizar = 0;
                for (int i = 0; i < tabla.Rows.Count; i++)
                {
                    String verificar = Convert.ToString(tabla[3, i].Value);
                    string presion_articulo = tabla[2, i].Value.ToString();
                    if (id_articulo == verificar && presion_articulo != "0")
                    {
                        bandera = false;
                        actualizar = i;
                        break;
                    }
                }
                if (bandera)
                {
                    tabla.Rows.Add();
                    int i = tabla.Rows.Count - 1;
                    tabla[0, i].Value = 1;
                    tabla[1, i].Value = nombre;
                    tabla[2, i].Value = precio;
                    tabla[3, i].Value = id_articulo;
                }
                else
                {
                    tabla[0, actualizar].Value = Convert.ToInt32(tabla[0, actualizar].Value) + 1;
                }
            }
            else 
            {
                tabla.Rows.Add();
                int i = tabla.Rows.Count - 1;
                tabla[0, i].Value = 1;
                tabla[1, i].Value = nombre;
                tabla[2, i].Value = 0;  // por que es una promo vale 0
                tabla[3, i].Value = id_articulo;
            }
        }
        public void Pedidoespecial(string descripcion, string precio, string id_articulo) // numrepeticion para reimprimir dos veces el ticket
        {
            tabla.Rows.Add();
            int filas = tabla.Rows.Count - 1;
            tabla[0, filas].Value = 1;
            tabla[1, filas].Value = descripcion;
            tabla[2, filas].Value = precio;
            tabla[3, filas].Value = id_articulo;
        }
        public void Reeimprimirtiket()
        {
            string cajero = null, noTicket = null, fecha = null, id_session = null;
            string busca_cajero = "SELECT nombre FROM logeo  ORDER BY id_sesion DESC limit 1";
            string busca_noticket = "SELECT folio_venta,fyh,id_sesion FROM venta  ORDER BY folio_venta DESC limit 1";
            double total = 0, efectivo = 0, tarjeta = 0, cambio = 0;
            List<string> articulo = new List<string>();
            List<int> cantarticulo = new List<int>();
            List<double> subtotal = new List<double>();
            int cantidad = 0;
            using (MySqlConnection conexion = new MySqlConnection(ConfigurationManager.ConnectionStrings["conexionGeneral"].ConnectionString))
            {
                conexion.Open();
                using (MySqlCommand consulta = new MySqlCommand())
                {
                    consulta.CommandText = busca_noticket;
                    consulta.Connection = conexion;
                    using (MySqlDataReader leer_ticket = consulta.ExecuteReader())
                    {
                        while (leer_ticket.Read())
                        {
                            noTicket = leer_ticket[0].ToString();
                            fecha = leer_ticket[1].ToString();
                            id_session = leer_ticket[2].ToString();
                        }
                    }
                    consulta.CommandText = busca_cajero;
                    consulta.Connection = conexion;
                    using (MySqlDataReader leer_cajero = consulta.ExecuteReader())
                    {
                        while (leer_cajero.Read())
                        {
                            cajero = leer_cajero[0].ToString();
                        }
                    }
                    string buscar_pagos = "SELECT totalVenta,efectivoEntro,tarjeta FROM detalles_pagos WHERE folio_venta='" + noTicket + "'";
                    string buscar_articulos = "SELECT nombre,categoria,cantidad,subtotal FROM detalle_venta d,articulos a, producto p,categoria c WHERE d.id_articulo = a.id_articulo  AND a.id_producto = p.id_producto AND c.id_cat=a.id_cat AND folio_venta = '" + noTicket + "'";
                    consulta.CommandText = buscar_pagos;
                    consulta.Connection = conexion;
                    using (MySqlDataReader leer_pagos = consulta.ExecuteReader())
                    {
                        while (leer_pagos.Read())
                        {
                            total = Convert.ToDouble(leer_pagos[0]);
                            efectivo = Convert.ToDouble(leer_pagos[1]);
                            tarjeta = Convert.ToDouble(leer_pagos[2]);
                        }
                        cambio = (efectivo + tarjeta) - total;
                    }
                    consulta.CommandText = buscar_articulos;
                    consulta.Connection = conexion;
                    using (MySqlDataReader leer_articulos = consulta.ExecuteReader())
                    {
                        while (leer_articulos.Read())
                        {
                            articulo.Add(leer_articulos[0].ToString());
                            cantarticulo.Add(Convert.ToInt32(leer_articulos[2].ToString()));
                            subtotal.Add(Convert.ToDouble(leer_articulos[3].ToString()));
                        }
                    }
                }
            }
            datos_ticket(noTicket,cajero,fecha,articulo,cantarticulo,subtotal,efectivo,tarjeta,cambio,cantidad,total);
        }
        public void datos_ticket(string noTicket,string cajero,string fecha,List<string>articulo,List<int> cantarticulo,List<double>subtotal,double efectivo, double tarjeta,double cambio,int cantidad,double total)
        {            
            CrearTicket ticket = new CrearTicket();
            ticket.AbreCajon();
            ticket.TextoCentro("HAPPY CAKE PASTELERIAS");
            ticket.TextoIzquierda("AV. DE LA LUNA SM. 504");
            ticket.TextoIzquierda("TEL: 99891493900");
            ticket.TextoIzquierda("R.F.C: LOFN730107DD1");
            ticket.TextoIzquierda("Caja # 1 Ticket #" + noTicket);
            ticket.lineasAsteriscos();
            ticket.TextoIzquierda("Cajero:" + cajero);
            ticket.TextoIzquierda("Fecha:" + fecha);
            ticket.lineasAsteriscos();
            ticket.EncabezadoVenta();   //NOMBRE DEL ARTICULO, CANT, PRECIO
            ticket.lineasAsteriscos();
            // generacion de articulos 
            for (int i = 0; i < articulo.Count; i++)
            {            // el 0 es del nombre 1 cantidad 2 precio
                ticket.AgregaArticulo(articulo[i], cantarticulo[i], subtotal[i]);
                cantidad += cantarticulo[i];
            }
             ticket.lineasIgual();
             //  precios
             ticket.AgregarTotales("TOTAL.........$", total);
             ticket.TextoIzquierda("");
             ticket.AgregarTotales("EFECTIVO......$", efectivo);
             ticket.AgregarTotales("TARJETA.......$", tarjeta);
             ticket.AgregarTotales("CAMBIO........$", cambio);
              //texto final
             ticket.TextoIzquierda("");
             ticket.TextoIzquierda("ARTÍCULOS VENDIDOS:" + cantidad);
             ticket.TextoIzquierda("");
             ticket.TextoIzquierda("");
             ticket.TextoIzquierda("");
             ticket.TextoIzquierda("");
             ticket.TextoIzquierda("");
             ticket.TextoIzquierda("");
             ticket.CortaTicket();
             ticket.ImprimirTicket(impresora); //Nombre de la impresora ticketera         
        }

        public void select_impresora()
        {
            impresora_seleccionada = "SELECT * FROM impresora WHERE estatus='1'";
            using (MySqlConnection conexion = new MySqlConnection(ConfigurationManager.ConnectionStrings["conexionImpresora"].ConnectionString))
            {
                conexion.Open();
                using (MySqlCommand consulta = new MySqlCommand(impresora_seleccionada,conexion))
                {
                    using (MySqlDataReader leer_impresora = consulta.ExecuteReader())
                    {
                        while (leer_impresora.Read())
                        {
                            impresora = leer_impresora[0].ToString();
                        }
                    }
                }
            }
        }
    }
}
