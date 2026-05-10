using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DVLDServices.Services;
using DVLD.Properties;
using System.IO;
using static DVLDServices.Services.UserService;
using DVLDServices.Services;
using DVLD.Peoples.Controls;

namespace DVLD.Users.Controls
{
    public partial class ctrlUserCard : UserControl
    {
        private UserDto _User;
        private readonly UserService _userService;

        private static readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient
        {
            BaseAddress = new Uri("http://localhost:5067/") // تأكد من وجود الـ / في النهاية
        };
        private int _UserID = -1;

        public int UserID
        {
            get { return _UserID; }
        }

        public UserDto SelectedUserInfo
        {
            get { return _User; }
        }

        public ctrlUserCard()
        {
            InitializeComponent();
            _userService = new UserService(_httpClient);
        }

        public async void LoadUserInfo(int UserID)
        {
            _UserID = UserID;
            _User = await _userService.GetUserByIdAsync(UserID);
            if (_User == null)
            {
                ResetUserInfo();
                MessageBox.Show("No User with UserID = " + UserID.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            _FillUserInfo();
        }


        private async void _FillUserInfo()
        {

            ctrlPersonCard2.LoadPersonInfo(_User.PersonalID);
            lblUserName.Text = _User.UserName;
            lblUserID.Text = _User.ID.ToString();

            if (_User.IsActive)
                lblIsActive.Text = "Yes";
            if (!_User.IsActive)
                lblIsActive.Text = "No";
        }

        public void ResetUserInfo()
        {
            _UserID = -1;
            lblUserName.Text = "[????]";
            lblUserID.Text = "[????]";
            lblIsActive.Text = "[????]";
        }



    }


}
