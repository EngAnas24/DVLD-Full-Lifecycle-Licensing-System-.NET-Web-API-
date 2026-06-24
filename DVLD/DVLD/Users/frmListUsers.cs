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
            BaseAddress = new Uri("http://localhost:5067/")
        };

        private DataTable _dtAllUsers;
        private int _selectedUserID = -1;

        public frmListUsers()
        {
            InitializeComponent();
            _UserService = new UserService(_httpClient);
        }

        private async void frmListUsers_Load(object sender, EventArgs e)
        {
            await _RefreshUserList();

            if (cbFilterBy.Items.Count > 0)
                cbFilterBy.SelectedIndex = 0;
        }

        private async Task _RefreshUserList()
        {
            try
            {
                var allUsers = await _UserService.GetUserAsync();

                if (allUsers == null || !allUsers.Any())
                {
                    _dtAllUsers = new DataTable();
                    dgvUsers.DataSource = null;
                    lblRecordsCount.Text = "0";
                    return;
                }

                _dtAllUsers = DatatableExtention.ToDataTable(allUsers);
                dgvUsers.DataSource = _dtAllUsers;

                if (dgvUsers.Columns.Contains("Password"))
                {
                    dgvUsers.Columns["Password"].Visible = false;
                }

                lblRecordsCount.Text = dgvUsers.Rows.Count.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dgvUsers.DataSource = null;
                lblRecordsCount.Text = "0";
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ApplyFilter()
        {
            if (_dtAllUsers == null) return;

            string selectedFilter = cbFilterBy.Text.Trim();
            string filterValue = txtFilterValue.Text.Trim();
            string filterColumn = "";

            switch (selectedFilter)
            {
                case "User ID":
                    filterColumn = "ID";
                    break;

                case "Person ID":
                    filterColumn = "PersonalID";
                    break;

                case "Full Name":
                    filterColumn = "FullName";
                    break;

                case "UserName":
                    filterColumn = "UserName";
                    break;

                case "Is Active":
                    filterColumn = "IsActive";
                    break;

                default:
                    filterColumn = "None";
                    break;
            }

            if (filterColumn == "None" || (filterColumn != "IsActive" && string.IsNullOrWhiteSpace(filterValue)))
            {
                _dtAllUsers.DefaultView.RowFilter = "";
                lblRecordsCount.Text = _dtAllUsers.Rows.Count.ToString();
                return;
            }

            try
            {
                if (filterColumn == "ID" || filterColumn == "PersonalID")
                {
                    if (int.TryParse(filterValue, out int id))
                    {
                        _dtAllUsers.DefaultView.RowFilter = $"[{filterColumn}] = {id}";
                    }
                    else
                    {
                        _dtAllUsers.DefaultView.RowFilter = "1=0"; 
                    }
                }
                else if (filterColumn == "IsActive")
                {
                    if (cbIsActive != null)
                    {
                        string selectedStatus = cbIsActive.Text.Trim();

                        if (selectedStatus == "All" || string.IsNullOrWhiteSpace(selectedStatus))
                        {
                            _dtAllUsers.DefaultView.RowFilter = "";
                        }
                        else
                        {
                            bool isActiveValue = (selectedStatus == "Yes");
                            _dtAllUsers.DefaultView.RowFilter = $"[IsActive] = {isActiveValue}";
                        }
                    }
                }
                else
                {
                    string safeFilterValue = filterValue.Replace("'", "''");
                    _dtAllUsers.DefaultView.RowFilter = $"[{filterColumn}] LIKE '{safeFilterValue}%'";
                }
            }
            catch
            {
                _dtAllUsers.DefaultView.RowFilter = "";
            }

            lblRecordsCount.Text = _dtAllUsers.DefaultView.Count.ToString();
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedFilter = cbFilterBy.Text.Trim();

            txtFilterValue.Visible = (selectedFilter != "None" && selectedFilter != "Is Active");

            if (cbIsActive != null)
            {
                cbIsActive.Visible = (selectedFilter == "Is Active");
            }

            if (txtFilterValue.Visible)
            {
                txtFilterValue.Text = "";
                txtFilterValue.Focus();
            }

            if (cbIsActive != null && cbIsActive.Visible)
            {
                cbIsActive.SelectedIndex = 0; 
            }

            ApplyFilter();
        }

        private void cbIsActive_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void UpdateSelectedUserID()
        {
            if (dgvUsers.CurrentRow != null && dgvUsers.CurrentRow.Index >= 0)
            {
                if (dgvUsers.CurrentRow.Cells["ID"].Value != DBNull.Value)
                {
                    _selectedUserID = Convert.ToInt32(dgvUsers.CurrentRow.Cells["ID"].Value);
                }
            }
        }

        private void dgvUsers_SelectionChanged(object sender, EventArgs e)
        {
            UpdateSelectedUserID();
        }

        private void dgvUsers_DoubleClick(object sender, EventArgs e)
        {
            UpdateSelectedUserID();
            if (_selectedUserID == -1) return;

            frmShowUserInfo frmShowUser = new frmShowUserInfo(_selectedUserID);
            frmShowUser.ShowDialog();
        }

        private void showDetailsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            UpdateSelectedUserID();
            if (_selectedUserID == -1) return;

            frmShowUserInfo frmShowUser = new frmShowUserInfo(_selectedUserID);
            frmShowUser.ShowDialog();
        }

        private async void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateSelectedUserID();
            if (_selectedUserID == -1) return;

            frmAddUpdateUser frmAddUpdate = new frmAddUpdateUser(_selectedUserID);
            frmAddUpdate.ShowDialog();
            await _RefreshUserList();
        }

        private async void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateSelectedUserID();
            if (_selectedUserID == -1) return;

            DialogResult confirmResult = MessageBox.Show(
                "هل أنت متأكد من حذف هذا المستخدم نهائياً؟",
                "تأكيد الحذف",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirmResult != DialogResult.Yes) return;

            bool isDeleted = await DVLDServices.Commons.clsFormHelper.ExecuteSafeDeleteAsync(
                async () => await _UserService.DeleteUserHttpResponseAsync(_selectedUserID),
                "المستخدم"
            );

            if (isDeleted)
            {
                await _RefreshUserList();
            }
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

        private void ChangePasswordtoolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateSelectedUserID();
            if (_selectedUserID == -1) return;
            frmChangePassword changePassword = new frmChangePassword(_selectedUserID);
            changePassword.ShowDialog();
        }
    }
}