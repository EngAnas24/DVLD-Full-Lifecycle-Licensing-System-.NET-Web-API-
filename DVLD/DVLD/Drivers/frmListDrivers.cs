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
            var Drivers = await _driverService.GetDriversAsync();
            _dtDrivers = DatatableExtention.ToDataTable(Drivers);
            dgvDrivers.DataSource = _dtDrivers;
            cbFilterBy.SelectedIndex = 0;
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

            // حالة المسح أو عدم اختيار عمود
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
    }
}
