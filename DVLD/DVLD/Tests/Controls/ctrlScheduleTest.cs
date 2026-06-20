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
using static DVLDServices.Services.LocalDrivingLicesnseApplicationsService;
using static DVLDServices.Services.TestAppointmentService;
using static DVLDServices.Services.clsApplicationService;
using DVLD.Properties;
using DVLDServices.GlobalClasses;

namespace DVLD.Tests.Controls
{
    public partial class ctrlScheduleTest : UserControl
    {
        public enum enMode { AddNew = 0, Update = 1 };
        public enMode _eMode = enMode.AddNew;

        public enum enCreationMode { FirstTimeSchedule = 0, ReTakeTestSchedule = 1 }
        private enCreationMode _CreationMode = enCreationMode.FirstTimeSchedule;

        private enTestType _enTestType = enTestType.VisionTest;
        private LocalDrivingLicenseApplicationsDto _localDrivingLicense;
        private int _LocalDrivingLicenseApplicationID = -1;
        private TestAppointmentDto _TestAppointment;
        private int _TestAppointmentID = -1;

        private ApplicationTypeService _applicationTypeService;
        private LocalDrivingLicesnseApplicationsService _localDrivingLicesnseService;
        private TestAppointmentService _AppointmentService;
        private TestTypeService _TestTypeService;

        private int _CreatedRetakeApplicationID = -1;

        private static readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient
        {
            BaseAddress = new Uri("http://localhost:5067/")
        };

        public ctrlScheduleTest()
        {
            InitializeComponent();
            _localDrivingLicesnseService = new LocalDrivingLicesnseApplicationsService(_httpClient);
            _applicationTypeService = new ApplicationTypeService(_httpClient);
            _AppointmentService = new TestAppointmentService(_httpClient);
            _TestTypeService = new TestTypeService(_httpClient);
        }

