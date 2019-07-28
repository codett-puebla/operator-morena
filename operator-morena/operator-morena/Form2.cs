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

namespace operator_morena
{
    public partial class wfDashBoard : MaterialForm
    {
        GMarkerGoogle marker;
        GMapOverlay mapOverlay;

        double LatIncial = 19.043719;
        double LngIncial = -98.198911;


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
                pbImagen.Image = Image.FromFile(image_name);
            }


        }

        private void wfDashBoard_Load(object sender, EventArgs e)
        {
            gMapControl1.DragButton = MouseButtons.Left;
            gMapControl1.CanDragMap = true;
            gMapControl1.MapProvider = GMapProviders.GoogleMap;
            gMapControl1.Position = new PointLatLng(LatIncial, LngIncial);
            gMapControl1.MinZoom = 0;
            gMapControl1.MaxZoom = 24;
            gMapControl1.Zoom = 9;
            gMapControl1.AutoScroll = true;
        }
    }
}
