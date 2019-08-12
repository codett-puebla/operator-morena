using System;
using System.Data.Entity;
using System.Linq;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using operator_morena.Connection;

namespace operator_morena
{
    public partial class scrSubOperator : MaterialForm
    {
        public string operators_key;

        public scrSubOperator()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;

            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Red900, //Color dialog 
                Primary.Red900, //Color control buttons
                Primary.Blue400,
                Accent.LightBlue200,
                TextShade.WHITE
            );
        }

        #region FUNCIONES

        private void fill_dgv(string search_value)
        {
            dgvOperator.Rows.Clear();
            var db = new ConnectionDB();
            var datos = db.Operators.Join(db.Sections, x => x.id_sections, y => y.id,
                (x, y) => new
                {
                    x.id,
                    x.name,
                    x.alias,
                    x.email,
                    x.phone,
                    x.status,
                    x.id_operators_key,
                    x.operators_key,
                    y.section,
                    y.town_name,
                    y.location_name
                }
            ).Where(x => x.status == 1 && x.operators_key != operators_key);

            if (!string.IsNullOrEmpty(search_value))
                datos = datos.Where(x => x.name.Contains(search_value) ||
                                         x.alias.Contains(search_value) ||
                                         x.email.Contains(search_value) ||
                                         x.phone.Contains(search_value) ||
                                         x.section.Contains(search_value) ||
                                         x.town_name.Contains(search_value) ||
                                         x.location_name.Contains(search_value));

            var datos_f = datos.ToList();

            foreach (var item in datos_f)
                dgvOperator.Rows.Add(item.id, item.name, item.alias, item.email, item.phone, item.section,
                    item.town_name, item.location_name);
        }

        #endregion

        private void scrSubOperator_Load(object sender, EventArgs e)
        {
            var db = new ConnectionDB();
            dgvSubOperator.Rows.Clear();

            var datos = db.Operators.Join(db.Sections, x => x.id_sections, y => y.id,
                (x, y) => new
                {
                    x.id,
                    x.name,
                    x.alias,
                    x.email,
                    x.phone,
                    x.status,
                    x.id_operators_key,
                    y.section,
                    y.town_name,
                    y.location_name
                }
            ).Where(x => x.status == 1 && x.id_operators_key == operators_key).ToList();

            foreach (var item in datos)
                dgvSubOperator.Rows.Add(item.id, item.name, item.alias, item.email, item.phone, item.section,
                    item.town_name, item.location_name);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            fill_dgv(tbSearch.Text);
        }

        private void aGREGARToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var index = dgvOperator.CurrentRow.Index;
            var id = Convert.ToInt32(dgvOperator.Rows[index].Cells[0].Value);

            foreach (DataGridViewRow row in dgvSubOperator.Rows)
                if (id == Convert.ToInt32(row.Cells[0].Value))
                {
                    MessageBox.Show("Operador ya asociado", "Operador", MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

            dgvSubOperator.Rows.Add(dgvOperator.Rows[index].Cells[0].Value,
                dgvOperator.Rows[index].Cells[1].Value,
                dgvOperator.Rows[index].Cells[2].Value,
                dgvOperator.Rows[index].Cells[3].Value,
                dgvOperator.Rows[index].Cells[4].Value,
                dgvOperator.Rows[index].Cells[5].Value,
                dgvOperator.Rows[index].Cells[6].Value,
                dgvOperator.Rows[index].Cells[7].Value);
        }

        private void eLIMINARToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var index = dgvSubOperator.CurrentRow;
            dgvSubOperator.Rows.Remove(index);
        }

        private void tsbtnSaveRelation_Click(object sender, EventArgs e)
        {
            var db = new ConnectionDB();

            //ELIMINAMOS LA RELACION ANTERIOR
            var operators = db.Operators.Where(x => x.id_operators_key == operators_key).ToList();
            foreach (var item in operators)
            {
                item.id_operators_key = "";

                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }

            //ESTABLECEMOS LA RELACION ACTUAL
            foreach (DataGridViewRow row in dgvSubOperator.Rows)
            {
                var id = Convert.ToInt32(row.Cells[0].Value);
                var operator_ = db.Operators.Where(x => x.id == id).FirstOrDefault();

                operator_.id_operators_key = operators_key;

                db.Entry(operator_).State = EntityState.Modified;
                db.SaveChanges();
            }

            MessageBox.Show("Relacion guardada con éxito", "Operador", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}