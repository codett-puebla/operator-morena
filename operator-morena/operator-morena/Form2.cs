using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using GMap.NET;
using GMap.NET.WindowsForms.Markers;
using GMap.NET.WindowsForms;
using GMap.NET.MapProviders;
using operator_morena.Connection;
using operator_morena.Models;
using System.Data.OleDb;
using System.IO;
using System.Security.Cryptography;

namespace operator_morena
{
    public partial class wfDashBoard : MaterialForm
    {
        double LatIncial = 19.043719;
        double LngIncial = -98.198911;
        private int id;
        public int user_kind;
        bool image_click = false;

        public wfDashBoard()
        {
            InitializeComponent();

            MaterialSkinManager materialSkinManager = MaterialSkinManager.Instance;
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

        private void WfDashBoard_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.ExitThread();
            Application.Exit();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            string image_name;
            OpenFileDialog ofd = new OpenFileDialog {
                Filter = "Archivos de Imagen JPG|*.jpg|Todos los archivos|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if(ofd.ShowDialog() == DialogResult.OK)
            {
                image_name = ofd.FileName;
                pbImagen.BackgroundImage = Image.FromFile(image_name);
                image_click = true;
            }
            else
            {
                pbImagen.BackgroundImage = null;
            }

        }

        private void wfDashBoard_Load(object sender, EventArgs e)
        {
            //gMapControl1.DragButton = MouseButtons.Left;
            //gMapControl1.CanDragMap = true;
            //gMapControl1.MapProvider = GMapProviders.GoogleMap;
            //gMapControl1.Position = new PointLatLng(LatIncial, LngIncial);
            //gMapControl1.MinZoom = 0;
            //gMapControl1.MaxZoom = 24;
            //gMapControl1.Zoom = 9;
            //gMapControl1.AutoScroll = true;

            //DEPENDIENDO EL TIPO DE USUARIO MOSTRAMOS LOS BOTONES CORRESPONDIENTES
            if(user_kind == 1)
            {
                tsbtnNuevo.Visible = true;
            }
            else
            {
                tsbtnNuevo.Visible = false;
            }

            ConnectionDB db = new ConnectionDB();

            List<string> sections = db.Sections.Select(x => x.town_name).Distinct().ToList();
            cbMunicipality.Items.Clear();
            cbMunicipality.DataSource = sections;
            fill_dgv("");
        }

        #region FUNCIONES
        private int check_score()
        {
            if (chb1.Checked)
                return 1;
            if (chb2.Checked)
                return 2;
            if (chb3.Checked)
                return 3;
            if (chb4.Checked)
                return 4;
            if (chb5.Checked)
                return 5;
            return 0;
        }

        private byte[] ConvertImageToBinary(Image img)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                if (img != null)
                {
                    img.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    return ms.ToArray();
                }
                else
                {
                    return ms.ToArray();
                }
            }
        }

