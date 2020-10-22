using System.Windows.Forms;

namespace Happy_Cake
{
    public partial class Velas : Form
    {
        public delegate void capturar_articulo(string id_articulo,bool promo);
        public event capturar_articulo recibir_info;
        bool promo_desactivada = false;
        enum clase
        {
            pastel = 1, velas = 2, galletas = 3, cafes = 4, extra = 5
        }
        public Velas()
        {            
            InitializeComponent();
            int[] color = { 120, 183, 218 };
            string consulta = "SELECT nombre,id_producto,clase FROM producto WHERE clase='" + (int)clase.velas + "' OR clase='"+(int)clase.extra+"'";
            Botones_dinamic botones_dinamicamente = new Botones_dinamic(flowLayoutPanel1, consulta,promo_desactivada);
            botones_dinamicamente.botones_dinamicamente(color);
            botones_dinamicamente.recibir_info += new Botones_dinamic.capturar_articulo(recibir_articulo);
        }
        public void recibir_articulo(string id_articulo,bool promo)
        {
            recibir_info(id_articulo,promo);
        }
    }
}
