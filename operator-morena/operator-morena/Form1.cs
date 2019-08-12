using System;
using System.Linq;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using operator_morena.Connection;

namespace operator_morena
{
    public partial class scrLogin : MaterialForm
    {
        public scrLogin()
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

            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void BtLogin_Click(object sender, EventArgs e)
        {
            //VALIDAMOS QUE EL CAMPO tbPassword NO ESTE VACÍO
            if (string.IsNullOrEmpty(tbPassword.Text))
            {
                MessageBox.Show("Campo obligatorio", "Operador", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbPassword.Focus();
                return;
            }

            var db = new ConnectionDB();

            //COMPROBAMOS QUE EL USUARIO EXISTA EN BASE DE DATOS
            var users = db.Users.Where(x => x.password == tbPassword.Text).FirstOrDefault();
            if (users == null)
            {
                MessageBox.Show("El usuario no existe", "Operador", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tbPassword.Focus();
                return;
            }

            Console.WriteLine("Entrando al sistema --> [" + tbPassword.Text + "]");
            var wf = new wfDashBoard();
            wf.user_kind = users.user_kind;
            wf.Show();
            wf.BringToFront();
            Hide();
        }
    }
}