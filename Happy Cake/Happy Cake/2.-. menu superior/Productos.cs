using System;
using System.Windows.Forms;

namespace Happy_Cake
{
    public partial class Productos : Form
    {
        funcionesSQL funciones = new funcionesSQL();
        enum categoria{
            // identificador de tamaños esta relacionado a la base de datos 
            Grande=1,Mediano=2,Chico=3,Individual=4,Rebanada=5,Pieza=6
        }
        public Productos()
        {
            InitializeComponent();
        }
        private void Productos_Load(object sender, EventArgs e)
        {
            string cadena = "SELECT a.id_producto as id_prod,a.id_articulo as id_art,nombre,descripcion,categoria,precio FROM producto p,articulos a,categoria c,clasificacion cl WHERE  a.id_cat=c.id_cat AND a.id_producto=p.id_producto AND cl.id_clase=p.clase ORDER BY a.id_producto,a.id_articulo";
            funciones.llenartabla(dataGridView1,cadena);
            dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[0].Width = 60;
            dataGridView1.Columns[1].Width = 60;
            dataGridView1.Columns[4].DefaultCellStyle.Format = "C2";
        }
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox3.Text.Trim() != "")
            {
                string cadena = "SELECT a.id_producto,a.id_articulo,nombre,descripcion,categoria,precio FROM producto p,articulos a,categoria c,clasificacion cl WHERE  a.id_cat=c.id_cat AND a.id_producto=p.id_producto AND cl.id_clase=p.clase AND nombre like'" + textBox3.Text + "%' ORDER BY a.id_producto,a.id_articulo";
                funciones.llenartabla(dataGridView1, cadena);
            }
            else
            {
                string cadena = "SELECT a.id_producto as id_prod,a.id_articulo as id_art,nombre,descripcion,categoria,precio FROM producto p,articulos a,categoria c,clasificacion cl WHERE  a.id_cat=c.id_cat AND a.id_producto=p.id_producto AND cl.id_clase=p.clase ORDER BY a.id_producto,a.id_articulo";
                funciones.llenartabla(dataGridView1, cadena);
            }
        }
        //llamada a nuevo producto
        private void button1_Click(object sender, EventArgs e)
        {
            verificacion verificacion = new verificacion();
            verificacion.StartPosition = FormStartPosition.CenterParent;
            verificacion.ShowDialog();
        }
    }
}