        private Image ConvertBinaryToImage(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                try
                {
                    return Image.FromStream(ms);
                }
                catch (Exception e)
                {
                    return null;
                }

            }
        }

        private string ConvertImageToBase64(Image image)
        {
            using (MemoryStream m = new MemoryStream())
            {
                if(image != null)
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();

                    // Convert byte[] to Base64 String
                    string base64String = Convert.ToBase64String(imageBytes);
                    return base64String;
                }
                else
                {
                    return "";
                }
            }
        }

        public Image ConvertBase64ToImage(string base64String)
        {
            // Convert base 64 string to byte[]
            byte[] imageBytes = Convert.FromBase64String(base64String);
            // Convert byte[] to Image
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                try
                {
                    Image image = Image.FromStream(ms, true);
                    return image;
                }
                catch(Exception e)
                {
                    return null;
                }
            }
        }

        public string GetMD5(string str)
        {
            MD5 md5 = MD5CryptoServiceProvider.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = md5.ComputeHash(encoding.GetBytes(str));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }

        private void clean_fields()
        {
            tbSOperador.Text = string.Empty;

            tbName.Text = string.Empty;
            tbLastName.Text = string.Empty;
            tbMLastName.Text = string.Empty;
            tbAlias.Text = string.Empty;
            tbEmail.Text = string.Empty;
            tbPhone.Text = string.Empty;
            tbAddress.Text = string.Empty;

            rtbComents.Text = string.Empty;
            rtbMunicipality.Text = string.Empty;

            cbPopulation.Text = string.Empty;
            cbMunicipality.Text = string.Empty;

            chb1.Checked = false;
            chb2.Checked = false;
            chb3.Checked = false;
            chb4.Checked = false;
            chb5.Checked = false;

            pbImagen.BackgroundImage = null;
        }

        private void enable_disable_fields(bool option)
        {
            tbName.Enabled = option;
            tbLastName.Enabled = option;
            tbMLastName.Enabled = option;
            tbAlias.Enabled = option;
            tbEmail.Enabled = option;
            tbPhone.Enabled = option;
            tbAddress.Enabled = option;

            rtbComents.Enabled = option;
            rtbMunicipality.Enabled = option;

            chb1.Enabled = option;
            chb2.Enabled = option;
            chb3.Enabled = option;
            chb4.Enabled = option;
            chb5.Enabled = option;


            cbSection.Enabled = option;
            cbPopulation.Enabled = option;
            cbMunicipality.Enabled = option;

            pbImagen.Enabled = option;
        }

        private void fill_dgv(string search_value)
        {
            dgvOperator.Rows.Clear();
            ConnectionDB db = new ConnectionDB();
            var datos = db.Operators.Join(db.Sections, x => x.id_sections, y => y.id,
                (x, y) => new
                {
                    x.id,
                    x.name,
                    x.alias,
                    x.email,
                    x.phone,
                    x.status,
                    y.section,
                    y.town_name,
                    y.location_name
                }
            ).Where(x => x.status == 1);

            if (!string.IsNullOrEmpty(search_value))
            {
                datos = datos.Where(x=> x.name.Contains(search_value) || 
                                        x.alias.Contains(search_value) || 
                                        x.email.Contains(search_value) || 
                                        x.phone.Contains(search_value) || 
                                        x.section.Contains(search_value) ||
                                        x.town_name.Contains(search_value) ||
                                        x.location_name.Contains(search_value));
            }

            var datos_f = datos.ToList();

            foreach (var item in datos_f)
            {
                dgvOperator.Rows.Add(item.id, item.name, item.alias, item.email, item.phone, item.section, item.town_name, item.location_name);
            }
        }

        private bool check_fields()
        {
            if (string.IsNullOrEmpty(tbName.Text))
            {
                MessageBox.Show("Campo obligatorio", "Operador", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbName.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(tbAlias.Text))
            {
                MessageBox.Show("Campo obligatorio", "Operador", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbName.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(tbPhone.Text))
            {
                MessageBox.Show("Campo obligatorio", "Operador", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbPhone.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(cbSection.Text))
            {
                MessageBox.Show("Campo obligatorio", "Operador", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbSection.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(cbMunicipality.Text))
            {
                MessageBox.Show("Campo obligatorio", "Operador", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbMunicipality.Focus();
                return false;
            }
            if (string.IsNullOrEmpty(cbPopulation.Text))
            {
                MessageBox.Show("Campo obligatorio", "Operador", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cbPopulation.Focus();
                return false;
            }

            return true;
        }

        private void new_buttons()
        {
            //BOTONES
            tsbtDelete.Visible = false;
            tsbtnNuevo.Visible = true;
            tsbtnEdita.Visible = false;
            tsbtnSaveEdit.Visible = false;
            tsbtnSubOperator.Visible = false;

            tsbtnGuarda.Visible = false;
            tsbtnCancela.Visible = false;
        }

        private void excel_import()
        {
            ConnectionDB db = new ConnectionDB();
            OleDbConnection connection;
            OleDbDataAdapter dataAdapter;
            DataTable dataTable = new DataTable();
            DataSet dataSet = new DataSet();
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "Excel 2007|*.xlsx",
                Title = "Open file"
            };

            if(ofd.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(ofd.FileName))
                    return;
                connection = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + ofd.FileName + ";Extended Properties=Excel 12.0;");

                try
                {
                    dataAdapter = new OleDbDataAdapter("SELECT * FROM [Hoja2$]",connection);
                    connection.Open();
                    dataAdapter.Fill(dataSet, "MyData");
                    dataTable = dataSet.Tables["MyData"];
                    foreach(DataRow item in dataTable.Rows)
                    {
                        Operator operators = new Operator();
                        operators.name = Convert.ToString(item[0]);
                        operators.alias = Convert.ToString(item[1]);
                        operators.phone = Convert.ToString(item[2]);
                        operators.email = Convert.ToString(item[3]);
                        operators.observation = Convert.ToString(item[4]);
                        operators.score = Convert.ToInt16(item[5]);
                        operators.status = Convert.ToInt16(item[6]);
                        operators.id_sections = Convert.ToInt16(item[7]);
                        operators.image = Convert.ToString(item[8]);

                        db.Operators.Add(operators);
                        db.SaveChanges();
                    }

                    MessageBox.Show("Proceso terminado", "Operador", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    fill_dgv("");
                }
                catch(Exception e)
                {

                }
            }
        }

        private void excel_export()
        {
            DialogResult result = MessageBox.Show("¿Esta seguro que desea exportar sus datos de operador?", "Operador", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (result != DialogResult.Yes)
            {
                return;
            }

            int i = 2;
            ConnectionDB db = new ConnectionDB();
            SaveFileDialog saveFile = new SaveFileDialog {
                Title = "Guardar como",
                Filter = "Excel |*.xlsx"
            };

            saveFile.ShowDialog();
            if (string.IsNullOrEmpty(saveFile.FileName))
            {
                MessageBox.Show("Nombre no válido", "Operador", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Microsoft.Office.Interop.Excel.Application application = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook workbook;
            Microsoft.Office.Interop.Excel.Worksheet worksheet;

            workbook = application.Workbooks.Add();
            worksheet = workbook.Worksheets.Add();

            List<Operator> operators = db.Operators.Where(x => x.status == 1).ToList();

            worksheet.Cells.Item[1, 1] = "Nombre";
            worksheet.Cells.Item[1, 2] = "Alias";
            worksheet.Cells.Item[1, 3] = "Teléfono";
            worksheet.Cells.Item[1, 4] = "Correo";
            worksheet.Cells.Item[1, 5] = "Observaciones";
            worksheet.Cells.Item[1, 6] = "Puntaje";
            worksheet.Cells.Item[1, 7] = "Status";
            worksheet.Cells.Item[1, 8] = "id_secciones";
            worksheet.Cells.Item[1, 9] = "imagen";

            foreach(Operator item in operators)
            {
                worksheet.Cells.Item[i, 1] = item.name;
                worksheet.Cells.Item[i, 2] = item.alias;
                worksheet.Cells.Item[i, 3] = item.phone;
                worksheet.Cells.Item[i, 4] = item.email;
                worksheet.Cells.Item[i, 5] = item.observation;
                worksheet.Cells.Item[i, 6] = item.score;
                worksheet.Cells.Item[i, 7] = item.status;
                worksheet.Cells.Item[i, 8] = item.id_sections;
                worksheet.Cells.Item[i, 9] = item.image;

                i = i + 1;
            }

            workbook.SaveAs(saveFile.FileName);

            application.Application.Visible = false;
            workbook.Close();

            MessageBox.Show("Proceso terminado", "Operador", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void csv_export()
        {
            DialogResult result = MessageBox.Show("¿Esta seguro que desea exportar sus datos de operador?", "Operador", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (result != DialogResult.Yes)
            {
                return;
            }

            string text;
            ConnectionDB db = new ConnectionDB();
            SaveFileDialog saveFile = new SaveFileDialog
            {
                Title = "Guardar como",
                Filter = "CSV Files (*.csv)|*.csv"
            };

            saveFile.ShowDialog();
            if (string.IsNullOrEmpty(saveFile.FileName))
            {
                MessageBox.Show("Nombre no válido", "Operador", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<Operator> operators = db.Operators.Where(x => x.status == 1).ToList();

            foreach (Operator item in operators)
            {
                text = item.name
                    + "," + item.last_name
                    + "," + item.m_last_name
                    + "," + item.alias 
                    + "," + item.phone
                    + "," + item.email
                    + "," + item.address
                    + "," + item.observation
                    + "," + item.municipality
                    + "," + item.score
                    + "," + item.status
                    + "," + item.id_sections
                    + "," + item.image
                    + "," + item.id_operators_key
                    + "," + item.operators_key
                    + System.Environment.NewLine;
                File.AppendAllText(saveFile.FileName, text);
            }

            MessageBox.Show("Proceso terminado", "Operador", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void csv_import()
        {
            ConnectionDB db = new ConnectionDB();
            OpenFileDialog ofd = new OpenFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv",
                Title = "Open file"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(ofd.FileName))
                    return;

                using (var reader = new StreamReader(ofd.FileName))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var item = line.Split(',');

                        Operator operators = new Operator();
                        operators.name = Convert.ToString(item[0]);
                        operators.last_name = Convert.ToString(item[1]);
                        operators.m_last_name = Convert.ToString(item[2]);
                        operators.alias = Convert.ToString(item[3]);
                        operators.phone = Convert.ToString(item[4]);
                        operators.email = Convert.ToString(item[5]);
                        operators.address = Convert.ToString(item[6]);
                        operators.observation = Convert.ToString(item[7]);
                        operators.municipality = Convert.ToString(item[8]);
                        operators.score = Convert.ToInt16(item[9]);
                        operators.status = Convert.ToInt16(item[10]);
                        operators.id_sections = Convert.ToInt16(item[11]);
                        operators.image = Convert.ToString(item[12]);
                        operators.id_operators_key = Convert.ToString(item[13]);
                        operators.operators_key = Convert.ToString(item[14]);

                        db.Operators.Add(operators);
                        db.SaveChanges();
                    }
                }
            }

            MessageBox.Show("Proceso terminado", "Operador", MessageBoxButtons.OK, MessageBoxIcon.Information);
            fill_dgv("");
        }
        #endregion

        #region BOTONES
        private void tsbtnNuevo_Click(object sender, EventArgs e)
        {
            enable_disable_fields(true);
            clean_fields();
            image_click = false;
            id = 0;

            //BOTONES
            tsbtDelete.Visible = false;
            tsbtnNuevo.Visible = false;
            tsbtnEdita.Visible = false;
            tsbtnSaveEdit.Visible = false;
            tsbtnSubOperator.Visible = false;

            tsbtnGuarda.Visible = true;
            tsbtnCancela.Visible = true;
        }

        private void tsbtnCancela_Click(object sender, EventArgs e)
        {
            clean_fields();
            enable_disable_fields(false);
            new_buttons();
        }

        private void tsbtnGuarda_Click(object sender, EventArgs e)
        {
            if (!check_fields())
            {
                return;
            }

            ConnectionDB db = new ConnectionDB();

            Section sections = db.Sections.Where(x => x.section == cbSection.Text && x.town_name == cbMunicipality.Text && x.location_name == cbPopulation.Text)
                            .FirstOrDefault();

            Operator operators = new Operator();
            operators.name = tbName.Text;
            operators.last_name = tbLastName.Text;
            operators.m_last_name = tbMLastName.Text;
            operators.alias = tbAlias.Text;
            operators.email = tbEmail.Text;
            operators.phone = tbPhone.Text;
            operators.address = tbAddress.Text;
            operators.observation = rtbComents.Text;
            operators.municipality = rtbMunicipality.Text;
            operators.score = check_score();
            operators.id_sections = sections.id;
            operators.status = 1;
            operators.image = ConvertImageToBase64(pbImagen.BackgroundImage);
            operators.operators_key = GetMD5(tbName.Text + "-" + tbLastName.Text + "-" + tbMLastName.Text + "-" + tbAlias.Text);

            db.Operators.Add(operators);
            db.SaveChanges();

            MessageBox.Show("Registro realizado con éxito", "Operador", MessageBoxButtons.OK, MessageBoxIcon.Information);
            clean_fields();
            enable_disable_fields(false);
            new_buttons();
            fill_dgv("");            
        }

        private void tsbtnEdita_Click(object sender, EventArgs e)
        {
            enable_disable_fields(true);
            image_click = false;

            //BOTONES
            tsbtDelete.Visible = false;
            tsbtnNuevo.Visible = false;
            tsbtnEdita.Visible = false;
            tsbtnSaveEdit.Visible = true;
            tsbtnSubOperator.Visible = false;

            tsbtnGuarda.Visible = false;
            tsbtnCancela.Visible = true;
        }

        private void tsbtnSaveEdit_Click(object sender, EventArgs e)
        {
            ConnectionDB db = new ConnectionDB();

            if (!check_fields())
            {
                return;
            }

            Operator operators = db.Operators.Where(x => x.id == id).FirstOrDefault();
            Section sections = db.Sections.Where(x => x.section == cbSection.Text && x.town_name == cbMunicipality.Text && x.location_name == cbPopulation.Text)
                            .FirstOrDefault();

            operators.name = tbName.Text;
            operators.last_name = tbLastName.Text;
            operators.m_last_name = tbMLastName.Text;
            operators.alias = tbAlias.Text;
            operators.email = tbEmail.Text;
            operators.phone = tbPhone.Text;
            operators.address = tbAddress.Text;
            operators.observation = rtbComents.Text;
            operators.municipality = rtbMunicipality.Text;
            operators.score = check_score();
            operators.id_sections = sections.id;
            operators.status = 1;
            if (image_click)
            {
                operators.image = ConvertImageToBase64(pbImagen.BackgroundImage);
            }
            db.Entry(operators).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            MessageBox.Show("Registro actualizado con éxito", "Operador", MessageBoxButtons.OK, MessageBoxIcon.Information);
            clean_fields();
            enable_disable_fields(false);
            new_buttons();
            fill_dgv("");
        }

        private void tsbtDelete_Click_1(object sender, EventArgs e)
        {
            ConnectionDB db = new ConnectionDB();
            DialogResult result = MessageBox.Show("¿Esta seguro que desea eliminar este registro?", "Operador", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                Operator operators = db.Operators.Where(x => x.id == id).FirstOrDefault();
                operators.status = 0;
                db.Entry(operators).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                MessageBox.Show("Registro eliminado con éxito", "Operador", MessageBoxButtons.OK, MessageBoxIcon.Information);
                clean_fields();
                enable_disable_fields(false);
                new_buttons();
                fill_dgv("");
            }
        }

        private void dgvOperator_DoubleClick(object sender, EventArgs e)
        {
            if (dgvOperator.Rows.Count == 0)
                return;

            ConnectionDB db = new ConnectionDB();
            int index = dgvOperator.CurrentRow.Index;
            id = Convert.ToInt16(dgvOperator.Rows[index].Cells[0].Value.ToString());

            Operator @operator = db.Operators.Where(x => x.id == id).FirstOrDefault();
            Operator s_operator = db.Operators.Where(x => x.operators_key == @operator.id_operators_key).FirstOrDefault();

            Section section = db.Sections.Where(x => x.id == @operator.id_sections).FirstOrDefault();

            pbImagen.BackgroundImage = ConvertBase64ToImage(@operator.image);
            tbName.Text = @operator.name;
            tbLastName.Text = @operator.last_name;
            tbMLastName.Text = @operator.m_last_name;
            tbAlias.Text = @operator.alias;
            tbEmail.Text = @operator.email;
            tbPhone.Text = @operator.phone;
            tbAddress.Text = @operator.address;
            rtbComents.Text = @operator.observation;
            rtbMunicipality.Text = @operator.municipality;

            switch (@operator.score)
            {
                case 1:
                    chb1.Checked = true;
                    break;
                case 2:
                    chb2.Checked = true;
                    break;
                case 3:
                    chb3.Checked = true;
                    break;
                case 4:
                    chb4.Checked = true;
                    break;
                case 5:
                    chb5.Checked = true;
                    break;
            }

            cbMunicipality.Text = section.town_name;
            cbSection.Text = section.section;            
            cbPopulation.Text = section.location_name;

            if(s_operator != null)
            {
                tbSOperador.Text = s_operator.name + " " + s_operator.last_name + " " + s_operator.m_last_name;
            }
            else
            {
                tbSOperador.Text = string.Empty;
            }

            if (user_kind == 1)
            {
                //BOTONES
                tsbtDelete.Visible = true;
                tsbtnNuevo.Visible = true;
                tsbtnEdita.Visible = true;
                tsbtnSaveEdit.Visible = false;
                tsbtnSubOperator.Visible = true;

                tsbtnGuarda.Visible = false;
                tsbtnCancela.Visible = false;
            }

        }

        private void tsbtnImport_Click(object sender, EventArgs e)
        {
            csv_import();
        }

        private void tsbtnExport_Click(object sender, EventArgs e)
        {
            csv_export();
        }

        private void stbtSearch_Click(object sender, EventArgs e)
        {
            fill_dgv(tsbtnSearch.Text);
        }
        #endregion

        #region COMBO BOX
        private void cbMunicipality_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbSection.Text = string.Empty;
            cbPopulation.Text = string.Empty;
        }

        private void cbSection_Click(object sender, EventArgs e)
        {
            ConnectionDB db = new ConnectionDB();
            cbSection.Items.Clear();
            cbPopulation.Items.Clear();

            List<string> section = db.Sections.Where(x => x.town_name == cbMunicipality.Text).Select(x => x.section).Distinct().ToList();

            foreach(string item in section)
            {
                cbSection.Items.Add(item);
            }
        }

        private void cbPopulation_Click(object sender, EventArgs e)
        {
            cbPopulation.Items.Clear();
            ConnectionDB db = new ConnectionDB();

            List<string> population = db.Sections.Where(x => x.section == cbSection.Text && x.town_name == cbMunicipality.Text).
                Select(x => x.location_name).ToList();

            foreach (string item in population)
            {
                cbPopulation.Items.Add(item);
            }
        }

        #endregion

        #region CHECKBOX
        private void chb1_CheckedChanged(object sender, EventArgs e)
        {
            chb2.Checked = false;
            chb3.Checked = false;
            chb4.Checked = false;
            chb5.Checked = false;
        }

        private void chb2_CheckedChanged(object sender, EventArgs e)
        {
            chb1.Checked = false;
            chb3.Checked = false;
            chb4.Checked = false;
            chb5.Checked = false;
        }

        private void chb3_CheckedChanged(object sender, EventArgs e)
        {
            chb1.Checked = false;
            chb2.Checked = false;
            chb4.Checked = false;
            chb5.Checked = false;
        }

        private void chb4_CheckedChanged(object sender, EventArgs e)
        {
            chb1.Checked = false;
            chb2.Checked = false;
            chb3.Checked = false;
            chb5.Checked = false;
        }

        private void chb5_CheckedChanged(object sender, EventArgs e)
        {
            chb1.Checked = false;
            chb2.Checked = false;
            chb3.Checked = false;
            chb4.Checked = false;
        }
        #endregion

        private void tsbtnSubOperator_Click(object sender, EventArgs e)
        {
            ConnectionDB db = new ConnectionDB();

            string operators_key = db.Operators.Where(x => x.id == id).Select(x => x.operators_key).FirstOrDefault();

            scrSubOperator form = new scrSubOperator();
            form.operators_key = operators_key;
            form.ShowDialog();
            form.BringToFront();
        }
    }
}
