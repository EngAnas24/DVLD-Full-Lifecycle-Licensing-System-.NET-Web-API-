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
        private readonly UserService _userService;
        private static readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient
        {
            BaseAddress = new Uri("http://localhost:5067/") // تأكد من وجود الـ / في النهاية
        };
        public frmAddUpdateUser()
        {
            InitializeComponent();
            Mode = eMode.AddNew;
            _userService = new UserService(_httpClient);
        }
        public frmAddUpdateUser(int UserID)
        {
            InitializeComponent();
            Mode = eMode.Update;
            _userService = new UserService(_httpClient);
            this.UserID = UserID;
        }

        private void frmAddUpdateUser_Load(object sender, EventArgs e)
        {
            Shown();
            if (Mode == eMode.Update)
                LoadData();
        }

        private async void LoadData()
        {
            var User = await _userService.GetUserByIdAsync(UserID);
            ctrlPersonCardWithFilter1.FilterEnabled = false;
            if (User == null)
            {
                MessageBox.Show("No User with ID = " + User, "User Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();

                return;
            }
           else
            {
                lblUserID.Text = User.ID.ToString();
                txtUserName.Text = User.UserName;
                txtPassword.Text = User.Password;
                txtConfirmPassword.Text = User.Password;
                chkIsActive.Checked = (User.IsActive);
                ctrlPersonCardWithFilter1.LoadPersonInfo(User.PersonalID);
            }
        }

        public void Shown()
        {
            if (Mode == eMode.AddNew)
            {
                lblTitle.Text = "Add New User";
                this.Text  = "Add New User";
                _User = new User();
                tpLoginInfo.Enabled = false;
                ctrlPersonCardWithFilter1.FilterFocus();

                
            }
            else
            {

                lblTitle.Text = "Update User";
                this.Text = "Update User";

                tpLoginInfo.Enabled = true;
                btnSave.Enabled = true;
            }

            txtUserName.Text = "";
            txtPassword.Text = "";
            txtConfirmPassword.Text = "";
            chkIsActive.Checked = true;

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
                MessageBox.Show("Please Select a Person  ", "Please Select a Person ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ctrlPersonCardWithFilter1.FilterFocus();
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                //Here we dont continue becuase the form is not valid
                MessageBox.Show("Some fileds are not valide!, put the mouse over the red icon(s) to see the erro",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }

            _User.PersonalID = ctrlPersonCardWithFilter1.PersonID;
            _User.UserName = txtUserName.Text.Trim();
            _User.Password = txtPassword.Text.Trim();
            _User.IsActive = chkIsActive.Checked;


            if (UserID <= 0)
            {
                try
                {
                    _User = await _userService.AddUserAsync(_User);
                    //change form mode to update.
                    Mode = eMode.Update;
                    lblTitle.Text = "Update User";
                    this.Text = "Update User";
                    MessageBox.Show("Data Saved Successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else if (UserID > 0)
            {
                _User.ID = UserID;
                _User = await _userService.UpdateUser(UserID, _User);
                lblUserID.Text = _User.ID.ToString();
                MessageBox.Show("Data Saved Successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else
                MessageBox.Show("Error: Data Is not Saved Successfully.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

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
            };

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
            };

        }

        private void txtUserName_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtUserName.Text.Trim()))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtUserName, "Username cannot be blank");
                return;
            }
            else
            {
                errorProvider1.SetError(txtUserName, null);
            };
        }



    }
    
}