using ClassLibrary1.Services;
using DVLD.Licenses;
using DVLD.Licenses.Local_Licenses;
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

namespace DVLD.Applications.Rlease_Detained_License
{

    public partial class frmReleaseDetainedLicenseApplication : Form
    {
        private int _SelectedLicenseID = -1;
        DetainedLicenseService _detainedLicenseService;
        ApplicationTypeService _applicationTypeService;
        LicenseService _licenseService;
        clsApplicationService _applicationService;
        ApplicationTypeService _typeService;
        private static readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient
        {
            BaseAddress = new Uri("http://localhost:5067/")
        };
        public frmReleaseDetainedLicenseApplication()
        {
            InitializeComponent();
            _detainedLicenseService = new DetainedLicenseService(_httpClient);
            _applicationTypeService = new ApplicationTypeService(_httpClient);
            _licenseService = new LicenseService(_httpClient);
            _applicationService = new clsApplicationService(_httpClient);
            _typeService = new ApplicationTypeService(_httpClient);

        }
        public frmReleaseDetainedLicenseApplication(int LicenseID)
        {
            InitializeComponent();
            _SelectedLicenseID = LicenseID;
            ctrlDriverLicenseInfoWithFilter1.LoadLicenseInfo(_SelectedLicenseID);
            ctrlDriverLicenseInfoWithFilter1.FilterEnabled = false;
            _detainedLicenseService = new DetainedLicenseService(_httpClient);
            _applicationTypeService = new ApplicationTypeService(_httpClient);
            _licenseService = new LicenseService(_httpClient);
            _applicationService = new clsApplicationService(_httpClient);
            _typeService = new ApplicationTypeService(_httpClient);

        }
        private void frmReleaseDetainedLicenseApplication_Load(object sender, EventArgs e)
        {

        }

        private async void ctrlDriverLicenseInfoWithFilter1_OnLicenseSelected(int obj)
        {
            _SelectedLicenseID = obj;

            lblLicenseID.Text = _SelectedLicenseID.ToString();

            llShowLicenseHistory.Enabled = (_SelectedLicenseID != -1);

            if (_SelectedLicenseID == -1)

            {
                return;
            }

            var IsDetainedLicense = await _detainedLicenseService.IsDetainedLicenseAsync(_SelectedLicenseID);

            if (!IsDetainedLicense)
            {
                MessageBox.Show("Selected License i is not detained, choose another one.", "Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var DetainedLicense = await _detainedLicenseService.GetDetainedLicenseByLicenseIDAsync(_SelectedLicenseID);
            var appType = await _applicationTypeService.GetApplicationTypeByIdAsync((int)EnApplicationType.ReleaseDetainedDrivingLicense);
            lblApplicationFees.Text = appType.ApplicationFees.ToString();
            lblCreatedByUser.Text = clsGlobal.GetUser.ID.ToString();
            lblDetainID.Text = DetainedLicense.DetainID.ToString();
            lblDetainDate.Text = DetainedLicense.DetainDate.ToString();
            lblFineFees.Text = DetainedLicense.FineFees.ToString();
            lblLicenseID.Text = ctrlDriverLicenseInfoWithFilter1.SelectedLicenseInfo.LicenseID.ToString();
            lblTotalFees.Text = (Convert.ToSingle(lblApplicationFees.Text) + Convert.ToSingle(lblFineFees.Text)).ToString();

            btnRelease.Enabled = true;
        }

        private void frmReleaseDetainedLicenseApplication_Activated(object sender, EventArgs e)
        {
            ctrlDriverLicenseInfoWithFilter1.txtLicenseIDFocus();
        }

        private async void llShowLicenseHistory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            var Lic = await _licenseService.GetLicenseByIdAsync(_SelectedLicenseID);
            var app = await _applicationService.GetApplicationByIdAsync(Lic.ApplicationID);

            frmShowPersonLicenseHistory frm = new frmShowPersonLicenseHistory(app.ApplicantPersonID);
            frm.ShowDialog();

        }
        private void llShowLicenseInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmShowLicenseInfo frm =
           new frmShowLicenseInfo(_SelectedLicenseID);
            frm.ShowDialog();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnRelease_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to release this detained license?", "Confirm",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            try
            {
                var appType = await _applicationTypeService.GetApplicationTypeByIdAsync((int)EnApplicationType.ReleaseDetainedDrivingLicense);
                var detainLicense = await _detainedLicenseService.GetDetainedLicenseByLicenseIDAsync(_SelectedLicenseID);
                int applicantPersonID = await _detainedLicenseService.FindPersonByLicenseId(_SelectedLicenseID);

                if (detainLicense == null)
                {
                    MessageBox.Show("Error: Could not find detain records for this license.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var releaseAppParams = new ReleaseTransactionParams()
                {
                    ApplicationID = 0,
                    ApplicantPersonID = applicantPersonID,
                    ApplicationDate = DateTime.Now,
                    ApplicationTypeID = (int)EnApplicationType.ReleaseDetainedDrivingLicense,
                    ApplicationStatus = (int)clsApplicationService.EnApplicationStatus.Completed,
                    LastStatusDate = DateTime.Now,
                    PaidFees = appType?.ApplicationFees ?? 0,
                    CreatedByUserID = clsGlobal.GetUser.ID,

                    DetainID = detainLicense.DetainID,
                    LicenseID = _SelectedLicenseID,
                    DetainDate = detainLicense.DetainDate,
                    IsReleased = true,
                    FineFees = decimal.Parse(lblFineFees.Text),
                    ReleaseDate = DateTime.Now,
                    ReleaseApplicationID = 0,
                    ReleasedByUserID = clsGlobal.GetUser.ID
                };

                var resultDto = await _detainedLicenseService.ReleaseLicense(releaseAppParams);

                if (resultDto != null)
                {
                    lblApplicationID.Text = resultDto.ReleaseApplicationID.ToString();

                    MessageBox.Show("Detained License released Successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    btnRelease.Enabled = false;
                    ctrlDriverLicenseInfoWithFilter1.FilterEnabled = false;
                    llShowLicenseInfo.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Failed to release the Detained License. Database transaction rolled back.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
