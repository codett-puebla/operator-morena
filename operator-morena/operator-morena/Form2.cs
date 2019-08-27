using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using Google.Maps;
using Google.Maps.Geocoding;
using MaterialSkin;
using MaterialSkin.Controls;
using Microsoft.Office.Interop.Excel;
using operator_morena.Connection;
using operator_morena.Models;
using Application = System.Windows.Forms.Application;
using DataTable = System.Data.DataTable;
using Placemark = GMap.NET.Placemark;

namespace operator_morena
{
    public partial class wfDashBoard : MaterialForm
    {
        private readonly string APIKEY = "AIzaSyCb9_Q9RXAwaDni9Uq0hgVcFPSJeMwoLek";
        private int id;
        private bool image_click;
        private readonly double LatIncial = 19.043719;
        private double lenght, latitude;
        private readonly double LngIncial = -98.198911;
        public int user_kind;


        public wfDashBoard()
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

        private void WfDashBoard_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.ExitThread();
            Application.Exit();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            string image_name;
            var ofd = new OpenFileDialog
            {
                Filter = "Archivos de Imagen JPG|*.jpg|Todos los archivos|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (ofd.ShowDialog() == DialogResult.OK)
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
            //MAPA
            gMapControl1.DragButton = MouseButtons.Left;
            gMapControl1.CanDragMap = true;
            gMapControl1.MapProvider = GMapProviders.GoogleMap;
            gMapControl1.Position = new PointLatLng(LatIncial, LngIncial);
            gMapControl1.MinZoom = 0;
            gMapControl1.MaxZoom = 24;
            gMapControl1.Zoom = 20;
            gMapControl1.AutoScroll = true;

            GoogleSigned.AssignAllServices(new GoogleSigned(APIKEY));

            //DEPENDIENDO EL TIPO DE USUARIO MOSTRAMOS LOS BOTONES CORRESPONDIENTES
            if (user_kind == 1)
                tsbtnNuevo.Visible = true;
            else
                tsbtnNuevo.Visible = false;

            var db = new ConnectionDB();

            var sections = db.Sections.Select(x => x.town_name).Distinct().ToList();
            cbMunicipality.Items.Clear();
            cbMunicipality.DataSource = sections;
            fill_dgv("");
        }

        private void tsbtnSubOperator_Click(object sender, EventArgs e)
        {
            var db = new ConnectionDB();

            var operators_key = db.Operators.Where(x => x.id == id).Select(x => x.operators_key).FirstOrDefault();

            var form = new scrSubOperator();
            form.operators_key = operators_key;
            form.ShowDialog();
            form.BringToFront();
        }

        private async void btnSearchAddress_Click(object sender, EventArgs e)
        {
            var request = new GeocodingRequest();
            request.Address = tbAddress.Text;

            try
            {
                var response = await new GeocodingService().GetResponseAsync(request);

                if (response.Status == ServiceResponseStatus.Ok)
                    foreach (var item in response.Results)
                    {
                        set_map_point(item.Geometry.Location.Latitude, item.Geometry.Location.Longitude);
                        latitude = item.Geometry.Location.Latitude;
                        lenght = item.Geometry.Location.Longitude;
                        break;
                    }
            }
            catch
            {
                latitude = 0;
                lenght = 0;
                gMapControl1.Position = new PointLatLng(LatIncial, LngIncial);
                gMapControl1.Overlays.Clear();
            }
        }

        private void gMapControl1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                List<Placemark> placemarks = null;
                var point = gMapControl1.FromLocalToLatLng(e.X, e.Y);
                var lat = point.Lat;
                var lng = point.Lng;
                latitude = lat;
                lenght = lng;

                gMapControl1.Position = point;

                GMapProviders.GoogleMap.ApiKey = APIKEY;
                set_map_point(lat, lng);

