using DVLDServices.Extentions;
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
using static DVLDServices.Services.LocalDrivingLicesnseApplicationsService;
using static DVLDServices.Services.TestAppointmentService;
using static DVLDServices.Services.TestService;

namespace DVLD.Tests
{
    public partial class frmListTestAppointments : Form
    {
        private int _LocalAppID;
        private int _TestAppointmentID;
        private TestAppointmentService _AppointmentService;
        private clsApplicationService _applicationService;
        private TestService _testService;
        private LocalDrivingLicesnseApplicationsService _DrivingLicesnseApplicationsService;
        DataTable dataTable;
        private enTestType _enTestType =  enTestType.VisionTest;
        private static readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient
        {
            BaseAddress = new Uri("http://localhost:5067/") // تأكد من وجود الـ / في النهاية
        };

        public frmListTestAppointments(int LocalAppID, enTestType testType)
        {
            InitializeComponent();
            _LocalAppID = LocalAppID;
            _AppointmentService = new TestAppointmentService(_httpClient);
            _enTestType = testType;
            _testService = new TestService(_httpClient);
            _DrivingLicesnseApplicationsService = new LocalDrivingLicesnseApplicationsService(_httpClient);
            _applicationService = new clsApplicationService(_httpClient);
        }

        public async void frmListTestAppointments_Load(object sender, EventArgs e)
        {
          await  ctrlDrivingLicenseApplicationInfo1.LoadData(_LocalAppID);
                 LoadData();
        }

        private async void btnAddNewAppointment_Click(object sender, EventArgs e)
        {
            var localapp = await _DrivingLicesnseApplicationsService.GetLocalApplicationByIdAsync(_LocalAppID);
            var app = await _applicationService.GetApplicationByIdAsync(localapp.ApplicationID);
            int isFound  = await _AppointmentService.IsThereAnActiveScheduledTest(_LocalAppID, (int)_enTestType);
            if (isFound == 1)
            {
                MessageBox.Show("Person Already have an active appointment for this test, You cannot add new appointment", "Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var Class = await _testService.GetClassID(localapp.ClassName);
            LastTestResult LastTest = await _testService.GetLastTest(Class.LicenseClassID, app.ApplicantPersonID, (int)_enTestType);

            if (LastTest == null)
            {
                frmScheduleTest frm1 = new frmScheduleTest(_LocalAppID,_enTestType,_TestAppointmentID);
                frm1.ShowDialog();
                frmListTestAppointments_Load(null, null);
                return;
            }

            //if person already passed the test s/he cannot retak it.
            if (LastTest.TestResult == true)
            {
                MessageBox.Show("This person already passed this test before, you can only retake faild test", "Not Allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            frmScheduleTest frm2 = new frmScheduleTest
                (_LocalAppID,_enTestType,_TestAppointmentID);
            frm2.ShowDialog();
            frmListTestAppointments_Load(null, null);
            //---
        }

        private async void LoadData()
        {
            try
            {
                var TestAppointment = await _AppointmentService.GetTestAppointmentByLocalAppIdAndTestTypeIdAsync(_LocalAppID, (int)_enTestType);

                if (TestAppointment is null || TestAppointment.Count == 0)
                {
                    dgvLicenseTestAppointments.DataSource = null;
                    return;
                }

                dataTable = DatatableExtention.ToDataTable(TestAppointment);
                dgvLicenseTestAppointments.DataSource = dataTable;

                if (dgvLicenseTestAppointments.Columns.Count > 0)
                {
                    dgvLicenseTestAppointments.Columns["TestTypeID"].Visible = false;
                    dgvLicenseTestAppointments.Columns["LocalDrivingLicenseApplicationID"].Visible = false;
                    dgvLicenseTestAppointments.Columns["CreatedByUserID"].Visible = false;
                    dgvLicenseTestAppointments.Columns["RetakeTestApplicationID"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _TestAppointmentID = (int) dgvLicenseTestAppointments.CurrentRow.Cells["TestAppointmentID"].Value;

            frmScheduleTest frmSchedule = new frmScheduleTest(_LocalAppID, _enTestType,_TestAppointmentID);
            frmSchedule.ShowDialog();
            frmListTestAppointments_Load(null, null);
        }

        private void takeTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _TestAppointmentID = (int)dgvLicenseTestAppointments.CurrentRow.Cells["TestAppointmentID"].Value;
            frmTakeTest frmTake = new frmTakeTest(_LocalAppID, _enTestType, _TestAppointmentID);
            frmTake.ShowDialog();
            frmListTestAppointments_Load(null, null);
        }
    }
}
