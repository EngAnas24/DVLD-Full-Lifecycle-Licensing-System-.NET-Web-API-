using DVLDServices.Extentions;
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
    public partial class frmListUsers : Form
    {
        private readonly UserService _UserService;
        private static readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient
        {
            BaseAddress = new Uri("http://localhost:5067/") // تأكد من وجود الـ / في النهاية
        };

        private DataTable _dtAllUser;
        private int UserID;

        public frmListUsers()
        {
            InitializeComponent();
            _UserService = new UserService(_httpClient);
        }

            private async void frmListUsers_Load(object sender, EventArgs e)
            {
                await _RefreshUserList();

                // تأكد أن الكومبوبوكس يحتوي على العناصر قبل اختيار المندكس
                if (cbFilterBy.Items.Count > 0)
                    cbFilterBy.SelectedIndex = 0;

                if (cbFilterBy.Text == "None")
                    txtFilterValue.Visible = false;

                if (cbFilterBy.Text == "IsActive")
                    cbIsActive.Visible = true;
            }

            private async Task _RefreshUserList()
            {
                try
                {
                    var allUser = await _UserService.GetUserAsync();
                    if (!allUser.Any())
                    {
                        lblRecordsCount.Text = "0";
                    }
                    // تحويل القائمة إلى DataTable باستخدام الـ Extension الخاص بك
                    _dtAllUser = DatatableExtention.ToDataTable(allUser);

                dgvUsers.DataSource = _dtAllUser;
                lblRecordsCount.Text = lblRecordsCount.Text + dgvUsers.Rows.Count.ToString();
                }
                catch (Exception ex)
                {
                lblRecordsCount.Text = lblRecordsCount.Text + " 0";
                }
            }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {
            if (_dtAllUser == null) return;

            string filterColumn = "";

            // يجب أن تطابق الحالات (cases) النصوص الموجودة في الكومبوبوكس تماماً
            switch (cbFilterBy.Text)
            {
                case "UserID":
                    filterColumn = "UserID";
                    break;
                    
                case "PersonalID":
                    filterColumn = "PersonalID";
                    break;
                    
                case "FullName":
                    filterColumn = "FullName";
                    break;
                case "IsActive":
                    filterColumn = "Is Active";
                    break;
                default:
                    filterColumn = "None";
                    break;
            }

            // تنظيف المسافات
            string filterValue = txtFilterValue.Text.Trim();

            if (string.IsNullOrWhiteSpace(filterValue) || filterColumn == "None")
            {
                _dtAllUser.DefaultView.RowFilter = "";
                lblRecordsCount.Text = _dtAllUser.Rows.Count.ToString();
                return;
            }

            try
            {
                // التعامل مع UserID كعمود رقمي
                if (filterColumn == "UserID")
                {
                    if (int.TryParse(filterValue, out int id))
                    {
                        _dtAllUser.DefaultView.RowFilter = string.Format("[{0}] = {1}", filterColumn, id);
                    }
                    else
                    {
                        // إذا كتب المستخدم نصاً في حقل الرقم، نعرض قائمة فارغة
                        _dtAllUser.DefaultView.RowFilter = "1=0";
                    }
                }
                else
                {
                    // بقية الأعمدة نصية
                    _dtAllUser.DefaultView.RowFilter = string.Format("[{0}] LIKE '{1}%'", filterColumn, filterValue);
                }
            }
            catch (Exception ex)
            {
                // لتجنب انهيار البرنامج في حال وجود رموز خاصة
                _dtAllUser.DefaultView.RowFilter = "";
            }

            lblRecordsCount.Text = _dtAllUser.DefaultView.Count.ToString();
        }
        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFilterValue.Visible = (cbFilterBy.Text != "None");

            if (txtFilterValue.Visible)
            {
                txtFilterValue.Text = "";
                txtFilterValue.Focus();
            }
            else
            {
                if (_dtAllUser != null)
                {
                    _dtAllUser.DefaultView.RowFilter = "";
                    lblRecordsCount.Text = _dtAllUser.Rows.Count.ToString();
                }
            }
        }

        private void dgvUsers_DoubleClick(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow == null) return;

            UserID = (int)dgvUsers.CurrentRow.Cells["ID"].Value;
            frmShowUserInfo frmShowUser = new frmShowUserInfo(UserID);
            frmShowUser.ShowDialog();
        }

        private void dgvUser_SelectionChanged(object sender, EventArgs e)
        {

            if (dgvUsers.CurrentRow != null && dgvUsers.CurrentRow.Index >= 0)
            {
                try
                {
                    UserID = Convert.ToInt32(dgvUsers.CurrentRow.Cells["UserID"].Value);
                }

                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
        private void showDetailsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (dgvUsers.CurrentRow == null) return;

            UserID = (int)dgvUsers.CurrentRow.Cells["ID"].Value;
            frmShowUserInfo frmShowUser = new frmShowUserInfo(UserID);
            frmShowUser.ShowDialog();
        }

        private async void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This User will be deleted!! Are you sure you want to delete?",
                                "Confirm Delete",
                                MessageBoxButtons.OKCancel,
                                MessageBoxIcon.Warning) == DialogResult.OK)
            {
                await _UserService.DeleteUserAsync(UserID);
                MessageBox.Show("User Deleted Successfully.", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await _RefreshUserList();
            }

            else
                MessageBox.Show("User was not deleted because it has data linked to it.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }


        private async void btnAddUser_Click(object sender, EventArgs e)
        {
            frmAddUpdateUser addUpdateUser = new frmAddUpdateUser();
            addUpdateUser.ShowDialog();
            await _RefreshUserList();

        }


        private async void addtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddUpdateUser frmAddUpdate = new frmAddUpdateUser();
            frmAddUpdate.ShowDialog();
           await _RefreshUserList();
        }
        private async void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int UserID =(int) dgvUsers.CurrentRow.Cells["ID"].Value;
            frmAddUpdateUser frmAddUpdate = new frmAddUpdateUser(UserID);
            frmAddUpdate.ShowDialog();
            await _RefreshUserList();


        }


        private async void deleteToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            int UserID = (int)dgvUsers.CurrentRow.Cells[0].Value;
            if (UserID != -1)
            {
              await  _UserService.DeleteUserAsync(UserID);
                MessageBox.Show("User has been deleted successfully", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await _RefreshUserList();
            }

            else
                MessageBox.Show("User is not delted due to data connected to it.", "Faild", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }
    }
}