                var statusCode = GMapProviders.GoogleMap.GetPlacemarks(point, out placemarks);
                if (statusCode == GeoCoderStatusCode.OK)
                    foreach (var item in placemarks)
                    {
                        tbAddress.Text = item.Address;
                        break;
                    }
            }
        }


        #region FUNCIONES

        private void web_browser_config(string district, string section)
        {
            var url = "https://www.iee-puebla.org.mx/2016/CARTOGRAFIA%20LOCAL%20ABRIL%202016/PSI%20";
            if (Convert.ToInt32(district) < 9) district = "0" + district;

            url = url + district + "/PSI21" + district + section + ".pdf";

            var path = AppDomain.CurrentDomain.BaseDirectory + "Files";

            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            if (!File.Exists(path + @"\PSI21" + district + section + ".pdf"))
            {
                var web = new WebClient();
                try
                {
                    web.DownloadFile(url, path + @"\PSI21" + district + section + ".pdf");
                    wbSecciones1.Navigate("file:///" + path + @"\PSI21" + district + section + ".pdf");
                }
                catch
                {
                    wbSecciones1.Navigate("about:blank");
                }
            }
            else
            {
                wbSecciones1.Navigate("file:///" + path + @"\PSI21" + district + section + ".pdf");
            }
        }

        private void set_map_point(double Latitude, double Longitude)
        {
            var point = new PointLatLng(Latitude, Longitude);
            GMapMarker marker = new GMarkerGoogle(point, GMarkerGoogleType.red);

            var markers = new GMapOverlay("markers");

            markers.Markers.Add(marker);

            gMapControl1.Position = point;
            gMapControl1.Overlays.Clear();
            gMapControl1.Overlays.Add(markers);
        }

        private byte[] ConvertImageToBinary(Image img)
        {
            using (var ms = new MemoryStream())
            {
                if (img != null)
                {
                    img.Save(ms, ImageFormat.Jpeg);
                    return ms.ToArray();
                }

                return ms.ToArray();
            }
        }

        private Image ConvertBinaryToImage(byte[] data)
        {
            using (var ms = new MemoryStream(data))
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
            using (var m = new MemoryStream())
            {
                if (image != null)
                {
                    image.Save(m, image.RawFormat);
                    var imageBytes = m.ToArray();

                    // Convert byte[] to Base64 String
                    var base64String = Convert.ToBase64String(imageBytes);
                    return base64String;
                }

                return "";
            }
        }

        public Image ConvertBase64ToImage(string base64String)
        {
            // Convert base 64 string to byte[]
            var imageBytes = Convert.FromBase64String(base64String);
            // Convert byte[] to Image
            using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
            {
                try
                {
                    var image = Image.FromStream(ms, true);
                    return image;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }

        public string GetMD5(string str)
        {
            var md5 = MD5.Create();
            var encoding = new ASCIIEncoding();
            byte[] stream = null;
            var sb = new StringBuilder();
            stream = md5.ComputeHash(encoding.GetBytes(str));
            for (var i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
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

            rater1.CurrentRating = 0;
            lbNPoblacion.Text = "";
            lbNSecciones.Text = "";

            cbPopulation.Text = string.Empty;
            cbMunicipality.Text = string.Empty;
            cbSection.Text = string.Empty;

            pbImagen.BackgroundImage = null;
            wbSecciones1.Navigate("about:blank");
            gMapControl1.Overlays.Clear();
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
            btnSearchAddress.Enabled = option;

            rater1.Enabled = option;
            gMapControl1.Enabled = option;

            cbSection.Enabled = option;
            cbPopulation.Enabled = option;
            cbMunicipality.Enabled = option;

            pbImagen.Enabled = option;
        }

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
                    y.section,
                    y.town_name,
                    y.location_name
                }
            ).Where(x => x.status == 1);

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
            var db = new ConnectionDB();
            OleDbConnection connection;
            OleDbDataAdapter dataAdapter;
            var dataTable = new DataTable();
            var dataSet = new DataSet();
            var ofd = new OpenFileDialog
            {
                Filter = "Excel 2007|*.xlsx",
                Title = "Open file"
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                if (string.IsNullOrEmpty(ofd.FileName))
                    return;
                connection = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + ofd.FileName +
                                                 ";Extended Properties=Excel 12.0;");

                try
                {
                    dataAdapter = new OleDbDataAdapter("SELECT * FROM [Hoja2$]", connection);
                    connection.Open();
                    dataAdapter.Fill(dataSet, "MyData");
                    dataTable = dataSet.Tables["MyData"];
                    foreach (DataRow item in dataTable.Rows)
                    {
                        var operators = new Operator();
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
                catch (Exception e)
                {
                }
            }
        }

        private void excel_export()
        {
            var result = MessageBox.Show("¿Esta seguro que desea exportar sus datos de operador?", "Operador",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (result != DialogResult.Yes) return;

            var i = 2;
            var db = new ConnectionDB();
            var saveFile = new SaveFileDialog
            {
                Title = "Guardar como",
                Filter = "Excel |*.xlsx"
            };

            saveFile.ShowDialog();
            if (string.IsNullOrEmpty(saveFile.FileName))
            {
                MessageBox.Show("Nombre no válido", "Operador", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var application = new Microsoft.Office.Interop.Excel.Application();
            Workbook workbook;
            Worksheet worksheet;

            workbook = application.Workbooks.Add();
            worksheet = workbook.Worksheets.Add();

            var operators = db.Operators.Where(x => x.status == 1).ToList();

            worksheet.Cells.Item[1, 1] = "Nombre";
            worksheet.Cells.Item[1, 2] = "Alias";
            worksheet.Cells.Item[1, 3] = "Teléfono";
            worksheet.Cells.Item[1, 4] = "Correo";
            worksheet.Cells.Item[1, 5] = "Observaciones";
            worksheet.Cells.Item[1, 6] = "Puntaje";
            worksheet.Cells.Item[1, 7] = "Status";
            worksheet.Cells.Item[1, 8] = "id_secciones";
            worksheet.Cells.Item[1, 9] = "imagen";

            foreach (var item in operators)
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
            var result = MessageBox.Show("¿Esta seguro que desea exportar sus datos de operador?", "Operador",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (result != DialogResult.Yes) return;

            string text;
            var db = new ConnectionDB();
            var saveFile = new SaveFileDialog
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

            var operators = db.Operators.Where(x => x.status == 1).ToList();

            foreach (var item in operators)
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
                       + "," + item.latitude
                       + "," + item.length
                       + Environment.NewLine;
                File.AppendAllText(saveFile.FileName, text);
            }

            MessageBox.Show("Proceso terminado", "Operador", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void csv_import()
        {
            var db = new ConnectionDB();
            var ofd = new OpenFileDialog
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

                        var operators = new Operator();
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
                        operators.latitude = Convert.ToString(item[15]);
                        operators.length = Convert.ToString(item[16]);

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
            latitude = 0;
            lenght = 0;

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
            if (!check_fields()) return;

            var db = new ConnectionDB();

            var sections = db.Sections.Where(x =>
                    x.section == cbSection.Text && x.town_name == cbMunicipality.Text &&
                    x.location_name == cbPopulation.Text)
                .FirstOrDefault();

            var operators = new Operator();
            operators.name = tbName.Text;
            operators.last_name = tbLastName.Text;
            operators.m_last_name = tbMLastName.Text;
            operators.alias = tbAlias.Text;
            operators.email = tbEmail.Text;
            operators.phone = tbPhone.Text;
            operators.address = Convert.ToString(tbAddress.Text).Replace(",", "");
            operators.observation = rtbComents.Text;
            operators.municipality = rtbMunicipality.Text;
            operators.score = rater1.CurrentRating;
            operators.id_sections = sections.id;
            operators.status = 1;
            operators.image = ConvertImageToBase64(pbImagen.BackgroundImage);
            operators.operators_key =
                GetMD5(tbName.Text + "-" + tbLastName.Text + "-" + tbMLastName.Text + "-" + tbAlias.Text);
            operators.latitude = Convert.ToString(latitude);
            operators.length = Convert.ToString(lenght);

            db.Operators.Add(operators);
            db.SaveChanges();

            MessageBox.Show("Registro realizado con éxito", "Operador", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
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
            var db = new ConnectionDB();

            if (!check_fields()) return;

            var operators = db.Operators.Where(x => x.id == id).FirstOrDefault();
            var sections = db.Sections.Where(x =>
                    x.section == cbSection.Text && x.town_name == cbMunicipality.Text &&
                    x.location_name == cbPopulation.Text)
                .FirstOrDefault();

            operators.name = tbName.Text;
            operators.last_name = tbLastName.Text;
            operators.m_last_name = tbMLastName.Text;
            operators.alias = tbAlias.Text;
            operators.email = tbEmail.Text;
            operators.phone = tbPhone.Text;
            operators.address = Convert.ToString(tbAddress.Text).Replace(",", "");
            operators.observation = rtbComents.Text;
            operators.municipality = rtbMunicipality.Text;
            operators.score = rater1.CurrentRating;
            operators.id_sections = sections.id;
            operators.status = 1;
            operators.length = Convert.ToString(lenght);
            operators.latitude = Convert.ToString(latitude);

            if (image_click) operators.image = ConvertImageToBase64(pbImagen.BackgroundImage);
            db.Entry(operators).State = EntityState.Modified;
            db.SaveChanges();

            MessageBox.Show("Registro actualizado con éxito", "Operador", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            clean_fields();
            enable_disable_fields(false);
            new_buttons();
            fill_dgv("");
        }

        private void tsbtDelete_Click_1(object sender, EventArgs e)
        {
            var db = new ConnectionDB();
            var result = MessageBox.Show("¿Esta seguro que desea eliminar este registro?", "Operador",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                var operators = db.Operators.Where(x => x.id == id).FirstOrDefault();
                operators.status = 0;
                db.Entry(operators).State = EntityState.Modified;
                db.SaveChanges();

                MessageBox.Show("Registro eliminado con éxito", "Operador", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
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

            var db = new ConnectionDB();
            var index = dgvOperator.CurrentRow.Index;
            id = Convert.ToInt16(dgvOperator.Rows[index].Cells[0].Value.ToString());

            var @operator = db.Operators.Where(x => x.id == id).FirstOrDefault();
            var s_operator = db.Operators.Where(x => x.operators_key == @operator.id_operators_key).FirstOrDefault();

            var section = db.Sections.Where(x => x.id == @operator.id_sections).FirstOrDefault();

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
            rater1.CurrentRating = @operator.score;

            cbMunicipality.Text = section.town_name;
            cbSection.Text = section.section;
            cbPopulation.Text = section.location_name;

            latitude = Convert.ToDouble(@operator.latitude);
            lenght = Convert.ToDouble(@operator.length);

            gMapControl1.Position = new PointLatLng(latitude, lenght);
            set_map_point(latitude, lenght);

            if (s_operator != null)
                tbSOperador.Text = s_operator.name + " " + s_operator.last_name + " " + s_operator.m_last_name;
            else
                tbSOperador.Text = string.Empty;

            lbNSecciones.Text = db.Sections.Where(x => x.town_name == cbMunicipality.Text).Select(x => x.section)
                .Distinct().Count().ToString();
            lbNPoblacion.Text = db.Sections
                .Where(x => x.section == cbSection.Text && x.town_name == cbMunicipality.Text)
                .Select(x => x.location_name).Distinct().Count().ToString();
            var distrito_seccion = db.Sections
                .Where(x => x.section == cbSection.Text && x.town_name == cbMunicipality.Text)
                .Select(x => new {x.district, x.section}).Distinct().FirstOrDefault();
            web_browser_config(distrito_seccion.district, distrito_seccion.section);

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
            var db = new ConnectionDB();
            cbSection.Text = string.Empty;
            cbPopulation.Text = string.Empty;

            lbNSecciones.Text = db.Sections.Where(x => x.town_name == cbMunicipality.Text).Select(x => x.section)
                .Distinct().Count().ToString();
            lbNPoblacion.Text = "";
        }

        private void cbSection_Click(object sender, EventArgs e)
        {
            var db = new ConnectionDB();
            cbSection.Items.Clear();
            cbPopulation.Items.Clear();

            var section = db.Sections.Where(x => x.town_name == cbMunicipality.Text).Select(x => x.section).Distinct()
                .ToList();

            foreach (var item in section) cbSection.Items.Add(item);
        }

        private void cbSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            var db = new ConnectionDB();

            if (string.IsNullOrEmpty(cbSection.Text)) return;
            lbNPoblacion.Text = db.Sections
                .Where(x => x.section == cbSection.Text && x.town_name == cbMunicipality.Text)
                .Select(x => x.location_name).Distinct().Count().ToString();

            var distrito_seccion = db.Sections
                .Where(x => x.section == cbSection.Text && x.town_name == cbMunicipality.Text)
                .Select(x => new {x.district, x.section}).Distinct().FirstOrDefault();
            web_browser_config(distrito_seccion.district, distrito_seccion.section);
        }

        private void cbPopulation_Click(object sender, EventArgs e)
        {
            cbPopulation.Items.Clear();
            var db = new ConnectionDB();

            var population = db.Sections.Where(x => x.section == cbSection.Text && x.town_name == cbMunicipality.Text)
                .Select(x => x.location_name).ToList();

            foreach (var item in population) cbPopulation.Items.Add(item);
        }

        #endregion
    }
}