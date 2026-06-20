using ClassLibrary1.GlobalClasses;
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
using static DVLDServices.Services.LicenseService;
using static DVLDServices.Services.LocalDrivingLicesnseApplicationsService;

namespace DVLD.Applications.Renew_Local_License
{
    public partial class frmRenewLocalDrivingLicenseApplication : Form
    {
        private int _LicenseID;
        private LicenseDto _licenseDto;
        private DriverService _driverService;
        private LicenseService _licenseService;
        private clsApplicationService _applicationService;
        private LicenseClassService _ClassService;
        private LocalDrivingLicesnseApplicationsService LocalapplicationsService;
        private static readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient
        {
            BaseAddress = new Uri("http://localhost:5067/")
        };
        public frmRenewLocalDrivingLicenseApplication()
        {
            InitializeComponent();
            _driverService = new DriverService(_httpClient);
            _applicationService = new clsApplicationService(_httpClient);
            _licenseService = new LicenseService(_httpClient);
            _ClassService = new LicenseClassService(_httpClient);
            ctrlDriverLicenseInfoWithFilter1.OnLicenseSelected += ctrlDriverLicenseInfoWithFilter1_OnLicenseSelected;
            LocalapplicationsService = new LocalDrivingLicesnseApplicationsService(_httpClient);
        }



        private async void ctrlDriverLicenseInfoWithFilter1_OnLicenseSelected(int ID)
        {
            _LicenseID = ID;
            lblOldLicenseID.Text = _LicenseID.ToString();

            llShowLicenseHistory.Enabled = (_LicenseID != -1);

            if (_LicenseID == -1)

            {
                return;
            }
            var Lic = await _licenseService.GetLicenseByIdAsync(_LicenseID);
            var app = await _applicationService.GetApplicationByIdAsync(Lic.ApplicationID);
            lblApplicationDate.Text = clsFormat.DateToShort(DateTime.Now);
            lblIssueDate.Text = lblApplicationDate.Text;

            lblExpirationDate.Text = "???";
            lblApplicationFees.Text = app.PaidFees.ToString();
            lblCreatedByUser.Text = clsGlobal.GetUser.UserName;
            lblLicenseFees.Text = Lic.PaidFees.ToString();
            lblTotalFees.Text = (Convert.ToSingle(lblApplicationFees.Text) + Convert.ToSingle(lblLicenseFees.Text)).ToString();
        }

        private async void btnRenewLicense_Click(object sender, EventArgs e)
        {
            var Lic = await _licenseService.GetLicenseByIdAsync(_LicenseID);
            var app = await _applicationService.GetApplicationByIdAsync(Lic.ApplicationID);
            var Licenseclass = await _ClassService.GetLicenseClassByIdAsync(Lic.LicenseClass);

            if (Lic.IsActive != 1)
            {
                MessageBox.Show("Selected License is not Active, choose an active license."
                    , "Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnRenewLicense.Enabled = false;
                return;
            }

            //check the license is not Expired.
            if (Lic.ExpirationDate >= DateTime.Now)
            {
                MessageBox.Show("Selected License is not yet expiared, it will expire on: " + clsFormat.DateToShort(ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.ExpirationDate)
                    , "Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnRenewLicense.Enabled = false;
                return;
            }
            btnRenewLicense.Enabled = true;
            if (MessageBox.Show("Are you sure you want to Renew the license?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            clsApplication application = new clsApplication()
            {
                ApplicationDate = DateTime.Now,
                ApplicationStatus = (int)EnApplicationStatus.Completed,
                ApplicationTypeID = (int)EnApplicationType.RenewDrivingLicenseService,
                CreatedByUserID = clsGlobal.GetUser.ID,

                ApplicantPersonID = app.ApplicantPersonID,
                LastStatusDate = DateTime.Now,
                PaidFees = app.PaidFees

            };
            var ApplicationResult = await _applicationService.AddApplicationAsync(application);
            var License = new clsLicense()
            {
                ApplicationID = ApplicationResult.ApplicationID,
                CreatedByUserID = clsGlobal.GetUser.ID,
                DriverID = Lic.DriverID,
                IsActive = 1,
                IssueDate = DateTime.Now,
                IssueReason = (int)enIssueReason.Renew,
                LicenseClass = Lic.LicenseClass,
                Notes = txtNotes.Text,
                PaidFees = Single.Parse(lblTotalFees.Text),
                ExpirationDate = DateTime.Now.AddYears(Licenseclass.DefaultValidityLength),

            };

            var LicenseResult = await _licenseService.AddLicenseAsync(License);
            var licinfo = await _licenseService.GetLicenseByApplicationIdAsync(LicenseResult.ApplicationID);

            var LocalApp = new LocalDrivingLicenseApplications()
            {
                ApplicationID = ApplicationResult.ApplicationID,
                LicenseClassID = licinfo.LicenseClassID
            };

            var LocalResult = await LocalapplicationsService.InsertLocalApplication(LocalApp);

            if (LicenseResult == null || ApplicationResult == null || LocalResult == null)
            {
                MessageBox.Show("لم يتم الحفظ ", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
                MessageBox.Show("تم الحفظ ", "",MessageBoxButtons.OK, MessageBoxIcon.Information);

            await _licenseService.DeactivateLicenseAsync(_LicenseID);

            btnRenewLicense.Enabled = false;
            ctrlDriverLicenseInfoWithFilter1.FilterEnabled = false;
            llShowLicenseInfo.Enabled = true;

            lblApplicationID.Text = ApplicationResult.ApplicationID.ToString();
            lblCreatedByUser.Text = clsGlobal.GetUser.UserName;
            lblExpirationDate.Text = LicenseResult.ExpirationDate.ToString();
            lblIssueDate.Text = DateTime.Now.ToString();
            lblRenewedLicenseID.Text = LicenseResult.LicenseID.ToString();



        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
