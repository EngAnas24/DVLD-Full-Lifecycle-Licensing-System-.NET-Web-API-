using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using DVLDServices.Services;
using DVLD.Applications.Controls;
using DVLD.Licenses.Local_Licenses;

namespace DVLD.Applications.Local_Driving_License
{
    public partial class ctrlDrivingLicenseApplicationInfo : UserControl
    {
        private int _ApplicationID;
        public int ApplicationID
        {
            get => _ApplicationID; 
            set => _ApplicationID = value; 
        }
        private LocalDrivingLicesnseApplicationsService _applicationService;
        private LicenseService _licenseService;
        private LocalDrivingLicesnseApplicationsService _LocalapplicationsService;
        private static readonly System.Net.Http.HttpClient _httpClient =
            new System.Net.Http.HttpClient
            {
                BaseAddress = new Uri("http://localhost:5067/")
            };
        
        public ctrlDrivingLicenseApplicationInfo()
        {
            InitializeComponent();
            _LocalapplicationsService = new LocalDrivingLicesnseApplicationsService(_httpClient);            
            _applicationService = new LocalDrivingLicesnseApplicationsService(_httpClient);
            _licenseService = new LicenseService(_httpClient);
        }

        public async Task LoadData(int applicationID)
        {
            _ApplicationID = applicationID;

            if (_applicationService == null)
            {
                MessageBox.Show("Service غير مهيأ");
                return;
            }

            var application = await _applicationService.GetLocalApplicationByIdAsync(applicationID);

            if (application == null)
            {
                MessageBox.Show("Application غير موجود");
                return;
            }
            if (application.Status == "New" || application.Status == "Canceled")
                llShowLicenceInfo.Enabled = false;

            lblAppliedFor.Text = application.ClassName;
            lblLocalDrivingLicenseApplicationID.Text = application.LocalDrivingLicenseApplicationID.ToString();
            lblPassedTests.Text = application.PassedTestCount.ToString();

            if (ctrlApplicationBasicInfo2 != null)
            {
                await ctrlApplicationBasicInfo2.LoadApplicationInfo(application.ApplicationID);
            }
        }
        private void _ResetLocalDrivingLicenseApplicationInfo()
        {
            _ApplicationID = -1;
            ctrlApplicationBasicInfo1.ResetApplicationInfo();
            lblLocalDrivingLicenseApplicationID.Text = "[????]";
            lblAppliedFor.Text = "[????]";


        }

        private async void llShowLicenceInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var Localapp = await _LocalapplicationsService.GetLocalApplicationByIdAsync(_ApplicationID);
            var app = await _licenseService.GetLicenseByApplicationIdAsync(Localapp.ApplicationID);
            var licenseID = await _licenseService.GetLicenseByPersonIdAsync(app.ID, app.LicenseClassID);

            frmShowLicenseInfo frm = new frmShowLicenseInfo(licenseID);
            frm.ShowDialog();

        }

    }
}