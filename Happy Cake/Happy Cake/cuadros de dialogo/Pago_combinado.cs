using System;
using System.Windows.Forms;

namespace Happy_Cake
{
    public partial class Pago_combinado : Form
    {
        operaciones operaciones = new operaciones();
        DataGridView tabla;
        double totalVenta, efectivo, tarjeta;
        public Pago_combinado(double totalVenta,DataGridView tabla)
        {
            InitializeComponent();
            this.tabla = tabla;
            this.totalVenta = totalVenta;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            double comparacion_efectivo;
            if (textBox1.Text.Trim() != "" && textBox2.Text.Trim() != "")
            {
                efectivo = Convert.ToDouble(textBox1.Text);
                tarjeta = Convert.ToDouble(textBox2.Text);
                comparacion_efectivo = efectivo + tarjeta;
                if (comparacion_efectivo >= totalVenta)
                    if ( tarjeta <= totalVenta)
                    {
                        operaciones operaciones = new operaciones(efectivo,tarjeta,tabla,totalVenta);
                        operaciones.Realizar_venta();                       
                        this.Close();
                    }
                    else MessageBox.Show("la cantidad ingresada no concuerda con el metodo establecido");
                else MessageBox.Show("La cantidad ingresada es menor al total de la compra \n porfavor verifica", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else MessageBox.Show("Campo vacios \n  Pofavor de verificar", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            validador.solo_numeros(e);
        }
    }
}
