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
        private int _LocalAppID;
        private ApplicationTypeService _applicationTypeService;
        private LocalDrivingLicesnseApplicationsService _localDrivingLicesnseService;
        private TestAppointmentService _AppointmentService;
        private static readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient
        {
            BaseAddress = new Uri("http://localhost:5067/") // تأكد من وجود الـ / في النهاية
        };

        public enum enMode { AddNew = 0, Update = 1 };
        public enMode _eMode = enMode.AddNew;
        public enum enCreationMode { FirstTimeSchedule = 0, ReTakeTestSchedule = 1 }
        private enCreationMode _CreationMode = enCreationMode.FirstTimeSchedule;
        private enTestType _enTestType = enTestType.VisionTest;
        private LocalDrivingLicenseApplicationsDto _localDrivingLicense;
        private int _LocalDrivingLicenseApplicationID = -1;
        private TestAppointmentDto _TestAppointment;
        private int _TestAppointmentID = -1;
        private TestTypeService _TestTypeService;

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
                        {
                            gbTestType.Text = "Vision Test";

                            break;
                        }

                    case enTestType.WrittenTest:
                        {
                            gbTestType.Text = "Written Test";
                            break;
                        }

                    case enTestType.StreetTest:
                        {
                            gbTestType.Text = "Street Test";
                            break;
                        }
                }

            }
        }
        public async void LoadData(int LocalDrivingLicenseApplicationID, int AppointmentID = -1)
        {
            if (AppointmentID <= 0)
                _eMode = enMode.AddNew;
            else
                _eMode = enMode.Update;

            _LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplicationID;
            _TestAppointmentID = AppointmentID;
            _localDrivingLicense = await _localDrivingLicesnseService.GetLocalApplicationByIdAsync(_LocalDrivingLicenseApplicationID);
            if (_localDrivingLicense == null)
            {
                MessageBox.Show("Error: No Local Driving License Application with ID = " + _LocalDrivingLicenseApplicationID.ToString(),
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnSave.Enabled = false;
                return;
            }

            if (await _AppointmentService.DoesAttendTestType(LocalDrivingLicenseApplicationID, (int)_enTestType) == 1)

                _CreationMode = enCreationMode.ReTakeTestSchedule;
            else
                _CreationMode = enCreationMode.FirstTimeSchedule;


            if (_CreationMode == enCreationMode.ReTakeTestSchedule)
            {
                var AppType = await _applicationTypeService.GetApplicationTypeByIdAsync((int)ApplicationTypeService.EnApplicationType.RetakeTest);
                lblRetakeAppFees.Text = AppType.ApplicationFees.ToString();
                gbRetakeTestInfo.Enabled = true;
                lblTitle.Text = "Schedule Retake Test";
                lblRetakeTestAppID.Text = "0";
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
            var test = await _AppointmentService.
               GetTotalTrialsPerTest(LocalDrivingLicenseApplicationID, (int)enTestType);
            lblTrial.Text = test.ToString();


            if (_eMode == enMode.AddNew)
            {
                var TestType = await _TestTypeService.GetTestTypeByIdAsync((int)_enTestType);
                lblFees.Text = TestType.TestTypeFees.ToString();
                dtpTestDate.MinDate = DateTime.Now;
                lblRetakeTestAppID.Text = "N/A";
            }
            else
            {
                if (!await _LoadTestAppointmentData())
                    return;
            }

            lblTotalFees.Text = (Convert.ToSingle(lblFees.Text) + Convert.ToSingle(lblRetakeAppFees.Text)).ToString();

            if (!await _HandleActiveTestAppointmentConstraint())
                return;

            if (!await _HandleAppointmentLockedConstraint())
                return;

            if (!await _HandlePrviousTestConstraint())
                return;



        }
        private async Task<bool> _HandlePrviousTestConstraint()
        {
            switch (_enTestType)
            {
                case enTestType.VisionTest:
                    lblUserMessage.Visible = false;
                    return true;

                case enTestType.WrittenTest:
                    if (await _AppointmentService.DoesAttendTestType(_LocalDrivingLicenseApplicationID, (int)_enTestType) != 1)
                    {
                        lblUserMessage.Text = "Cannot Sechule, Vision Test should be passed first";
                        lblUserMessage.Visible = true;
                        btnSave.Enabled = false;
                        dtpTestDate.Enabled = false;
                        return false;
                    }
                    else
                    {
                        lblUserMessage.Enabled = false;
                        btnSave.Enabled = true;
                        dtpTestDate.Enabled = true;
                    }
                    return true;

                case enTestType.StreetTest:
                    if (await _AppointmentService.DoesAttendTestType(_LocalDrivingLicenseApplicationID, (int)_enTestType) != 1)
                    {
                        lblUserMessage.Text = "Cannot Sechule, Written Test should be passed first";
                        lblUserMessage.Visible = true;
                        btnSave.Enabled = false;
                        dtpTestDate.Enabled = false;
                        return false;
                    }
                    else
                    {
                        lblUserMessage.Enabled = false;
                        btnSave.Enabled = true;
                        dtpTestDate.Enabled = true;
                    }
                    return true;
            }
            return true;
        }

        private async Task<bool> _HandleAppointmentLockedConstraint()
        {
            // 1. التحقق من أن الكائن ليس null
            // إذا كان null، فهذا يعني أننا في وضع الإضافة (AddNew) والموعد بالتأكيد ليس مقفلاً
            if (_TestAppointment == null)
            {
                lblUserMessage.Visible = false;
                return false; // أو true حسب منطقك في التسمية، المهم ألا ينهار البرنامج
            }

            // 2. الآن يمكننا فحص الخاصية بأمان
            if (_TestAppointment.IsLocked == 1)
            {
                lblUserMessage.Visible = true;
                lblUserMessage.Text = "Person already sat for the test, appointment locked.";
                dtpTestDate.Enabled = false;
                btnSave.Enabled = false;
                return true; // نعم، يوجد قيد (Constraint) والموعد مقفل
            }
            else
            {
                lblUserMessage.Visible = false;
            }

            return false;
        }

        private async Task<bool> _LoadTestAppointmentData()
        {
            _TestAppointment = await _AppointmentService.GetTestAppointmentByIdAsync(_TestAppointmentID);
            if (_TestAppointment == null)
            {
                MessageBox.Show("Error: No Appointment with ID = " + _TestAppointmentID.ToString(),
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnSave.Enabled = false;
                return false;
            }

            lblFees.Text = _TestAppointment.PaidFees.ToString();
            if (DateTime.Compare(DateTime.Now, _TestAppointment.AppointmentDate) < 0)
                dtpTestDate.MinDate = DateTime.Now;
            else
                dtpTestDate.MinDate = _TestAppointment.AppointmentDate;

            dtpTestDate.Value = _TestAppointment.AppointmentDate;

            if (_TestAppointment.RetakeTestApplicationID == -1)
            {
                lblRetakeAppFees.Text = "0";
                lblRetakeTestAppID.Text = "N/A";
            }
            else
            {
                var AppType = await _applicationTypeService.GetApplicationTypeByIdAsync((int)ApplicationTypeService.EnApplicationType.RetakeTest);
                lblRetakeAppFees.Text = AppType.ApplicationFees.ToString();
                gbRetakeTestInfo.Enabled = true;
                lblRetakeTestAppID.Text = _TestAppointment.RetakeTestApplicationID.ToString();
                lblTitle.Text = "Schedule Retake Test";
            }

            return true;
        }
        public async Task<bool> _HandleActiveTestAppointmentConstraint()
        {
            var ActiveTestAppointment = await _AppointmentService.IsThereAnActiveScheduledTest(_LocalDrivingLicenseApplicationID, (int)_enTestType);
            if (_eMode == enMode.AddNew && ActiveTestAppointment ==1)
            {
                lblUserMessage.Text = "Person Already have an active appointment for this test";
                btnSave.Enabled = false;
                dtpTestDate.Enabled = false;
                return false;
            }
            return true;
        }
        private async Task<bool> _HandleRetakeApplication()
        {
            if (_eMode == enMode.AddNew && _CreationMode == enCreationMode.ReTakeTestSchedule)
            {
                var App = new clsApplicationService(_httpClient);
                var Applicant = await App.GetApplicationByIdAsync(_localDrivingLicense.ApplicationID);

                clsApplication Application = new clsApplication();
                Application.ApplicantPersonID = Applicant.ApplicantPersonID;
                Application.ApplicationDate = DateTime.Now;
                Application.ApplicationTypeID = (int)ApplicationTypeService.EnApplicationType.RetakeTest;
                Application.ApplicationStatus = (int)clsApplicationService.EnApplicationStatus.Completed;
                Application.LastStatusDate = DateTime.Now;
                Application.PaidFees = Applicant.PaidFees;
                Application.CreatedByUserID = clsGlobal.GetUser.ID;

                var result = await App.AddApplicationAsync(Application);
                if (result == null)
                {
                    _TestAppointment.RetakeTestApplicationID = -1;
                    MessageBox.Show("Faild to Create application", "Faild", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                _TestAppointment.RetakeTestApplicationID = Application.ApplicationID;

            }
            return true;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            int retakeAppID = -1;
            if (int.TryParse(lblRetakeTestAppID.Text, out int result))
            {
                retakeAppID = result;
            }

            // 2. بناء كائن الموعد
            var testApp = new TestAppointment()
            {
                TestAppointmentID = _TestAppointmentID,
                TestTypeID = (int)_enTestType,
                LocalDrivingLicenseApplicationID = _LocalDrivingLicenseApplicationID,
                AppointmentDate = dtpTestDate.Value,
                PaidFees = Convert.ToSingle(lblFees.Text),
                CreatedByUserID = clsGlobal.GetUser.ID,
                RetakeTestApplicationID = retakeAppID
            };

            if (_eMode == enMode.AddNew)
            {
                var savedResult = await _AppointmentService.AddTestAppointmentAsync(testApp);

                if (savedResult != null)
                {
                    MessageBox.Show("تم حفظ موعد الاختبار بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _TestAppointmentID = savedResult.TestAppointmentID;
                    _eMode = enMode.Update;
                }
                else
                {
                    MessageBox.Show("حدث خطأ أثناء محاولة حفظ الموعد", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                var updateResult = await _AppointmentService.UpdateTestAppointment(_TestAppointmentID, testApp);
                if (updateResult != null)
                {
                    MessageBox.Show("تم تعديل موعد الاختبار بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _TestAppointmentID = updateResult.TestAppointmentID;
                }
                else
                {
                    MessageBox.Show("حدث خطأ أثناء محاولة تعديل الموعد", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }

        }



    }
}
