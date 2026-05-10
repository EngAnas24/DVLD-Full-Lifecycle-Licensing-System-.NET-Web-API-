using ClassLibrary1.GlobalClasses;
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
using static DVLDServices.Services.clsApplicationService;
using static DVLDServices.Services.LicenseService;
using static DVLDServices.Services.LocalDrivingLicesnseApplicationsService;

namespace DVLD.Applications.ReplaceLostOrDamagedLicense
{
    public partial class frmReplaceLostOrDamagedLicenseApplication : Form
    {
        private int _LicenseID;
        private LicenseDto _licenseDto;
        private DriverService _driverService;
        private LicenseService _licenseService;
        private clsApplicationService _applicationService;
        private LicenseClassService _ClassService;
        private ApplicationTypeService _typeService;
        private EnApplicationType _ApplicationType;
        private enIssueReason _enIssueReason;
        private LocalDrivingLicesnseApplicationsService _LocalapplicationsService;
        private static readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient
        {
            BaseAddress = new Uri("http://localhost:5067/")
        };
        public frmReplaceLostOrDamagedLicenseApplication()
        {
            InitializeComponent();
            _driverService = new DriverService(_httpClient);
            _applicationService = new clsApplicationService(_httpClient);
            _licenseService = new LicenseService(_httpClient);
            _ClassService = new LicenseClassService(_httpClient);
            _typeService = new ApplicationTypeService(_httpClient);
            _LocalapplicationsService = new LocalDrivingLicesnseApplicationsService(_httpClient);
            ctrlDriverLicenseInfoWithFilter1.OnLicenseSelected += ctrlDriverLicenseInfoWithFilter1_OnLicenseSelected;
        }

        private void frmReplaceLostOrDamagedLicenseApplication_Load(object sender, EventArgs e)
        {
            rbDamagedLicense.Checked = true;
            _GetApplicationTypeID();
            lblApplicationDate.Text = clsFormat.DateToShort(DateTime.Now);
            lblCreatedByUser.Text = clsGlobal.GetUser.UserName;
        }


        private void _GetApplicationTypeID()
        {
            //this will decide which application type to use accirding 
            // to user selection.

            if (rbDamagedLicense.Checked)
            {
                _ApplicationType = EnApplicationType.ReplacementForADamagedDrivingLicense;
                _enIssueReason = enIssueReason.DamagedReplacement;
            }

            else
            {
                _ApplicationType = EnApplicationType.ReplacementForALostDrivingLicense;
                _enIssueReason = enIssueReason.LostReplacement;
            }
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

            var AppType =  await _typeService.GetApplicationTypeByIdAsync((int)_ApplicationType);
            lblApplicationFees.Text = AppType.ApplicationFees.ToString();
            
            
        }

        private async void btnIssueReplacement_Click(object sender, EventArgs e)
        {
            var Lic = await _licenseService.GetLicenseByIdAsync(_LicenseID);
            var app = await _applicationService.GetApplicationByIdAsync(Lic.ApplicationID);
            var Licenseclass = await _ClassService.GetLicenseClassByIdAsync(Lic.LicenseClass);
            var AppType = await _typeService.GetApplicationTypeByIdAsync((int)_ApplicationType);

            //check the license is not not active.
            if (Lic.IsActive != 1)
            {
                MessageBox.Show("Selected License is not Not Active, choose an active license."
                    , "Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnIssueReplacement.Enabled = false;
                return;
            }
            btnIssueReplacement.Enabled = true;

            if (MessageBox.Show("Are you sure you want to Replace the license?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            clsApplication application = new clsApplication()
            {
                ApplicationDate = DateTime.Now,
                ApplicationStatus = (int)EnApplicationStatus.Completed,
                ApplicationTypeID = (int)_ApplicationType,
                CreatedByUserID = clsGlobal.GetUser.ID,

                ApplicantPersonID = app.ApplicantPersonID,
                LastStatusDate = DateTime.Now,
                PaidFees = AppType.ApplicationFees

            };
            var ApplicationResult = await _applicationService.AddApplicationAsync(application);
            var License = new clsLicense()
            {
                ApplicationID = ApplicationResult.ApplicationID,
                CreatedByUserID = clsGlobal.GetUser.ID,
                DriverID = Lic.DriverID,
                IsActive = 1,
                IssueDate = DateTime.Now,
                IssueReason = (int)_enIssueReason,
                LicenseClass = Lic.LicenseClass,
                PaidFees = Single.Parse(lblApplicationFees.Text),
                ExpirationDate = DateTime.Now.AddYears(Licenseclass.DefaultValidityLength),
                Notes = ""
            };

            var LicenseResult = await _licenseService.AddLicenseAsync(License);
            var licinfo = await _licenseService.GetLicenseByApplicationIdAsync(LicenseResult.ApplicationID);

            var LocalApp = new LocalDrivingLicenseApplications()
            {
                ApplicationID = ApplicationResult.ApplicationID,
                LicenseClassID = licinfo.LicenseClassID
            };

            var LocalResult = await _LocalapplicationsService.InsertLocalApplication(LocalApp);

            if (LicenseResult == null || ApplicationResult == null || LocalResult == null)
            {
                MessageBox.Show("لم يتم الحفظ ", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
                MessageBox.Show("تم الحفظ ", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
            await _licenseService.DeactivateLicenseAsync(_LicenseID);

            lblRreplacedLicenseID.Text = LicenseResult.LicenseID.ToString();
            btnIssueReplacement.Enabled = false;
            ctrlDriverLicenseInfoWithFilter1.FilterEnabled = false;
            llShowLicenseInfo.Enabled = true;
            lblRreplacedLicenseID.Text = LicenseResult.LicenseID.ToString();
            llShowLicenseHistory.Enabled = true;
        }

        private void rbDamagedLicense_CheckedChanged(object sender, EventArgs e)
        {
            _GetApplicationTypeID();
        }

        private void rbLostLicense_CheckedChanged(object sender, EventArgs e)
        {
            _GetApplicationTypeID();
        }

        private async void llShowLicenseHistory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var Lic = await _licenseService.GetLicenseByIdAsync(_LicenseID);
            var app = await _applicationService.GetApplicationByIdAsync(Lic.ApplicationID);

            frmShowPersonLicenseHistory frm = new frmShowPersonLicenseHistory(app.ApplicantPersonID);
            frm.ShowDialog();
        }

        private void llShowLicenseInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            frmShowLicenseInfo frmShowLicense = new frmShowLicenseInfo(_LicenseID);
            frmShowLicense.ShowDialog();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
    
}
