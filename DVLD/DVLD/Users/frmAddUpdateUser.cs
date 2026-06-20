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
using static DVLDServices.Services.PeopleService;
using static DVLDServices.Services.UserService;

namespace DVLD.Users
{
    public partial class frmAddUpdateUser : Form
    {
        public enum eMode { AddNew = 0, Update = 1 }
        private eMode Mode;
        private int UserID;
        public User _User;
        public UserDto User;

        private readonly UserService _userService;

        private static readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient
        {
            BaseAddress = new Uri("http://localhost:5067/")
        };

        public frmAddUpdateUser()
        {
            InitializeComponent();
            Mode = eMode.AddNew;
            _userService = new UserService(_httpClient);
        }

        public frmAddUpdateUser(int userID)
        {
            InitializeComponent();
            Mode = eMode.Update;
            _userService = new UserService(_httpClient);
            this.UserID = userID;
        }

        private async void frmAddUpdateUser_Load(object sender, EventArgs e)
        {
            _ResetDefaultValues();

            if (Mode == eMode.Update)
                await LoadData();
        }

        private async Task LoadData()
        {
            try
            {
                // إسناد البيانات مباشرة للمتغير العام الخاص بالفورم لمنع الـ NullReferenceException
                User = await _userService.GetUserByIdAsync(UserID);

                ctrlPersonCardWithFilter1.FilterEnabled = false;

                if (User == null)
                {
                    MessageBox.Show($"No User with ID = {UserID}", "User Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    this.Close();
                    return;
                }

                // تعبئة عناصر الواجهة من الكائن العام
                lblUserID.Text = User.ID.ToString();
                txtUserName.Text = User.UserName;
                txtPassword.Text = User.Password;
                txtConfirmPassword.Text = User.Password;
                chkIsActive.Checked = User.IsActive;
                ctrlPersonCardWithFilter1.LoadPersonInfo(User.PersonalID);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void _ResetDefaultValues()
        {
            if (Mode == eMode.AddNew)
            {
                lblTitle.Text = "Add New User";
                this.Text = "Add New User";
                _User = new User(); // حجز مكان في الذاكرة لوضع الإضافة
                tpLoginInfo.Enabled = false;
                btnSave.Enabled = false;

                // تصفير الحقول فقط في وضع الإضافة الجديد
                txtUserName.Text = "";
                txtPassword.Text = "";
                txtConfirmPassword.Text = "";
                chkIsActive.Checked = true;

                ctrlPersonCardWithFilter1.FilterFocus();
            }
            else
            {
                lblTitle.Text = "Update User";
                this.Text = "Update User";
                tpLoginInfo.Enabled = true;
                btnSave.Enabled = true;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPersonInfoNext_Click(object sender, EventArgs e)
        {
            if (Mode == eMode.Update)
            {
                btnSave.Enabled = true;
                tpLoginInfo.Enabled = true;
                tcUserInfo.SelectedTab = tcUserInfo.TabPages["tpLoginInfo"];
                return;
            }

            if (ctrlPersonCardWithFilter1.PersonID != -1)
            {
                tpLoginInfo.Enabled = true;
                tcUserInfo.SelectedTab = tcUserInfo.TabPages["tpLoginInfo"];
                btnSave.Enabled = true;
            }
            else
            {
                MessageBox.Show("Please Select a Person", "Please Select a Person", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ctrlPersonCardWithFilter1.FilterFocus();
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                MessageBox.Show("Some fields are not valid!, put the mouse over the red icon(s) to see the error",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // برمجة دفاعية لحماية الكائن من أي خطأ غير متوقع
            if (_User == null) _User = new User();

            _User.PersonalID = ctrlPersonCardWithFilter1.PersonID;
            _User.UserName = txtUserName.Text.Trim();
            _User.Password = txtPassword.Text.Trim();
            _User.IsActive = chkIsActive.Checked;

            try
            {
                if (Mode == eMode.AddNew)
                {
                    // استدعاء واحد فقط نظيف لمنع التكرار
                    _User = await _userService.AddUserAsync(_User);

                    if (_User != null && _User.ID > 0)
                    {
                        Mode = eMode.Update;
                        lblUserID.Text = _User.ID.ToString();
                        lblTitle.Text = "Update User";
                        this.Text = "Update User";
                        MessageBox.Show("Data Saved Successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Error: Data was not saved successfully.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else // وضع التعديل (Mode == eMode.Update)
                {
                    _User.ID = UserID;
                    _User = await _userService.UpdateUser(UserID, _User);

                    if (_User != null)
                    {
                        lblUserID.Text = _User.ID.ToString();
                        MessageBox.Show("Data Updated Successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while saving: {ex.Message}", "Exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void txtConfirmPassword_Validating(object sender, CancelEventArgs e)
        {
            if (txtConfirmPassword.Text.Trim() != txtPassword.Text.Trim())
            {
                e.Cancel = true;
                errorProvider1.SetError(txtConfirmPassword, "Password Confirmation does not match Password!");
            }
            else
            {
                errorProvider1.SetError(txtConfirmPassword, null);
            }
        }

        private void txtPassword_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtPassword.Text.Trim()))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtPassword, "Password cannot be blank");
            }
            else
            {
                errorProvider1.SetError(txtPassword, null);
            }
        }

        private void txtUserName_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUserName.Text.Trim()))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtUserName, "Username cannot be blank");
            }
            else
            {
                errorProvider1.SetError(txtUserName, null);
            }
        }
    }
}