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

        }

        private void BtLogin_Click(object sender, EventArgs e)
        {
            //VALIDAMOS QUE EL CAMPO tbPassword NO ESTE VACÍO
            if (String.IsNullOrEmpty(tbPassword.Text))
            {
                MessageBox.Show("Campo obligatorio","Operador",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                tbPassword.Focus();
                return;
            }

            ConnectionDB db = new ConnectionDB();

            //COMPROBAMOS QUE EL USUARIO EXISTA EN BASE DE DATOS
            Users users = db.Users.Where(x=> x.password == tbPassword.Text).FirstOrDefault();
            if(users == null)
            {
                MessageBox.Show("El usuario no existe", "Operador", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbPassword.Focus();
                return;
            }
            
            Console.WriteLine("Entrando al sistema --> [" + tbPassword.Text + "]");
            wfDashBoard wf = new wfDashBoard();
            wf.user_kind = users.user_kind;
            wf.Show();
            wf.BringToFront();
            this.Hide();
        }
    }
}
