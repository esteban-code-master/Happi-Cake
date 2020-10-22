using System;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace Happy_Cake
{
    class operaciones
    {
        funcionesSQL funcionesSQL = new funcionesSQL();
        DataGridView tabla;
        MySqlConnection conexion = new MySqlConnection(ConfigurationManager.ConnectionStrings["conexionGeneral"].ConnectionString);
        string impresora_seleccionada, impresora, id_sesion, Ingresar_venta, Ingresar_detalle_venta, Ingresar_pago;
        double efectivo_entrante, tarjeta_entrante, total_compra, cantitad_articulos, change;
        int numRepeticion = 1 ; // numero de veces que se debe de imprimir el ticket
        public operaciones(double efectivo, double tarjeta, DataGridView tabla, double total_compra)
        {
            impresora_seleccionada = null;
            impresora = null;
            this.efectivo_entrante = efectivo;
            this.tarjeta_entrante = tarjeta;
            this.tabla = tabla;
            this.total_compra = total_compra;
        }
        public operaciones()
        {

        }
        public double cambio(double efectivo, double tarjeta)
        {
            double cambio = 0;
             if(efectivo != 0)
             {
                cambio = efectivo - (total_compra - tarjeta);  
             }
            return cambio;
        }
        public void Realizar_venta()
        {
            change = cambio(efectivo_entrante,tarjeta_entrante);   // realiza el cambio
            id_sesion = funcionesSQL.id_sesion();        // retorna la ultima sesion abierta
            conexion.Open();
            using (MySqlCommand ingresar_num_venta = new MySqlCommand())
            {
                using (MySqlTransaction transaction = conexion.BeginTransaction())
                {
                    try
                    {
                      // inicio de la transaccion
                        Ingresar_venta = "INSERT INTO venta (fyh,id_sesion) VALUES(now(),'" + id_sesion + "')";
                        ingresar_num_venta.CommandText = Ingresar_venta;
                        ingresar_num_venta.Connection = conexion;
                        ingresar_num_venta.Transaction = transaction;
                        ingresar_num_venta.ExecuteNonQuery();
                        for (int i = 0; i < tabla.Rows.Count; i++)
                        {                                                                                            //  folio_venta                |      id_articulo   |  cantidad vendida del mismo articulo | precio actuial del producto
                            Ingresar_detalle_venta = "INSERT INTO detalle_venta VALUES((SELECT folio_venta FROM venta  ORDER BY folio_venta DESC limit 1),'" + tabla[3, i].Value + "','" + tabla[0, i].Value + "','" + tabla[2, i].Value + "')";
                            ingresar_num_venta.CommandText = Ingresar_detalle_venta;
                            ingresar_num_venta.Connection = conexion;
                            ingresar_num_venta.Transaction = transaction;
                            ingresar_num_venta.ExecuteNonQuery();
                            cantitad_articulos += Convert.ToDouble(tabla[0, i].Value);   // contar el total de los articulos vendidos
                            if (tabla[3,i].Value.ToString() == "272")
                            {
                                numRepeticion = 2;
                            }
                        }
                        Ingresar_pago = "INSERT INTO detalles_pagos VALUES ((SELECT folio_venta FROM venta  ORDER BY folio_venta DESC limit 1),'" + total_compra + "','" + efectivo_entrante + "','" + tarjeta_entrante + "')";
                        ingresar_num_venta.CommandText = Ingresar_pago;
                        ingresar_num_venta.Connection = conexion;
                        ingresar_num_venta.Transaction = transaction;
                        ingresar_num_venta.ExecuteNonQuery();
                        transaction.Commit();

                        Ventana_emergente ventana_Emergente = new Ventana_emergente("Su cambio es \n"+" $"+change);
                        ventana_Emergente.StartPosition = FormStartPosition.CenterScreen;
                        ventana_Emergente.ShowDialog();
                        select_impresora();
                        imprecion_ticket(tabla, id_sesion, total_compra, efectivo_entrante,tarjeta_entrante, change, cantitad_articulos.ToString());
                        efectivo_entrante = 0; tarjeta_entrante=0; tabla.Rows.Clear(); total_compra = 0;
                    }
                    catch
                    {
                        MessageBox.Show("error al ingresar", "");
                        transaction.Rollback();
                    }
                }
            }
            conexion.Close();
            conexion.Dispose();
        }
        public void select_impresora()
        {
            impresora_seleccionada = "SELECT * FROM impresora WHERE estatus='1'";
            using (MySqlConnection conexion = new MySqlConnection(ConfigurationManager.ConnectionStrings["conexionImpresora"].ConnectionString))
            {
                conexion.Open();
                using (MySqlCommand consulta = new MySqlCommand(impresora_seleccionada, conexion))
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
        public void imprecion_ticket(DataGridView tabla_venta, string id_session, double total, double efectivo, double tarjeta, double cambio, string cantidad)
        {
            string cajero = null, noTicket = null;
            string busca_cajero = "SELECT nombre FROM logeo WHERE id_sesion='" + id_session + "'";
            string busca_noticket = "SELECT folio_venta FROM venta  ORDER BY folio_venta DESC limit 1";
            using (MySqlConnection conexion = new MySqlConnection(ConfigurationManager.ConnectionStrings["conexionGeneral"].ConnectionString))
            {
                conexion.Open();
                using (MySqlCommand consulta = new MySqlCommand())
                {
                    consulta.CommandText =busca_noticket;
                    consulta.Connection =conexion;
                    using (MySqlDataReader leer_ticket = consulta.ExecuteReader())
                    {
                        while (leer_ticket.Read())
                        {
                            noTicket = leer_ticket[0].ToString();
                        }
                    }
                    consulta.CommandText =busca_cajero;
                    consulta.Connection =conexion;
                    using (MySqlDataReader leer_cajero = consulta.ExecuteReader())
                    {
                        while (leer_cajero.Read())
                        {
                            cajero = leer_cajero[0].ToString();
                        }
                    }
                }
            }
            detalle_ticket(tabla_venta,noTicket,id_session,cajero,total,efectivo,tarjeta,cambio,cantidad);            
        }
        public void detalle_ticket(DataGridView tabla_venta, string noTicket,string id_session,string cajero,double total,double efectivo,double tarjeta,double cambio,string cantidad)
        {
            for (int j = 0; j < numRepeticion; j++)
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
                ticket.TextoExtremos("FECHA: " + DateTime.Now.ToShortDateString(), "HORA: " + DateTime.Now.ToShortTimeString());
                ticket.lineasAsteriscos();
                ticket.EncabezadoVenta();   //NOMBRE DEL ARTICULO, CANT, PRECIO
                ticket.lineasAsteriscos();
                // generacion de articulos 
                for (int i = 0; i < tabla_venta.Rows.Count; i++)
                {                                                                                   // el 0 es del nombre 1 cantidad 2 precio
                    ticket.AgregaArticulo(tabla_venta[1, i].Value.ToString(), Convert.ToInt32(tabla_venta[0, i].Value), Convert.ToDouble(tabla_venta[2, i].Value));
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
        }
    }
}