        public enTestType enTestType
        {
            get => _enTestType;
            set
            {
                _enTestType = value;
                switch (_enTestType)
                {
                    case enTestType.VisionTest:
                        gbTestType.Text = "Vision Test";
                        break;
                    case enTestType.WrittenTest:
                        gbTestType.Text = "Written Test";
                        break;
                    case enTestType.StreetTest:
                        gbTestType.Text = "Street Test";
                        break;
                }
            }
        }
        public async Task LoadData(int LocalDrivingLicenseApplicationID, int AppointmentID)
        {
            _LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplicationID;

            _localDrivingLicense = await _localDrivingLicesnseService.GetLocalApplicationByIdAsync(_LocalDrivingLicenseApplicationID);

            if (_localDrivingLicense == null)
            {
                MessageBox.Show("Error: No Local Driving License Application with ID = " + _LocalDrivingLicenseApplicationID.ToString(),
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnSave.Enabled = false;
                return;
            }

            if (AppointmentID > 0)
            {
                _eMode = enMode.Update;
                _TestAppointmentID = AppointmentID;
                _TestAppointment = await _AppointmentService.GetTestAppointmentByIdAsync(_TestAppointmentID);
            }
            else
            {
                _eMode = enMode.AddNew;
                _TestAppointmentID = -1;
            }

            bool IsPassedTest = await _AppointmentService.CheckIfPassedTest(LocalDrivingLicenseApplicationID, (int)_enTestType);
            bool IsFailedTest = await _AppointmentService.DoesAttendTestType(LocalDrivingLicenseApplicationID, (int)_enTestType);

            if (IsPassedTest)
            {
                lblUserMessage.Text = "Person already passed this test. You cannot schedule a new appointment.";
                lblUserMessage.Visible = true;
                btnSave.Enabled = false;
                dtpTestDate.Enabled = false;
                return;
            }
            else if (IsFailedTest)
            {
                _CreationMode = enCreationMode.ReTakeTestSchedule;
            }
            else
            {
                _CreationMode = enCreationMode.FirstTimeSchedule;
            }

            if (_CreationMode == enCreationMode.ReTakeTestSchedule)
            {
                var AppType = await _applicationTypeService.GetApplicationTypeByIdAsync((int)ApplicationTypeService.EnApplicationType.RetakeTest);
                lblRetakeAppFees.Text = AppType.ApplicationFees.ToString();
                gbRetakeTestInfo.Enabled = true;
                lblTitle.Text = "Schedule Retake Test";

                lblRetakeTestAppID.Text = (_eMode == enMode.AddNew) ? "-1" : _TestAppointment?.RetakeTestApplicationID.ToString();
            }
            else
            {
                gbRetakeTestInfo.Enabled = false;
                lblTitle.Text = "Schedule Test";
                lblRetakeAppFees.Text = "0";
                lblRetakeTestAppID.Text = "N/A";
            }

            lblLocalDrivingLicenseAppID.Text = _LocalDrivingLicenseApplicationID.ToString();
            lblDrivingClass.Text = _localDrivingLicense.ClassName;
            lblFullName.Text = _localDrivingLicense.FullName;

            var testTrials = await _AppointmentService.GetTotalTrialsPerTest(LocalDrivingLicenseApplicationID, (int)_enTestType);
            lblTrial.Text = testTrials.ToString();

            if (_eMode == enMode.AddNew)
            {
                var TestType = await _TestTypeService.GetTestTypeByIdAsync((int)_enTestType);
                lblFees.Text = TestType.TestTypeFees.ToString();
                dtpTestDate.MinDate = DateTime.Now;
                dtpTestDate.Value = DateTime.Now;
            }
            else
            {
                if (_TestAppointment != null)
                {
                    lblFees.Text = _TestAppointment.PaidFees.ToString();
                    if (DateTime.Compare(DateTime.Now, _TestAppointment.AppointmentDate) < 0)
                        dtpTestDate.MinDate = DateTime.Now;
                    else
                        dtpTestDate.MinDate = _TestAppointment.AppointmentDate;

                    dtpTestDate.Value = _TestAppointment.AppointmentDate;
                }
            }

            lblTotalFees.Text = (Convert.ToSingle(lblFees.Text) + Convert.ToSingle(lblRetakeAppFees.Text)).ToString();

            if (!await _HandleActiveTestAppointmentConstraint()) return;
            if (!await _HandleAppointmentLockedConstraint(AppointmentID)) return;
            if (!await _HandlePrviousTestConstraint()) return;
        }
        private async Task<bool> _HandleAppointmentLockedConstraint(int OriginalAppointmentID)
        {
            if (OriginalAppointmentID > 0)
            {
                var checkAppointment = await _AppointmentService.GetTestAppointmentByIdAsync(OriginalAppointmentID);
                if (checkAppointment != null && checkAppointment.IsLocked == true && _eMode == enMode.Update)
                {
                    lblUserMessage.Visible = true;
                    lblUserMessage.Text = "Person already sat for the test, appointment locked.";
                    dtpTestDate.Enabled = false;
                    btnSave.Enabled = false;
                    return false;
                }
            }
            return true;
        }

        private async Task<bool> _HandlePrviousTestConstraint()
        {
            switch (_enTestType)
            {
                case enTestType.VisionTest:
                    lblUserMessage.Visible = false;
                    return true;

                case enTestType.WrittenTest:
                    if (!await _AppointmentService.CheckIfPassedTest(_LocalDrivingLicenseApplicationID, (int)enTestType.VisionTest))
                    {
                        lblUserMessage.Text = "Cannot Schedule, Vision Test should be passed first";
                        lblUserMessage.Visible = true;
                        btnSave.Enabled = false;
                        dtpTestDate.Enabled = false;
                        return false;
                    }
                    break;

                case enTestType.StreetTest:
                    if (!await _AppointmentService.CheckIfPassedTest(_LocalDrivingLicenseApplicationID, (int)enTestType.WrittenTest))
                    {
                        lblUserMessage.Text = "Cannot Schedule, Written Test should be passed first";
                        lblUserMessage.Visible = true;
                        btnSave.Enabled = false;
                        dtpTestDate.Enabled = false;
                        return false;
                    }
                    break;
            }

            lblUserMessage.Visible = false;
            btnSave.Enabled = true;
            dtpTestDate.Enabled = true;
            return true;
        }

        public async Task<bool> _HandleActiveTestAppointmentConstraint()
        {
            var ActiveTestAppointment = await _AppointmentService.IsThereAnActiveScheduledTest(_LocalDrivingLicenseApplicationID, (int)_enTestType);
            if (_eMode == enMode.AddNew && ActiveTestAppointment == 1)
            {
                lblUserMessage.Text = "Person Already have an active appointment for this test";
                lblUserMessage.Visible = true;
                btnSave.Enabled = false;
                dtpTestDate.Enabled = false;
                return false;
            }
            return true;
        }

        private async Task<bool> _HandleRetakeApplication()
        {
            if (_CreationMode == enCreationMode.ReTakeTestSchedule && _eMode == enMode.AddNew)
            {
                var App = new clsApplicationService(_httpClient);
                var Applicant = await App.GetApplicationByIdAsync(_localDrivingLicense.ApplicationID);
                var AppType = await _applicationTypeService.GetApplicationTypeByIdAsync((int)ApplicationTypeService.EnApplicationType.RetakeTest);

                clsApplication Application = new clsApplication();
                Application.ApplicantPersonID = Applicant.ApplicantPersonID;
                Application.ApplicationDate = DateTime.Now;
                Application.ApplicationTypeID = (int)ApplicationTypeService.EnApplicationType.RetakeTest;
                Application.ApplicationStatus = (int)clsApplicationService.EnApplicationStatus.Completed;
                Application.LastStatusDate = DateTime.Now;
                Application.PaidFees = AppType.ApplicationFees;
                Application.CreatedByUserID = clsGlobal.GetUser.ID;

                var result = await App.AddApplicationAsync(Application);
                if (result == null)
                {
                    MessageBox.Show("Failed to Create Retake Application", "Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                _CreatedRetakeApplicationID = result.ApplicationID;
                lblRetakeTestAppID.Text = _CreatedRetakeApplicationID.ToString();
            }
            return true;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (!await _HandleRetakeApplication())
                return;

            if (_eMode == enMode.AddNew)
            {
                var testApp = new TestAppointment()
                {
                    TestAppointmentID = -1,
                    TestTypeID = (int)_enTestType,
                    LocalDrivingLicenseApplicationID = _LocalDrivingLicenseApplicationID,
                    AppointmentDate = dtpTestDate.Value,
                    PaidFees = Convert.ToSingle(lblFees.Text),
                    CreatedByUserID = clsGlobal.GetUser.ID,
                    RetakeTestApplicationID = _CreatedRetakeApplicationID
                };

                var savedResult = await _AppointmentService.AddTestAppointmentAsync(testApp);

                if (savedResult != null)
                {
                    MessageBox.Show("تم حفظ موعد الاختبار بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _CloseParentFormWithSuccess();
                    await LoadData(savedResult.LocalDrivingLicenseApplicationID, savedResult.TestAppointmentID);
                }
                else
                {
                    MessageBox.Show("حدث خطأ أثناء محاولة حفظ الموعد", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                var testApp = new TestAppointment()
                {
                    TestAppointmentID = _TestAppointmentID,
                    TestTypeID = (int)_enTestType,
                    LocalDrivingLicenseApplicationID = _LocalDrivingLicenseApplicationID,
                    AppointmentDate = dtpTestDate.Value,
                    PaidFees = Convert.ToSingle(lblFees.Text),
                    CreatedByUserID = clsGlobal.GetUser.ID,
                    RetakeTestApplicationID = _TestAppointment.RetakeTestApplicationID
                };

                var updateResult = await _AppointmentService.UpdateTestAppointment(_TestAppointmentID, testApp);

                if (updateResult != null)
                {
                    MessageBox.Show("تم تعديل موعد الاختبار بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _CloseParentFormWithSuccess();
                }
                else
                {
                    MessageBox.Show("حدث خطأ أثناء محاولة تعديل الموعد", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void _CloseParentFormWithSuccess()
        {
            if (this.ParentForm != null)
            {
                this.ParentForm.DialogResult = DialogResult.OK;
                this.ParentForm.Close();
            }
        }
    }
}