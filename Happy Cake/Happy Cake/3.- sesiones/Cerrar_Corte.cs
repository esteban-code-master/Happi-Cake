using System;
using System.Configuration;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Happy_Cake
{
    public partial class Cerrar_Corte : Form
    {
        string impresora_seleccionada = null, impresora = null, cajero;
        double diferencia = 0, fondo = 0, ventaTotal = 0, Entrada = 0, Salida = 0, venta_cajero = 0, dineroTarjeta = 0;
        string sesionAux = null,id_sesion=null;
        public enum estatus
        {
            session_cerrada = 0
        }
        public Cerrar_Corte()
        {
            InitializeComponent();
        }
        private void Cerrar_Corte_Load(object sender, EventArgs e)
        {
            dataGridView2.Rows.Add();
            dataGridView2.Rows[0].Cells[0].Value = "1";
            dataGridView2.Rows.Add();
            dataGridView2.Rows[1].Cells[0].Value = "2";
            dataGridView2.Rows.Add();
            dataGridView2.Rows[2].Cells[0].Value = "5";
            dataGridView2.Rows.Add();
            dataGridView2.Rows[3].Cells[0].Value = "10";
            dataGridView2.Rows.Add();
            dataGridView2.Rows[4].Cells[0].Value = "20";
            dataGridView2.Rows.Add();
            dataGridView2.Rows[5].Cells[0].Value = "50";
            dataGridView2.Rows.Add();
            dataGridView2.Rows[6].Cells[0].Value = "100";
            dataGridView2.Rows.Add();
            dataGridView2.Rows[7].Cells[0].Value = "200";
            dataGridView2.Rows.Add();
            dataGridView2.Rows[8].Cells[0].Value = "500";
            dataGridView2.Rows.Add();
            dataGridView2.Rows[9].Cells[0].Value = "1000";
            dataGridView2.Rows.Add();
            dataGridView2.Rows[10].Cells[0].Value = "1";
            dataGridView2[1, 0].Value = "0";
            dataGridView2[1, 1].Value = "0";
            dataGridView2[1, 2].Value = "0";
            dataGridView2[1, 3].Value = "0";
            dataGridView2[1, 4].Value = "0";
            dataGridView2[1, 5].Value = "0";
            dataGridView2[1, 6].Value = "0";
            dataGridView2[1, 7].Value = "0";
            dataGridView2[1, 8].Value = "0";
            dataGridView2[1, 9].Value = "0";
            dataGridView2[1, 10].Value = "0";
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            validador.solo_numeros(e);
        }       
        private void button3_Click(object sender, EventArgs e)  // boton de corte 
        {          
            string datos_sesion, venta_sistema, total_entrada, total_salida;     // valores del sistema 
            datos_sesion = "SELECT id_sesion ,nombre,fondo FROM logeo WHERE status=1";           
            try
            {
                using (MySqlConnection conexion = new MySqlConnection(ConfigurationManager.ConnectionStrings["conexionGeneral"].ConnectionString))
                {
                    conexion.Open();
                    using (MySqlCommand consulta = new MySqlCommand())
                    {
                        consulta.CommandText = datos_sesion;
                        consulta.Connection = conexion;
                        using (MySqlDataReader leer =consulta.ExecuteReader() )
                        {
                            while (leer.Read())
                            {
                                id_sesion = leer[0].ToString();
                                cajero = leer[1].ToString();
                                fondo = Convert.ToDouble(leer[2]);
                            }
                        }
                        if (id_sesion != null)
                        {
                            venta_sistema = "SELECT SUM(totalVenta) FROM detalles_pagos d,venta v WHERE d.folio_venta = v.folio_venta AND id_sesion ='" + id_sesion + "' GROUP by id_sesion";
                            total_entrada = "SELECT SUM(cantidad) FROM entrada WHERE id_sesion ='" + id_sesion + "' GROUP BY id_sesion";
                            total_salida = "SELECT SUM(cantidad) FROM salida WHERE id_sesion='" + id_sesion + "' GROUP BY id_sesion";
                            consulta.CommandText = venta_sistema;
                            consulta.Connection = conexion;
                            using (MySqlDataReader leer_venta = consulta.ExecuteReader())
                            {
                                while (leer_venta.Read())
                                {
                                    ventaTotal = Convert.ToDouble(leer_venta[0]);
                                }
                            }
                            consulta.CommandText = total_entrada;
                            consulta.Connection = conexion;
                            using (MySqlDataReader leer_entrada = consulta.ExecuteReader())
                            {
                                while (leer_entrada.Read())
                                {
                                    Entrada = Convert.ToDouble(leer_entrada[0]);
                                }
                            }
                            consulta.CommandText = total_salida;
                            consulta.Connection = conexion;
                            using (MySqlDataReader leer_salida = consulta.ExecuteReader())
                            {
                                while (leer_salida.Read())
                                {
                                    Salida = Convert.ToDouble(leer_salida[0]);
                                }
                            }
                            realizar_corte();
                        }
                        else MessageBox.Show("No hay corte pendiente , intente de nuevo");
                    }
                }
            }
            catch
            {
                MessageBox.Show("Algo ha salido mal");
            }
        }             
        private void button2_Click(object sender, EventArgs e) // jalar valores
        {
            double cantidad_entrante = 0;
            try
            {
                for (int i = 0; i < 11; i++)
                {
                    cantidad_entrante = cantidad_entrante + (Convert.ToDouble(dataGridView2[0, i].Value)) * (Convert.ToDouble(dataGridView2[1, i].Value));
                }
                venta_cajero = cantidad_entrante;   // dinero ingresado por cajero en efectivo
                labEfectivo.Text = cantidad_entrante.ToString("#0,0.00");
                cantidad_entrante = 0;
                if (textBox1.Text.Trim() != "")
                {
                    dineroTarjeta = Convert.ToDouble(textBox1.Text.Trim()); // dinero ingresado por cajero en tarjeta
                    labTarjet.Text = dineroTarjeta.ToString("#0,0.00"); 
                }
                else
                {
                    MessageBox.Show("campo vacio");
                }
            }
            catch (Exception)
            {

            }
        }       
        private void button1_Click(object sender, EventArgs e)  // boton de imprimir corte de ticket
        {
            if (sesionAux != null) {
                select_impresora();
                imprecion_ticket(ventaTotal, Convert.ToDouble(venta_cajero), Convert.ToDouble(dineroTarjeta), diferencia, cajero, Entrada, Salida, fondo);
            }
            else
            {
                MessageBox.Show("No hay nada que imprimir, intente de nuevo");
            }
        }       
        private void dataGridView2_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)   // validacion de datagrid
        {
            DataGridViewTextBoxEditingControl dText = (DataGridViewTextBoxEditingControl)e.Control;

            dText.KeyPress -= new KeyPressEventHandler(dataGridView2_KeyPress);
            dText.KeyPress += new KeyPressEventHandler(dataGridView2_KeyPress);
        }
        private void dataGridView2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (Char.IsControl(e.KeyChar))
            {
                e.Handled = false;
            }
            else if (Char.IsSeparator(e.KeyChar))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
        public void imprecion_ticket(double total, double efectivo, double tarjeta, double diferencia, string cajero,double entrada,double salida,double fondo)
        {
                CrearTicket ticket = new CrearTicket();
                ticket.AbreCajon();
                ticket.TextoCentro("HAPPY CAKE PASTELERIAS");
                ticket.TextoIzquierda("AV. DE LA LUNA SM. 504");
                ticket.TextoIzquierda("TEL: 99891493900");
                ticket.TextoIzquierda("R.F.C: LOFN730107DD1");
                ticket.TextoCentro("CORTE DE CAJA");
                ticket.TextoIzquierda("Caja # 1");
                ticket.lineasAsteriscos();
                ticket.TextoIzquierda("Cajero:" + cajero);
                ticket.TextoExtremos("FECHA: " + DateTime.Now.ToShortDateString(), "HORA: " + DateTime.Now.ToShortTimeString());
                ticket.lineasAsteriscos();
                ticket.lineasIgual();
                //  precios
                ticket.AgregarTotales("VENTA TOTAL $", total);
                ticket.lineasIgual();
                ticket.TextoIzquierda("");
                ticket.AgregarTotales("FONDO INICIAL $", fondo);
                ticket.AgregarTotales("EFECTIVO CAJERO $", efectivo);
                ticket.AgregarTotales("TARJETA $", tarjeta);
                ticket.AgregarTotales("ENTRADAS EN TURNO $", entrada);
                ticket.AgregarTotales("SALIDAS EN  TURNO $", salida);
                ticket.lineasIgual();
                ticket.AgregarTotales("DIFERENCIA  $", diferencia);
                ticket.lineasIgual();
                //texto final;
                ticket.TextoIzquierda("");
                ticket.TextoIzquierda("");
                ticket.TextoIzquierda("");
                ticket.TextoIzquierda("");
                ticket.TextoIzquierda("");
                ticket.TextoIzquierda("");
                ticket.TextoIzquierda("");
                ticket.CortaTicket();
                ticket.ImprimirTicket(impresora); //Nombre de la impresora ticketera
                MessageBox.Show("Imprimiendo...");
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
        public void realizar_corte()
        {
            double superTotal, cantidadTotal_cajero;
            superTotal = (fondo + ventaTotal + Entrada - Salida); // venta total sistema
            cantidadTotal_cajero = Convert.ToDouble(venta_cajero) + Convert.ToDouble(dineroTarjeta); // venta total segun cajero
            diferencia = cantidadTotal_cajero - superTotal; // diferencia
            DialogResult resultado;
            if (diferencia > 20 || diferencia < -20)
            {
                resultado = MessageBox.Show("La diferencia supera a lo establecido", "Desea continuar", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            }
            else
            {
                resultado = DialogResult.OK;
            }
            if (resultado == DialogResult.OK)
            {
                funcionesSQL funciones = new funcionesSQL();
                Ventana_emergente ventana_Emergente = new Ventana_emergente("DIFERENCIA $ " + diferencia);
                ventana_Emergente.StartPosition = FormStartPosition.CenterParent;
                ventana_Emergente.ShowDialog();
                string ingresar_diferencia = "INSERT INTO diferencias VALUES('" + id_sesion + "','" + diferencia + "')";
                funciones.insertar(ingresar_diferencia);
                string cerrar_session = "UPDATE logeo SET status='" + (int)estatus.session_cerrada + "' WHERE id_sesion='" + id_sesion + "'";
                funciones.insertar(cerrar_session);
                /* datos del ticket*/
                labelcajero.Text = cajero;
                labelfecha.Text = DateTime.Now.ToShortDateString();
                labelventa.Text = ventaTotal.ToString("#,0.00"); // total venta
                labelEfectivo.Text = venta_cajero.ToString("#,0.00");  // efectivo entrante
                labelTarjeta.Text = dineroTarjeta.ToString("#,0.00") ; // tarjeta
                labelEntrada.Text = Entrada.ToString("#,0.00");// entrada
                labelSalidas.Text = Salida.ToString("#,0.00"); // salida
                labelDiferencia.Text = diferencia.ToString("#,0.00"); // diferencia
                labelFondo.Text = fondo.ToString("#,0.00");
                sesionAux = id_sesion;
                id_sesion = null;
           }
        }
    }
}
