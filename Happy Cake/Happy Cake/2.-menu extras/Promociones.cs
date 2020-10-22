using System.Windows.Forms;

namespace Happy_Cake
{
    public partial class Cobros_Especiales : Form
    {
        public delegate void capturar_articulo(string id_articulo, bool promo);
        public event capturar_articulo recibir_info;
        bool promo_activada = true ; // indicador de la promocion activada 
        public Cobros_Especiales()
        {   //120, 183, 218  
            InitializeComponent();
            int[] color = { 33, 235, 77 };
            string productos="SELECT nombre ,p.id_producto,clase FROM producto p ,articulos a WHERE p.id_producto=a.id_producto and id_cat between 4 and 5";
            Botones_dinamic botones_dinamicamente = new Botones_dinamic(flowLayoutPanel1, productos , promo_activada);
            botones_dinamicamente.botones_dinamicamente(color);
            botones_dinamicamente.recibir_info += new Botones_dinamic.capturar_articulo(recibir_articulo);
        }
        public void recibir_articulo(string id_articulo, bool promo)
        {
            recibir_info(id_articulo, promo);
        }
    }
}
