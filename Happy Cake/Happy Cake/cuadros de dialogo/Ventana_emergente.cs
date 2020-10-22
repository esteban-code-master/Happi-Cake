using System;
using System.Windows.Forms;

namespace Happy_Cake
{
    public partial class Ventana_emergente : Form
    {
        public Ventana_emergente(double cambio)
        {
            InitializeComponent();
            label2.Text = cambio.ToString();
        }
        public Ventana_emergente(string texto)
        {
            InitializeComponent();
            label1.Text = texto; label2.Text = null;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
