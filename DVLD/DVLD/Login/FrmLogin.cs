using DVLDServices.GlobalClasses;
using DVLDServices.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DVLDServices.Services.UserService;

namespace DVLD.Login
{
    public partial class FrmLogin : Form
    {
        private UserDto _user; 
        private readonly UserService _userService;
        private static readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient
        {
            BaseAddress = new Uri("http://localhost:5067/") 
        };
        public FrmLogin()
        {
            InitializeComponent();
            _userService = new UserService(_httpClient);
        }
        private void FrmLogin_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        public void LoadData()
        {
            if (clsGlobal.GetStoredUserData())
            {
                txtUserName.Text = clsGlobal.GetUser.UserName;
                txtPassword.Text = clsGlobal.GetUser.Password;
            }
 
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUserName.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("الرجاء إدخال اسم المستخدم وكلمة المرور.");
                return;
            }

            try
            {
                btnLogin.Enabled = false;
                var loginResult = await _userService.GetUserByNameAndPasswordAsync(txtUserName.Text, txtPassword.Text);

                if (loginResult != null)
                {
                    if (!loginResult.IsActive)
                    {
                        MessageBox.Show("هذا الحساب معطل، يرجى مراجعة المسؤول.");
                        return;
                    }

                    clsGlobal.GetUser = loginResult;

                    clsGlobal.RememberUserNameAndPassword(txtUserName.Text, txtPassword.Text, chkRememberMe.Checked);

                    FrmMain main = new FrmMain(clsGlobal.GetUser.ID);

                    main.FormClosed += (s, args) => {
                        this.Close();
                    };

                    main.Show();
                    this.Hide(); 
                }
                else
                {
                    MessageBox.Show("اسم المستخدم أو كلمة المرور غير صحيحة.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ: {ex.Message}");
            }
            finally
            {
                btnLogin.Enabled = true;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
