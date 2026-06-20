using ClassLibrary1.Services;
using DVLD.Licenses;
using DVLD.Licenses.Detain_License;
using DVLD.Licenses.Local_Licenses;
using DVLD.Peoples;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD.Applications.Rlease_Detained_License
{
    public partial class frmListDetainedLicenses : Form
    {
        DetainedLicenseService _detainedLicenseService;
        private DataTable dataTable;
        private static readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient
        {
            BaseAddress = new Uri("http://localhost:5067/")
        };
        public frmListDetainedLicenses()
        {
            InitializeComponent();
            _detainedLicenseService = new DetainedLicenseService(_httpClient);
        }

    

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void frmListDetainedLicenses_Load(object sender, EventArgs e)
        {
            cbFilterBy.SelectedIndex = 0;

            try { 

            var detainedLicenses = await _detainedLicenseService.GetDetainedLicensesAsync();

            dgvDetainedLicenses.DataSource = null;

            if (detainedLicenses is null || detainedLicenses.Count == 0)
            {
                return;
            }

            dataTable = DVLDServices.Extentions.DatatableExtention.ToDataTable(detainedLicenses);

            BindingSource bindingSource = new BindingSource();
            bindingSource.DataSource = dataTable;

            dgvDetainedLicenses.DataSource = bindingSource;


            lblTotalRecords.Text = dgvDetainedLicenses.Rows.Count.ToString();

                if (dgvDetainedLicenses.Rows.Count > 0)
                {
                    dgvDetainedLicenses.Columns[0].HeaderText = "D.ID";
                    dgvDetainedLicenses.Columns[0].Width = 90;

                    dgvDetainedLicenses.Columns[1].HeaderText = "L.ID";
                    dgvDetainedLicenses.Columns[1].Width = 90;

                    dgvDetainedLicenses.Columns[2].HeaderText = "D.Date";
                    dgvDetainedLicenses.Columns[2].Width = 160;

                    dgvDetainedLicenses.Columns[3].HeaderText = "Is Released";
                    dgvDetainedLicenses.Columns[3].Width = 110;

                    dgvDetainedLicenses.Columns[4].HeaderText = "Fine Fees";
                    dgvDetainedLicenses.Columns[4].Width = 110;

                    dgvDetainedLicenses.Columns[5].HeaderText = "Release Date";
                    dgvDetainedLicenses.Columns[5].Width = 160;

                    dgvDetainedLicenses.Columns[6].HeaderText = "N.No.";
                    dgvDetainedLicenses.Columns[6].Width = 90;

                    dgvDetainedLicenses.Columns[7].HeaderText = "Full Name";
                    dgvDetainedLicenses.Columns[7].Width = 330;

                    dgvDetainedLicenses.Columns[8].HeaderText = "Rlease App.ID";
                    dgvDetainedLicenses.Columns[8].Width = 150;

                    dgvDetainedLicenses.Columns[9].HeaderText = "PersonID";
                    dgvDetainedLicenses.Columns[9].Width = 150;
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show($"خطأ أثناء تحديث بيانات الجدول: {ex.Message}", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                List<DetainedLicenseDto> emptyList = new List<DetainedLicenseDto>();
                dgvDetainedLicenses.DataSource = emptyList;

            }

        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {
            string FilterColumn = "";
            switch (cbFilterBy.Text)
            {
                case "Detain ID":
                    FilterColumn = "DetainID";
                    break;
                case "Is Released":
                    {
                        FilterColumn = "IsReleased";
                        break;
                    };

                case "National No.":
                    FilterColumn = "NationalNo";
                    break;


                case "Full Name":
                    FilterColumn = "FullName";
                    break;

                case "Release Application ID":
                    FilterColumn = "ReleaseApplicationID";
                    break;

                default:
                    FilterColumn = "None";
                    break;
            }


            if (txtFilterValue.Text.Trim() == "" || FilterColumn == "None")
            {
                dataTable.DefaultView.RowFilter = "";
                lblTotalRecords.Text = dgvDetainedLicenses.Rows.Count.ToString();
                return;
            }


            if (FilterColumn == "DetainID" || FilterColumn == "ReleaseApplicationID")
                //in this case we deal with numbers not string.
                dataTable.DefaultView.RowFilter = string.Format("[{0}] = {1}", FilterColumn, txtFilterValue.Text.Trim());
            else
                dataTable.DefaultView.RowFilter = string.Format("[{0}] LIKE '{1}%'", FilterColumn, txtFilterValue.Text.Trim());

            lblTotalRecords.Text = dataTable.Rows.Count.ToString();

        }

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbFilterBy.Text == "Is Released")
            {
                txtFilterValue.Visible = false;
                cbIsReleased.Visible = true;
                cbIsReleased.Focus();
                cbIsReleased.SelectedIndex = 0;
            }

            else

            {

                txtFilterValue.Visible = (cbFilterBy.Text != "None");
                cbIsReleased.Visible = false;

                if (cbFilterBy.Text == "None")
                {
                    txtFilterValue.Enabled = false;
                    //dataTable.DefaultView.RowFilter = "";
                    //lblTotalRecords.Text = dgvDetainedLicenses.Rows.Count.ToString();

                }
                else
                    txtFilterValue.Enabled = true;

                txtFilterValue.Text = "";
                txtFilterValue.Focus();
            }
        }

        private void cbIsReleased_SelectedIndexChanged(object sender, EventArgs e)
        {
            string FilterColumn = "IsReleased";
            string FilterValue = cbIsReleased.Text;

            switch (FilterValue)
            {
                case "All":
                    break;
                case "Yes":
                    FilterValue = "1";
                    break;
                case "No":
                    FilterValue = "0";
                    break;
            }


            if (FilterValue == "All")
                dataTable.DefaultView.RowFilter = "";
            else
                dataTable.DefaultView.RowFilter = string.Format("[{0}] = {1}", FilterColumn, FilterValue);

            lblTotalRecords.Text = dataTable.Rows.Count.ToString();
        }

        private void txtFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cbFilterBy.Text == "Detain ID" || cbFilterBy.Text == "Release Application ID")
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void showPersonLicenseHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {

            int PersonalID = (int)dgvDetainedLicenses.CurrentRow.Cells[9].Value;
            frmShowPersonLicenseHistory personLicenseHistory = new frmShowPersonLicenseHistory(PersonalID);
            personLicenseHistory.ShowDialog();
        }

        private void PesonDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int PersonalID = (int)dgvDetainedLicenses.CurrentRow.Cells[9].Value;

            frmShowPersonInfo frm = new frmShowPersonInfo(PersonalID);
            frm.ShowDialog();
        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int LicenseID = (int)dgvDetainedLicenses.CurrentRow.Cells[1].Value;

            frmShowLicenseInfo frm = new frmShowLicenseInfo(LicenseID);
            frm.ShowDialog();

        }

        private void btnDetainLicense_Click(object sender, EventArgs e)
        {
            frmDetainLicenseApplication frm = new frmDetainLicenseApplication();
            frm.ShowDialog();
            //refresh
            frmListDetainedLicenses_Load(null, null);

        }

        private void btnReleaseDetainedLicense_Click(object sender, EventArgs e)
        {
            frmReleaseDetainedLicenseApplication frm = new frmReleaseDetainedLicenseApplication();
            frm.ShowDialog();
            //refresh
            frmListDetainedLicenses_Load(null, null);

        }

        private void releaseDetainedLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {

            int LicenseID = (int)dgvDetainedLicenses.CurrentRow.Cells[1].Value;
            frmReleaseDetainedLicenseApplication frm = new frmReleaseDetainedLicenseApplication(LicenseID);
            frm.ShowDialog();
            //refresh
            frmListDetainedLicenses_Load(null, null);



        }

    }
}
