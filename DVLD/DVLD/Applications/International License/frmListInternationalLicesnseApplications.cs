using DVLD.Licenses;
using DVLD.Licenses.International_Licenses;
using DVLD.Peoples;
using DVLDServices.Extentions;
using DVLDServices.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using static DVLDServices.Services.InternationalLicenseService;

namespace DVLD.Applications.International_License
{
    public partial class frmListInternationalLicesnseApplications : Form
    {
        private int _InternationallincenseID;
        private DriverService _driverService;
        private InternationalLicenseService InternationalLicenseService;
        private static readonly System.Net.Http.HttpClient _httpClient =
        new System.Net.Http.HttpClient
        {
            BaseAddress = new Uri("http://localhost:5067/")
        };
        private DataTable _dtInternationalLicenseApplications;
        public frmListInternationalLicesnseApplications()
        {
            InitializeComponent();
            _driverService = new DriverService(_httpClient);
            InternationalLicenseService = new InternationalLicenseService(_httpClient);
        }

        private async void frmListInternationalLicesnseApplications_Load(object sender, EventArgs e)
        {
            try
            {
  
            var list = await InternationalLicenseService.GetInternationalLicensesAsync();
            if (list != null)
            {
                _dtInternationalLicenseApplications = DatatableExtention.ToDataTable(list);
                dgvInternationalLicenses.DataSource = _dtInternationalLicenseApplications;
            }
            else
                dgvInternationalLicenses.DataSource = null;
            }
            catch
            {
                List<InternationalLicenseDto> emptyList =
                    new List<InternationalLicenseDto>();
                dgvInternationalLicenses.DataSource = emptyList;

            }
            finally
            {
                if (cbFilterBy.Items.Count > 0)
                    cbFilterBy.SelectedIndex = 0;

                if (cbFilterBy.Text == "None")
                    txtFilterValue.Visible = false;
            }
        }

        private async void PesonDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var DriverID =(int)dgvInternationalLicenses.CurrentRow.Cells["DriverID"].Value;
            var Driver = await _driverService.GetDriverByDriverIDAsync(DriverID);
            frmShowPersonInfo frmShowPerson = new frmShowPersonInfo(Driver.PersonID);
                frmShowPerson.ShowDialog();
        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var InternationalLicenseID = (int)dgvInternationalLicenses.CurrentRow.Cells["InternationalLicenseID"].Value;

            frmShowInternationalLicenseInfo frmShowInternational = new frmShowInternationalLicenseInfo(InternationalLicenseID);
            frmShowInternational.ShowDialog();
        }

        private async void showPersonLicenseHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var InternationalLicenseID = (int)dgvInternationalLicenses.CurrentRow.Cells["InternationalLicenseID"].Value;
            var license = await InternationalLicenseService.GetInternationalLicenseByIdAsync(InternationalLicenseID);
            var driver = await _driverService.GetDriverByDriverIDAsync(license.DriverID);
            frmShowPersonLicenseHistory frm = new frmShowPersonLicenseHistory(driver.PersonID);
            frm.ShowDialog();
        }

        private void btnNewApplication_Click(object sender, EventArgs e)
        {
            frmNewInternationalLicenseApplication frmNewInternationalLicense = new frmNewInternationalLicenseApplication();
            frmNewInternationalLicense.ShowDialog();
            frmListInternationalLicesnseApplications_Load(null, null);

        }

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbFilterBy.Text == "Is Active")
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
                    //_dtDetainedLicenses.DefaultView.RowFilter = "";
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
            string FilterColumn = "IsActive";
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
                _dtInternationalLicenseApplications.DefaultView.RowFilter = "";
            else
                //in this case we deal with numbers not string.
                _dtInternationalLicenseApplications.DefaultView.RowFilter = string.Format("[{0}] = {1}", FilterColumn, FilterValue);

            lblInternationalLicensesRecords.Text = _dtInternationalLicenseApplications.Rows.Count.ToString();
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {


            string FilterColumn = "";
            //Map Selected Filter to real Column name 
            switch (cbFilterBy.Text)
            {
                case "International License ID":
                    FilterColumn = "InternationalLicenseID";
                    break;
                case "Application ID":
                    {
                        FilterColumn = "ApplicationID";
                        break;
                    };

                case "Driver ID":
                    FilterColumn = "DriverID";
                    break;

                case "Local License ID":
                    FilterColumn = "IssuedUsingLocalLicenseID";
                    break;

                case "Is Active":
                    FilterColumn = "IsActive";
                    break;


                default:
                    FilterColumn = "None";
                    break;
            }


            //Reset the filters in case nothing selected or filter value conains nothing.
            if (txtFilterValue.Text.Trim() == "" || FilterColumn == "None")
            {
                _dtInternationalLicenseApplications.DefaultView.RowFilter = "";
                lblInternationalLicensesRecords.Text = dgvInternationalLicenses.Rows.Count.ToString();
                return;
            }



            _dtInternationalLicenseApplications.DefaultView.RowFilter = string.Format("[{0}] = {1}", FilterColumn, txtFilterValue.Text.Trim());

            lblInternationalLicensesRecords.Text = _dtInternationalLicenseApplications.Rows.Count.ToString();
        }
    }
}
