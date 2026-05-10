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

namespace DVLD.Licenses
{
    public partial class frmShowPersonLicenseHistory : Form
    {
        private int _PersonID;
        private DriverService _driverService;
        HttpClient _httpClient = new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:5067/")
        };
        public frmShowPersonLicenseHistory(int PersonID)
        {
            InitializeComponent();
            _PersonID = PersonID;
            _driverService = new DriverService(_httpClient);
        }

        private async void ctrlDriverLicenses1_Load(object sender, EventArgs e)
        {
           var driver =  await _driverService.GetDriverByPersonIDAsync(_PersonID);
            ctrlDriverLicenses1.LoadLocalLicenseData(_PersonID);
            ctrlPersonCardWithFilter1.LoadPersonInfo(_PersonID);
            ctrlPersonCardWithFilter1.FilterEnabled = false;
            ctrlDriverLicenses1.LoadInternatioalLicenseData(driver.DriverID);
        }
    }
}
