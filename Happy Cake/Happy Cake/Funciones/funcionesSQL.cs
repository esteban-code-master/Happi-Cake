using System;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.Data;


namespace Happy_Cake
{
    class funcionesSQL
    {
        string cadena = ConfigurationManager.ConnectionStrings["conexionGeneral"].ConnectionString;
        string configuraciones = ConfigurationManager.ConnectionStrings["conexionImpresora"].ConnectionString;
        MySqlConnection conexion = new MySqlConnection();
        MySqlConnection conexion_config = new MySqlConnection();
        public  enum estatus
        {
            session_iniciada =1, session_cerrada =0
        }
        public funcionesSQL()
        {
            conexion.ConnectionString = cadena;
            conexion_config.ConnectionString = configuraciones;
        }
        public int insertar(string cadena)
        {
            // si retorna 0 signifia que no se puedo insertar los datos 
            int retorno = 0;
            try
            {
                conexion.Open();
                MySqlCommand insert = new MySqlCommand(cadena, conexion);
                retorno = insert.ExecuteNonQuery();
                insert.Dispose();
            }
            catch (Exception)
            {
                return retorno;
            }
            finally
            {                
                conexion.Close();
                conexion.Dispose();                
            }
            return retorno;
        }
        public int insertar_impresora(string cadena)
        {
            // si retorna 0 signifia que no se puedo insertar los datos 
            int retorno = 0;
            try
            {
                conexion_config.Open();
                using (MySqlCommand insert = new MySqlCommand(cadena, conexion_config))
                {
                    retorno = insert.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                return retorno;
            }
            finally
            {
                conexion_config.Close();
                conexion_config.Dispose();
            }
            return retorno;
        }
        public int llenartabla(DataGridView grid, string cadena)
        {
            int retorno = 0;
            try
            {
                conexion.Open();
                using (MySqlCommand realizar = new MySqlCommand(cadena,conexion ))
                {
                    using (MySqlDataAdapter da = new MySqlDataAdapter(realizar))
                    {
                        using (DataTable dt = new DataTable())
                        {
                            da.Fill(dt);
                            grid.DataSource = dt;
                            return retorno;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return retorno = 1;
            }
            finally
            {
                conexion.Close();
                conexion.Dispose();
            }
           
        }        
        // metodo para obtejer el id de la sesion iniciada
        public string id_sesion()
        {
            string id = null;
            string id_sesion = "SELECT id_sesion FROM logeo WHERE status='" + (int)estatus.session_iniciada + "'";
            try
            {
                conexion.Open();
                using (MySqlCommand consultar = new MySqlCommand(id_sesion,conexion))
                {
                    using (MySqlDataReader leer = consultar.ExecuteReader())
                    {
                        while (leer.Read())
                        {
                            id = leer[0].ToString();
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("contacte con el desarrolador ", "Algo a ocurrido mal", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                conexion.Close();
                conexion.Dispose();
            }
            return id;
        }
    }
}
