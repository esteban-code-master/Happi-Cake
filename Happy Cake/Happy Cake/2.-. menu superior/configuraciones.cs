using System;
using System.Configuration;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Happy_Cake
{
    public partial class configuraciones : Form
    {
        string ingresar_impresora = null;
        string buscar_impresora = null;
        enum estatus
        {
            impresora_ativa=1 , impresora_desac=0
        }
        public configuraciones()
        {
          InitializeComponent();    
          foreach (string name in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
          {
               string impresora_existente = null;
               comboBox1.Items.Add(name.ToString());
               buscar_impresora = "SELECT* FROM impresora WHERE nombre_impresora='" + name + "'";
                using (MySqlConnection conexion = new MySqlConnection(ConfigurationManager.ConnectionStrings["conexionImpresora"].ConnectionString))
                {
                    conexion.Open();
                    using (MySqlCommand consulta = new MySqlCommand(buscar_impresora,conexion)) {
                        using (MySqlDataReader leer_impresora = consulta.ExecuteReader())
                        {
                            while (leer_impresora.Read())
                            {
                                impresora_existente = leer_impresora[0].ToString();
                            }
                        }
                        if (impresora_existente != name)
                        {
                            funcionesSQL funcionesSQL = new funcionesSQL();
                            ingresar_impresora = "INSERT INTO impresora VALUES('" + name + "','" + (int)estatus.impresora_desac + "');";
                            funcionesSQL.insertar_impresora(ingresar_impresora);
                        }
                    }
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
           funcionesSQL funcionesSQL = new funcionesSQL();
           string actulizar_todo = "UPDATE impresora SET estatus='" + (int)estatus.impresora_desac + "'";
           string actulizar_actual = "UPDATE impresora SET estatus='" + (int)estatus.impresora_ativa + "' WHERE nombre_impresora='" + comboBox1.Text + "'";
           funcionesSQL.insertar_impresora(actulizar_todo);                      
           funcionesSQL.insertar_impresora(actulizar_actual);
           MessageBox.Show(comboBox1.Text+" seleccionado");
        }
    }
}
