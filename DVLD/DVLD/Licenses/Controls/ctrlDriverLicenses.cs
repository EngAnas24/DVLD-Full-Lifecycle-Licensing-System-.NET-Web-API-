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
using DVLDServices.Extentions;

namespace DVLD.Licenses.Controls
{
    public partial class ctrlDriverLicenses : UserControl
    {
        private DataTable _dataTable;
        private LicenseService _LocallicenseService;
        private InternationalLicenseService _InternationalLicenseService;
        private static readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient
        {
            BaseAddress = new Uri("http://localhost:5067/") // تأكد من وجود الـ / في النهاية
        };
        public ctrlDriverLicenses()
        {
            InitializeComponent();
            _LocallicenseService = new LicenseService(_httpClient);
            _InternationalLicenseService = new InternationalLicenseService(_httpClient);
        }
        
        public async void LoadLocalLicenseData(int PersonID)
        {
            var list = await _LocallicenseService.GetLicensesByPersonIdAsync(PersonID);
            _dataTable = DatatableExtention.ToDataTable(list);
            dgvLocalLicensesHistory.DataSource = _dataTable;
        }

        public async void LoadInternatioalLicenseData(int DriverID)
        {
            var list = await _InternationalLicenseService.GetActiveInternationalLicensesByDriverID(DriverID);
            _dataTable = DatatableExtention.ToDataTable(list);
            dgvInternationalLicensesHistory.DataSource = _dataTable;
        }
    }
}
