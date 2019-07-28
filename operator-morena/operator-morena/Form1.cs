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
using operator_morena.Connection;
using operator_morena.Models;

namespace operator_morena
{
    public partial class scrLogin : MaterialForm
    {
        private wfDashBoard nextWidnows;
        public scrLogin()
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

            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.nextWidnows = new wfDashBoard();

        }

        private void BtLogin_Click(object sender, EventArgs e)
        {
            ConnectionDB db = new ConnectionDB();
            var datos = db.Users.ToList();

            Console.WriteLine("Entrando al sistema --> [" + tbPassword.Text + "]");
            this.nextWidnows.Show();
            this.nextWidnows.BringToFront();
            this.Hide();
        }
    }
}
