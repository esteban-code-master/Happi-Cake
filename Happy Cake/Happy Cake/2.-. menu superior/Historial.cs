using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Happy_Cake
{
    public partial class Historial : Form
    {
        funcionesSQL funcionesSQL = new funcionesSQL();
        string impresora_seleccionada = null,impresora = null,notickeg, fechag, cajerog, totalg = "0", efectivog = "0", tarjetag = "0";
        int cambiog = 0,activo = 0;
        public Historial()
        {
            InitializeComponent();
            controles(false, false, 0, 0);
        }
        private void Historial_Load(object sender, EventArgs e)
        {
            button3_Click(sender, e);
        }
        public void controles(bool grupo1,bool grupo2,int x, int y)
        {
            // grupo 1 controles de las ventas grupo 2 es de entradas y salidas
            button6.Visible = grupo1;
            button6.Enabled = grupo1;
            button7.Enabled = grupo2;
            button7.Visible = grupo2;
            button5.Enabled = grupo2;           
            button5.Visible = grupo2;                      
            flowLayoutPanel2.Visible = grupo1;            
            flowLayoutPanel4.Visible = grupo2;       
            dataGridView1.Size = new Size(x,y);
            flowLayoutPanel2.Size = new Size(289, 444);
            flowLayoutPanel2.Location = new Point(552, 6);
            flowLayoutPanel4.Size = new Size(289, 444);
            flowLayoutPanel4.Location = new Point(552, 6);                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        
            button7.Location = new Point(786, 502);
            button6.Location = new Point(786, 502);
            button5.Location = new Point(786, 502);
        }        
        private void button2_Click(object sender, EventArgs e) // boton de ventas de dias anteriores
        {
            controles(false,false,829,429);
            string[] fecha_separada = calendario.Value.ToShortDateString().Split('/');
            string ventas_t = "SELECT DATE_FORMAT(fyh,'%Y/%M/%d') as fecha,SUM(totalVenta) as total,SUM(tarjeta) as tarjeta FROM detalles_pagos d, venta v WHERE d.folio_venta=v.folio_venta GROUP BY month(fyh),year(fyh),day(fyh)";
            funcionesSQL.llenartabla(dataGridView1, ventas_t);
        }           
        private void button3_Click(object sender, EventArgs e)  // compras
        {
            double total_venta=0;
            activo = 1;
            panel1.Visible = true;
            controles(true, false,534,429);                      
            string[] fecha_separada = calendario.Value.ToShortDateString().Split('/');
            string ticket = "SELECT p.folio_venta,fyh,nombre,p.totalVenta,p.tarjeta FROM venta v, detalles_pagos p, logeo l WHERE v.folio_venta = p.folio_venta AND v.id_sesion = l.id_sesion AND month(fyh)= '" + fecha_separada[1] + "' AND year(fyh)= '" + fecha_separada[2] + "' AND day(fyh)= '" + fecha_separada[0] + "'";
            funcionesSQL.llenartabla(dataGridView1, ticket);
            for (int fila=0;fila<dataGridView1.Rows.Count;fila++)
            {
                total_venta += Convert.ToDouble(dataGridView1[3,fila].Value);
            }
            label2.Text = total_venta.ToString();
        }
        private void button4_Click(object sender, EventArgs e)  // entrada
        {            
            activo = 2;
            panel1.Visible = false;
            controles(false,true, 534, 429);                                        
            string[] fecha_separada = calendario.Value.ToShortDateString().Split('/');
            string entradas = "SELECT folio_entrada,fyh as fecha,nombre as cajero,descripcion,cantidad FROM entrada e,logeo l WHERE e.id_sesion = l.id_sesion AND month(fyh)= '"+fecha_separada[1]+"' AND year(fyh)= '"+fecha_separada[2]+"' AND day(fyh)= '"+fecha_separada[0]+"'";
            funcionesSQL.llenartabla(dataGridView1, entradas);
            reinicioVariables();
            descripcion.Text = "ENTRADAS";
        }       
        private void button1_Click(object sender, EventArgs e)  // salida
        {
            activo = 2;
            panel1.Visible = false;
            controles(false,true, 534, 429);                   
            string[] fecha_separada = calendario.Value.ToShortDateString().Split('/');
            string salidas = "SELECT folio_salida,fyh as fecha,nombre as cajero,descripcion,cantidad FROM salida s,logeo l WHERE s.id_sesion = l.id_sesion AND month(fyh)= '" + fecha_separada[1] + "' AND year(fyh)= '" + fecha_separada[2] + "' AND day(fyh)= '" + fecha_separada[0] + "'";
            funcionesSQL.llenartabla(dataGridView1, salidas);
            reinicioVariables();
            descripcion.Text = "SALIDAS";
        }               
        private void button5_Click(object sender, EventArgs e)  // imprimir entrada
        {
            select_impresora();
            imprimir_especial(dataGridView1, "ENTRADA");
        }
        private void button7_Click(object sender, EventArgs e)  // imrprimir salida
        {
            select_impresora();
            imprimir_especial(dataGridView1, "SALIDAS");
        }
        private void button6_Click(object sender, EventArgs e) // impresion de compras
        {
            select_impresora();
            imprecion_ticket(cajerog, notickeg, Convert.ToDouble(totalg), Convert.ToDouble(efectivog), Convert.ToDouble(tarjetag), cambiog, fechag);
        }
        public void agregar_compra(int index)
        {
            flowLayoutPanel3.Controls.Clear();
            int cambio;
            string noticke, fecha, cajero, total = "0", efectivo = "0", tarjeta = "0", buscar_pagos,buscar_articulos;
            List<string> articulos = new List<string>(); 
            List<string> articulos_detalle = new List<string>();
            noticke = dataGridView1.Rows[index].Cells[0].Value.ToString();
            fecha = dataGridView1.Rows[index].Cells[1].Value.ToString();
            cajero = dataGridView1.Rows[index].Cells[2].Value.ToString();
            buscar_pagos = "SELECT*FROM detalles_pagos WHERE folio_venta='" + noticke + "'";
            buscar_articulos = "SELECT folio_venta,nombre,cantidad,subtotal FROM detalle_venta d,articulos a, producto p,categoria c WHERE d.id_articulo = a.id_articulo  AND a.id_producto = p.id_producto AND c.id_cat=a.id_cat AND folio_venta = '" + noticke + "'";
            using (MySqlConnection conexion = new MySqlConnection(ConfigurationManager.ConnectionStrings["conexionGeneral"].ConnectionString))
            {
                conexion.Open();
                using (MySqlCommand consulta = new MySqlCommand())
                {
                    consulta.CommandText = buscar_pagos;
                    consulta.Connection = conexion;
                    using (MySqlDataReader leer_pagos = consulta.ExecuteReader())
                    {
                        while (leer_pagos.Read())
                        {
                            total = leer_pagos[1].ToString();
                            efectivo = leer_pagos[2].ToString();
                            tarjeta = leer_pagos[3].ToString();
                        }
                    }
                    consulta.CommandText = buscar_articulos;
                    consulta.Connection = conexion;
                    using (MySqlDataReader leer_articulos =consulta.ExecuteReader())
                    {
                        while (leer_articulos.Read())
                        {
                            articulos.Add(leer_articulos[1].ToString());
                            articulos_detalle.Add("     " + leer_articulos[2].ToString() + "         " + leer_articulos[3].ToString());
                        }
                    }
                }
            }
            cambio = Convert.ToInt32(efectivo) + Convert.ToInt32(tarjeta) - Convert.ToInt32(total);
            numero_venta.Text = noticke;
            cajero_1.Text = cajero;
            fecha_1.Text = fecha;
            total_venta.Text = total;
            this.efectivo.Text = efectivo; 
            this.tarjeta.Text = tarjeta; 
            this.cambio.Text = cambio.ToString(); // crear los labels dinamicos
            Labeldinamicos(articulos,articulos_detalle);
            notickeg = noticke; fechag = fecha; cajerog = cajero; totalg = total; efectivog = efectivo; tarjetag = tarjeta; cambiog = cambio;
        }
        public void Labeldinamicos(List<string> articulos,List<string> articulos_detalle)
        {
            Label[,] label = new Label[100, 100];
            int pos_arti = 0; // contadores
            int pos_deta = 0; // contadores
            for (int i = 0; i < articulos.Count; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    label[i, j] = new Label();
                    label[i, j].Name = "b" + i;
                    label[i, j].Size = new Size(120, 30);
                    if (j % 2 == 0)
                    {
                        label[i, j].Text = articulos[pos_arti];
                        pos_arti++;
                    }
                    else
                    {
                        label[i, j].Text = articulos_detalle[pos_deta];
                        pos_deta++;
                    }
                    label[i, j].Font = new Font(FontFamily.GenericSansSerif, 8.0F, FontStyle.Bold);
                    flowLayoutPanel3.Controls.Add(label[i, j]);
                }
            }
        }
        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e) // genera el ticket visual en la pantalla
        {
            if (e.RowIndex != -1) {
                int index = 0;
                index = dataGridView1.CurrentRow.Index;
                switch (activo) 
                {
                    case 1:
                        agregar_compra(index); //compra
                        break;
                    case 2:             
                        numero_ticket.Text = dataGridView1[0, index].Value.ToString();      // entrada y salidas                 
                        fecha.Text = dataGridView1[1, index].Value.ToString();
                        cajero.Text = dataGridView1[2, index].Value.ToString();
                        dato1.Text = dataGridView1[3, index].Value.ToString();
                        precio.Text = dataGridView1[4, index].Value.ToString();
                        total.Text = dataGridView1[4, index].Value.ToString();
                        break;
                }
            }          
        }
        public void imprecion_ticket(string cajero,string noTicket ,double total, double efectivo, double tarjeta, double cambio,string fecha)
        {
            string buscar_articulos = "SELECT nombre,cantidad,subtotal FROM detalle_venta d,articulos a, producto p,categoria c WHERE d.id_articulo = a.id_articulo  AND a.id_producto = p.id_producto AND c.id_cat=a.id_cat AND folio_venta = '" + noTicket + "'";           
            List<string> nombre_art = new List<string>();
            List<int> cantidad = new List<int>();
            List<int> precio = new List<int>();
            int contadorArticulo = 0;
            using (MySqlConnection conexion = new MySqlConnection(ConfigurationManager.ConnectionStrings["conexionGeneral"].ConnectionString))
            { 
                conexion.Open();
                using (MySqlCommand consulta = new MySqlCommand(buscar_articulos,conexion))
                {
                   using(MySqlDataReader leer_art = consulta.ExecuteReader())
                   {
                      while (leer_art.Read())
                      {
                         nombre_art.Add(leer_art[0].ToString());
                         cantidad.Add(Convert.ToInt32(leer_art[1]));
                         precio.Add(Convert.ToInt32(leer_art[2]));
                      }
                   }
                }
            }
            if (dataGridView1.Rows.Count > 0)
            {
                if (numero_venta.Text != "none")
                {                    
                    CrearTicket ticket = new CrearTicket();
                    ticket.AbreCajon();
                    ticket.TextoCentro("HAPPY CAKE PASTELERIAS");
                    ticket.TextoIzquierda("AV. DE LA LUNA SM. 504");
                    ticket.TextoIzquierda("TEL: 99891493900");
                    ticket.TextoIzquierda("R.F.C: LOFN730107DD1");
                    ticket.TextoIzquierda("Caja # 1 Ticket #" + noTicket);
                    ticket.lineasAsteriscos();
                    ticket.TextoIzquierda("CAJERO:" + cajero);
                    ticket.TextoIzquierda("FECHA:" + fecha);
                    ticket.lineasAsteriscos();
                    ticket.EncabezadoVenta();   //NOMBRE DEL ARTICULO, CANT, PRECIO
                    ticket.lineasAsteriscos();
                    // generacion de articulos 
                    for (int i = 0; i < nombre_art.Count; i++)
                    {
                        ticket.AgregaArticulo(nombre_art[i], cantidad[i], precio[i]);
                        contadorArticulo += cantidad[i];
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
                    ticket.TextoIzquierda("ARTÍCULOS VENDIDOS:" + contadorArticulo);
                    ticket.TextoIzquierda("");
                    ticket.TextoIzquierda("");
                    ticket.TextoIzquierda("");
                    ticket.TextoIzquierda("");
                    ticket.TextoIzquierda("");
                    ticket.TextoIzquierda("");
                    ticket.TextoIzquierda("");
                    ticket.ImprimirTicket(impresora);
                    MessageBox.Show("IMPRESION REALIZADA CON EXITO", "");
                    reinicioVariables();
                }
                else MessageBox.Show("no hay nada que imprimir");
            }
           else MessageBox.Show("no hay nada que imprimir");
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
        public void imprimir_especial(DataGridView tabla,string concepto)
        {
            if (tabla.Rows.Count > 0)
            {
                if (numero_ticket.Text != "none" )
                {
                    int index;
                    index = tabla.CurrentRow.Index;
                    CrearTicket ticket = new CrearTicket();
                    ticket.AbreCajon();
                    ticket.TextoCentro("HAPPY CAKE PASTELERIAS");
                    ticket.TextoIzquierda("AV. DE LA LUNA SM. 504");
                    ticket.TextoIzquierda("TEL: 99891493900");
                    ticket.TextoIzquierda("R.F.C: LOFN730107DD1");
                    ticket.TextoIzquierda("Ticket #" + tabla[0, index].Value);
                    ticket.TextoCentro(concepto); // si es entrada o salida
                    ticket.lineasAsteriscos();
                    ticket.TextoIzquierda("CAJERO:" + tabla[2, index].Value);
                    ticket.TextoIzquierda("FECHA:" + tabla[1, index].Value);
                    ticket.lineasAsteriscos();
                    ticket.EncabezadoVenta();   //NOMBRE DEL ARTICULO, CANT, PRECIO
                    ticket.lineasAsteriscos();
                    ticket.AgregaArticulo(tabla[3, index].Value.ToString(), 1, Convert.ToDouble(tabla[4, index].Value));
                    // generacion de articulos 
                    ticket.lineasIgual();
                    //  precios
                    ticket.AgregarTotales("TOTAL.........$", Convert.ToDouble(tabla[4, index].Value));
                    ticket.TextoIzquierda("");
                    ticket.TextoIzquierda("");
                    ticket.TextoIzquierda("");
                    ticket.TextoIzquierda("");
                    ticket.TextoIzquierda("");
                    ticket.TextoIzquierda("");
                    ticket.TextoIzquierda("");
                    ticket.CortaTicket();
                    ticket.ImprimirTicket(impresora); //Nombre de la impresora ticketera
                    MessageBox.Show("IMPRESION REALIZADA CON EXITO", "");
                    reinicioVariables();
                }
                else MessageBox.Show("no hay nada que imprimir");
            }
            else MessageBox.Show("no hay nada que imprimir");
        }
        public  void reinicioVariables()
        {
            // varibales del ticket de entradas
            numero_ticket.Text = "none";
            cajero.Text = "none";
            fecha.Text = "none";
            dato1.Text = "none";
            precio.Text = "none";
            total.Text = "0";
            flowLayoutPanel3.Controls.Clear();
            // variables de compra
            numero_venta.Text = "none";
            cajero_1.Text = "none";
            fecha_1.Text = "none";
            total_venta.Text = "0";
            efectivo.Text = "0";
            tarjeta.Text = "0";
            cambio.Text = "0";
        }      
    }
}
