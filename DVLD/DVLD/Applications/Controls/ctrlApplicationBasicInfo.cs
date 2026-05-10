using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using DVLDServices.Services;
using DVLD.Peoples;
using static DVLDServices.Services.clsApplicationService;

namespace DVLD.Applications.Controls
{
    public partial class ctrlApplicationBasicInfo : UserControl
    {
        private int _ApplicationID = -1;
        private clsApplicationDto _Application;
        private clsApplicationService _applicationService;

        private static readonly System.Net.Http.HttpClient _httpClient =
            new System.Net.Http.HttpClient
            {
                BaseAddress = new Uri("http://localhost:5067/")
            };

        // الكونستركتر الافتراضي - Designer يستخدمه
        public ctrlApplicationBasicInfo()
        {
            InitializeComponent();
            _applicationService = new clsApplicationService(_httpClient);
            _Application = new clsApplicationDto();
        }

        // LoadApplicationInfo يجب استدعاؤه من الفورم
        public async Task LoadApplicationInfo(int applicationID)
        {
            _ApplicationID = applicationID;

            if (_applicationService == null)
            {
                MessageBox.Show("Service غير مهيأ");
                return;
            }

            var app = await _applicationService.GetApplicationByIdAsync(applicationID);

            if (app == null)
            {
                ResetApplicationInfo();
                MessageBox.Show("مافي اي تقديم", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _Application = app;

            // تعبئة Labels بطريقة آمنة
            lblApplicationID.Text = app.ApplicationID.ToString();
            lblType.Text = app.ApplicationTypeID.ToString();
            lblStatus.Text = app.ApplicationStatus != null ? app.ApplicationStatus.ToString() : "";
            lblFees.Text = app.PaidFees.ToString();
            lblDate.Text = app.ApplicationDate.ToString();
            lblCreatedByUser.Text = app.CreatedByUserID.ToString();
            lblStatusDate.Text = app.LastStatusDate.ToString();
            lblApplicant.Text = app.ApplicantName;
        }

        public void ResetApplicationInfo()
        {
            _ApplicationID = -1;
            lblApplicationID.Text = "[????]";
            lblStatus.Text = "[????]";
            lblType.Text = "[????]";
            lblFees.Text = "[????]";
            lblApplicant.Text = "[????]";
            lblDate.Text = "[????]";
            lblStatusDate.Text = "[????]";
            lblCreatedByUser.Text = "[????]";

        }

        private async void llViewPersonInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_Application == null)
                return;

            frmShowPersonInfo frm = new frmShowPersonInfo(_Application.ApplicantPersonID);
            frm.ShowDialog();

            await LoadApplicationInfo(_ApplicationID);
        }
    }
}