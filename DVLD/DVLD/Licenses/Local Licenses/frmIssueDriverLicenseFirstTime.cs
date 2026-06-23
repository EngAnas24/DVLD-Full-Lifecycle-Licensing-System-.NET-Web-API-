using DVLDServices.GlobalClasses;
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
using static DVLDServices.Services.clsApplicationService;
using static DVLDServices.Services.DriverService;
using static DVLDServices.Services.LicenseService;
using static DVLDServices.Services.LocalDrivingLicesnseApplicationsService;

namespace DVLD.Licenses.Local_Licenses
{
    public partial class frmIssueDriverLicenseFirstTime : Form
    {
        private int _LocalDrivingLicenseApplicationID;
        private LocalDrivingLicenseApplicationsDto _LocalDrivingLicenseApplication;
        private LocalDrivingLicesnseApplicationsService _LocalApplicationsService;
        private DriverService _driverService;
        private LicenseService _licenseService;
        private LicenseClassService _ClassService;
        private clsApplicationService _applicationService;
        private int LicenseID = -1;
        private DriverDto Driver;
        private Driver driver;

        HttpClient _httpClient = new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:5067/")
        };

        public frmIssueDriverLicenseFirstTime(int LocalDrivingLicenseApplicationID)
        {
            InitializeComponent();
            _LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplicationID;
            _LocalApplicationsService = new LocalDrivingLicesnseApplicationsService(_httpClient);
            _licenseService = new LicenseService(_httpClient);
            _driverService = new DriverService(_httpClient);
            _ClassService = new LicenseClassService(_httpClient);
            _applicationService = new clsApplicationService(_httpClient);
            Driver = new DriverDto();

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmIssueDriverLicenseFirstTime_Load(object sender, EventArgs e)
        {
            LoadData();
        }


        public async Task LoadData() // تم تغييرها إلى Task لسهولة تتبع الأخطاء
        {
            try
            {
                txtNotes.Focus();

                // 1. جلب بيانات الطلب المحلي
                _LocalDrivingLicenseApplication = await _LocalApplicationsService.GetLocalApplicationByIdAsync(_LocalDrivingLicenseApplicationID);

                if (_LocalDrivingLicenseApplication == null)
                {
                    MessageBox.Show($"No Application with ID = {_LocalDrivingLicenseApplicationID}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                // 2. التحقق من عدد الاختبارات المجتازة
                if (_LocalDrivingLicenseApplication.PassedTestCount != 3)
                {
                    MessageBox.Show("Person Should Pass All Tests First.", "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                // 3. التحقق من وجود رخصة سابقة
                var licenseInfo = await _licenseService.GetLicenseByApplicationIdAsync(_LocalDrivingLicenseApplication.ApplicationID);

                if (licenseInfo != null)
                {
                    // ملاحظة: تأكد من منطق جلب الـ ID هنا، هل نحتاج طلب إضافي فعلاً؟
                    // إذا كان الـ ID موجود في الـ licenseInfo لا داعي للاستعلام مرة أخرى
                    LicenseID = licenseInfo.ID;

                    MessageBox.Show($"Person already has License with ID = {LicenseID}", "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                // 4. تحميل البيانات للكنترول
                await ctrlDrivingLicenseApplicationInfo1.LoadData(_LocalDrivingLicenseApplicationID);
            }
            catch (Exception ex)
            {
                // عرض الخطأ للمستخدم بدلاً من توقف البرنامج
                MessageBox.Show("An error occurred while loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void btnIssueLicense_Click(object sender, EventArgs e)
        {
            var Licenseinfo = await _licenseService.GetLicenseByApplicationIdAsync(_LocalDrivingLicenseApplication.ApplicationID);
            var App = await _applicationService.GetApplicationByIdAsync(_LocalDrivingLicenseApplication.ApplicationID);

            int driverId = -1;

            var existingDriver = await _driverService.GetDriverByPersonIDAsync(App.ApplicantPersonID);

            if (existingDriver != null)
            {
                driverId = existingDriver.DriverID;
            }
            else
            {
                var newDriver = new Driver()
                {
                    CreatedDate = DateTime.Now,
                    PersonID = App.ApplicantPersonID,
                    CreatedByUserID = clsGlobal.GetUser.ID
                };

                var saveDriverResult = await _driverService.AddDriverAsync(newDriver);

                if (saveDriverResult != null)
                {
                    driverId = saveDriverResult.DriverID;
                }
                else
                {
                    MessageBox.Show("Error: Driver's Data Is not Saved Successfully.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            var LicenseClassID= await _ClassService.GetLicenseClassIDByLocalAppID(_LocalDrivingLicenseApplicationID);
            var LicenseClassInfo = await _ClassService.GetLicenseClassByIdAsync(LicenseClassID);
            if (LicenseClassInfo == null)
            {
                MessageBox.Show("خطأ: لم يتم العثور على معلومات فئة الرخصة.");
                return;
            }
            var license = new clsLicense()
            {
                ApplicationID = _LocalDrivingLicenseApplication.ApplicationID,
                DriverID = driverId,
                CreatedByUserID = clsGlobal.GetUser.ID,
                ExpirationDate = DateTime.Now.AddYears(LicenseClassInfo.DefaultValidityLength),
                IsActive = 1,
                IssueDate = DateTime.Now,
                IssueReason = (int) enIssueReason.FirstTime,
                LicenseClass = LicenseClassID,
                Notes = txtNotes.Text,
                PaidFees = LicenseClassInfo.ClassFees
            };
            var SaveLicenseResult = await _licenseService.AddLicenseAsync(license);
            if (SaveLicenseResult != null)
            {
                MessageBox.Show("Data Saved Successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnIssueLicense.Enabled = false;
      
                //now we should set the application status to complete.
                App.ApplicationStatus = (int)clsApplicationService.EnApplicationStatus.Completed;
                var Application = new clsApplication()
                {
                    ApplicationID = App.ApplicationID,
                    ApplicantPersonID = App.ApplicantPersonID,
                    ApplicationDate = App.ApplicationDate,
                    ApplicationStatus = App.ApplicationStatus,
                    ApplicationTypeID = App.ApplicationTypeID,
                    CreatedByUserID = App.CreatedByUserID,
                    LastStatusDate = App.LastStatusDate,
                    PaidFees = App.PaidFees
                };
                await _applicationService.UpdateApplication(Application.ApplicationID, Application);
                await _applicationService.ChangeStatusToCompleted(Application.ApplicationID);
            }
            else
                MessageBox.Show("Error: Data Is not Saved Successfully.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
