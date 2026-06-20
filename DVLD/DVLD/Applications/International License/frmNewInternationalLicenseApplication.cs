using ClassLibrary1.GlobalClasses;
using DVLD.Licenses;
using DVLD.Licenses.International_Licenses;
using DVLDServices.GlobalClasses;
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
using static DVLDServices.Services.ApplicationTypeService;
using static DVLDServices.Services.clsApplicationService;
using static DVLDServices.Services.InternationalLicenseService;

namespace DVLD.Applications.International_License
{
    public partial class frmNewInternationalLicenseApplication : Form
    {
        private int _InternationalLicenseID = -1;
        private int _LicenseID;
        private static readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient
        {
            BaseAddress = new Uri("http://localhost:5067/")
        };
        private LicenseService _licenseService;
        private InternationalLicenseService _InternationallicenseService;
        private ApplicationTypeService _typeService;
        private clsApplicationService _applicationService;
        private DriverService _driverService;
        private LocalDrivingLicesnseApplicationsService _localDrivingLicesnse;


        public frmNewInternationalLicenseApplication()
        {
            InitializeComponent();
            _licenseService = new LicenseService(_httpClient);
            _InternationallicenseService = new InternationalLicenseService(_httpClient);
            _applicationService = new clsApplicationService(_httpClient);
            _typeService = new ApplicationTypeService(_httpClient);
            _localDrivingLicesnse = new LocalDrivingLicesnseApplicationsService(_httpClient);
            _driverService = new DriverService(_httpClient);
            ctrlDriverLicenseInfoWithFilter1.OnLicenseSelected += ctrlDriverLicenseInfoWithFilter1_OnLicenseSelected;
        }
        private async void ctrlDriverLicenseInfoWithFilter1_OnLicenseSelected(int ID)
        {
            _LicenseID = ID;
            lblLocalLicenseID.Text = _LicenseID.ToString();

            llShowLicenseHistory.Enabled = (_LicenseID != -1);

            if (_LicenseID == -1)

            {
                return;
            }

            if (ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo == null)
            {
                return;
            }

            if (ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.LicenseClass != 3)
            {
                MessageBox.Show("Selected License should be Class 3, select another one.",
                                "Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var InternationaLicense = await _InternationallicenseService.GetActiveInternationalLicenseByDriverID(ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.DriverID);

            if (InternationaLicense != null && InternationaLicense.InternationalLicenseID != -1)
            {
                MessageBox.Show("Person already has an active international license with ID = " + InternationaLicense.InternationalLicenseID.ToString(),
                                "Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);

                _InternationalLicenseID = InternationaLicense.InternationalLicenseID;
                llShowLicenseInfo.Enabled = true;
                btnIssueLicense.Enabled = false;
                return;
            }

            btnIssueLicense.Enabled = true;
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void frmNewInternationalLicenseApplication_Load(object sender, EventArgs e)
        {

            lblApplicationDate.Text = clsFormat.DateToShort(DateTime.Now);
            lblIssueDate.Text = lblApplicationDate.Text;
            lblExpirationDate.Text = clsFormat.DateToShort(DateTime.Now.AddYears(1));//add one year.
            var AppType = await _typeService.GetApplicationTypeByIdAsync((int)EnApplicationType.NewInternationalLicense);
            lblFees.Text = AppType.ApplicationFees.ToString();
            lblCreatedByUser.Text = clsGlobal.GetUser.UserName;
        }

        private async void btnIssueLicense_Click(object sender, EventArgs e)
        {
            var license = await _licenseService.GetLicenseByIdAsync(_LicenseID);
            var driver = await _driverService.GetDriverByDriverIDAsync(license.DriverID);
            var AppType = await _typeService.GetApplicationTypeByIdAsync((int)EnApplicationType.NewInternationalLicense);
            var LocalApp = await _localDrivingLicesnse.GetLocalApplicationByApplicationId(license.ApplicationID);
            clsApplication clsApplication = new clsApplication()
            {
                ApplicantPersonID = driver.PersonID,
                ApplicationDate = DateTime.Now,
                ApplicationStatus = (int)EnApplicationStatus.Completed,
                ApplicationTypeID = (int)EnApplicationType.NewInternationalLicense,
                CreatedByUserID = clsGlobal.GetUser.ID,
                PaidFees  = AppType.ApplicationFees,
                LastStatusDate =  DateTime.Now,
                

            };

            var InternationalLicense = new InternationalLicense()
            {
                DriverID = ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.DriverID,
                IssuedUsingLocalLicenseID = LocalApp.LocalDrivingLicenseApplicationID,
                IssueDate = DateTime.Now,
                ExpirationDate = DateTime.Now.AddYears(1),
                CreatedByUserID = clsGlobal.GetUser.ID,
                ApplicationID = ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.ApplicationID,
                IsActive = true 
            };
            var InternationalLicenseResult = await _InternationallicenseService.AddInternationalLicenseAsync(InternationalLicense);
            var ApplicationResult = await _applicationService.AddApplicationAsync(clsApplication);
            if(ApplicationResult == null || InternationalLicenseResult == null)
            {
                MessageBox.Show("لم يتم الحفظ" , "Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
                MessageBox.Show("تم الحفظ بنجاح", "Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Information);


            var International = await _InternationallicenseService.GetActiveInternationalLicenseByDriverID(InternationalLicenseResult.DriverID);
            lblInternationalLicenseID.Text = International.InternationalLicenseID.ToString();
            lblApplicationID.Text = International.ApplicationID.ToString();
            lblLocalLicenseID.Text = International.IssuedUsingLocalLicenseID.ToString();

            btnIssueLicense.Enabled = false;
            llShowLicenseInfo.Enabled = true;
            ctrlDriverLicenseInfoWithFilter1.FilterEnabled = false;

        }

        private  void llShowLicenseInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmShowInternationalLicenseInfo frm = new frmShowInternationalLicenseInfo(_InternationalLicenseID);
            frm.ShowDialog();
        }

        private async void llShowLicenseHistory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var license = await _licenseService.GetLicenseByIdAsync(_LicenseID);
            var driver = await _driverService.GetDriverByDriverIDAsync(license.DriverID);
            frmShowPersonLicenseHistory frm = new frmShowPersonLicenseHistory(driver.PersonID);
             frm.ShowDialog();
        }

        private void gpApplicationInfo_Enter(object sender, EventArgs e)
        {

        }
    }
}
