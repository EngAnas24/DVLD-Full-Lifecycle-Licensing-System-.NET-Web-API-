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

namespace DVLD.Users
{
    public partial class frmChangePassword : Form
    {
        private int _UserID;
        private readonly UserService _UserService;
        private static readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient
        {
            BaseAddress = new Uri("http://localhost:5067/") // تأكد من وجود الـ / في النهاية
        };
        public frmChangePassword(int UserID)
        {
            InitializeComponent();
            _UserID = UserID;
            _UserService = new UserService(_httpClient);
        }
        private void frmChangePassword_Load(object sender, EventArgs e)
        {
            ctrlUserCard1.LoadUserInfo(_UserID);
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtConfirmPassword.Text != txtNewPassword.Text)
                {
                    MessageBox.Show("يجب ان تتطابق تاكيد كلمة السر مع كلمة السر الجديدة");
                    return;
                }

                await _UserService.ChangePasswordAsync(_UserID, txtCurrentPassword.Text, txtNewPassword.Text);
                MessageBox.Show("تم تغيير كلمة السر بنجاح");
                clsGlobal.RememberUserNameAndPassword(clsGlobal.GetUser.UserName, txtNewPassword.Text, true);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
