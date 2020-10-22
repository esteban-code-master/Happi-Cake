using System;
using System.Windows.Forms;

namespace Happy_Cake
{
    public partial class Abrir_Corte : Form
    {
        enum satatus{
           session_iniciada=1, //  al momento de iniciar session 
        }
        // istancia de las funciones  SQL
        funcionesSQL funciones = new funcionesSQL();
        public Abrir_Corte()
        {
            InitializeComponent();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            // cadena de insertado para la tabla de logeo
            string sesion_abierta = funciones.id_sesion();
            if (sesion_abierta == null)
            {
                if (textBox1.Text.Trim() != "" && textBox2.Text.Trim() != "")
                {
                    DialogResult resutado = MessageBox.Show("Cajero :  " + textBox1.Text + "\nFondo : " + textBox2.Text, "INICIAR SESSION", MessageBoxButtons.YesNo, MessageBoxIcon.Information); ;
                    if (resutado == DialogResult.Yes)
                    {
                        string cadena = "INSERT INTO logeo (nombre,fondo,status) VALUES('" + textBox1.Text + "','" + textBox2.Text + "','" + (int)satatus.session_iniciada + "')";
                        funciones.insertar(cadena);
                        Ventana_emergente ventana_Emergente = new Ventana_emergente("SESION ABIERTA"); /// ventana de cambio
                        ventana_Emergente.StartPosition = FormStartPosition.CenterParent;                        
                        ventana_Emergente.ShowDialog();
                    }
                }
                else MessageBox.Show("Campos vacios", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else MessageBox.Show("Tienes la sesion abierta \n Realize el corte e intente ingresar de nuevo", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            validador.solo_numeros(e);
        }
    }
}
