using DVLD.Applications.International_License;
using DVLD.Licenses;
using DVLD.Peoples;
using DVLDServices.Extentions;
using DVLDServices.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DVLDServices.Services.DriverService;

namespace DVLD.Drivers
{
    public partial class frmListDrivers : Form
    {
        private readonly DriverService _driverService;
        private DataTable _dtDrivers;
        HttpClient _httpClient = new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:5067/")
        };
        public frmListDrivers()
        {
            InitializeComponent();
            _driverService = new DriverService(_httpClient);

        }


        private async void LoadData()
        {
            try
            {
                var drivers = await _driverService.GetDriversAsync();

                _dtDrivers = drivers.ToDataTable();
                dgvDrivers.DataSource = _dtDrivers;
            }
            catch (Exception ex) when (ex.Message.Contains("NotFound") || ex.Message.Contains("No Driver found"))
            {
                List<DriverDto> emptyList = new List<DriverDto>(); 
                _dtDrivers = emptyList.ToDataTable();
                dgvDrivers.DataSource = _dtDrivers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ غير متوقع أثناء تحميل البيانات: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (cbFilterBy.Items.Count > 0)
                {
                    cbFilterBy.SelectedIndex = 0;
                }
            }
        }
        private void frmListDrivers_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {
            string filterColumn = "";

            switch (cbFilterBy.Text)
            {
                case "Person ID":
                    filterColumn = "PersonID"; 
                    break;
                case "National No":
                    filterColumn = "NationalNo";
                    break;
                case "Full Name":
                    filterColumn = "FullName";
                    break;
                case "Driver ID":
                    filterColumn = "DriverID";
                    break;
                default:
                    filterColumn = "None";
                    break;
            }

            string filterValue = txtFilterValue.Text.Trim();

            if (string.IsNullOrWhiteSpace(filterValue) || filterColumn == "None")
            {
                _dtDrivers.DefaultView.RowFilter = "";
                lblRecordsCount.Text = _dtDrivers.Rows.Count.ToString();
                return;
            }

            try
            {
                if (filterColumn == "PersonID" || filterColumn == "DriverID")
                {
                    if (int.TryParse(filterValue, out _))
                        _dtDrivers.DefaultView.RowFilter = string.Format("[{0}] = {1}", filterColumn, filterValue);
                    else
                        _dtDrivers.DefaultView.RowFilter = "1=0";                 }
                else
                {
                    _dtDrivers.DefaultView.RowFilter = string.Format("[{0}] LIKE '{1}%'", filterColumn, filterValue);
                }
            }
            catch (Exception)
            {
                _dtDrivers.DefaultView.RowFilter = "";
            }

            lblRecordsCount.Text = _dtDrivers.DefaultView.Count.ToString();
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
                if (_dtDrivers != null)
                {
                    _dtDrivers.DefaultView.RowFilter = "";
                    lblRecordsCount.Text = _dtDrivers.Rows.Count.ToString();
                }
            }
        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int PersonID = (int)dgvDrivers.CurrentRow.Cells["PersonID"].Value;
            frmShowPersonInfo frmShowPerson = new frmShowPersonInfo(PersonID);
            frmListDrivers_Load(null, null);
            frmShowPerson.ShowDialog();

        }

        private void showPersonLicenseHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int PersonID = (int)dgvDrivers.CurrentRow.Cells["PersonID"].Value;
            frmShowPersonLicenseHistory personLicenseHistory = new frmShowPersonLicenseHistory(PersonID);
            personLicenseHistory.ShowDialog();
        }

        private void issueInternationalLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmNewInternationalLicenseApplication frmNewInternationalLicense =
                new frmNewInternationalLicenseApplication();
            frmNewInternationalLicense.ShowDialog();
        }
    }
}
