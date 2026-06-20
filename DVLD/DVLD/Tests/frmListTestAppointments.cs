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
using System.Collections.Generic;
namespace DVLD.Tests
{
    public partial class frmListTestAppointments : Form
    {
        private readonly int _LocalAppID;
        private int _TestAppointmentID;
        private readonly TestAppointmentService _AppointmentService;
        private readonly clsApplicationService _applicationService;
        private readonly TestService _testService;
        private readonly LocalDrivingLicesnseApplicationsService _DrivingLicesnseApplicationsService;
        private DataTable dataTable;
        private readonly enTestType _enTestType = enTestType.VisionTest;

        private static readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient
        {
            BaseAddress = new Uri("http://localhost:5067/")
        };

        public frmListTestAppointments(int localAppID, enTestType testType)
        {
            InitializeComponent();
            _LocalAppID = localAppID;
            _AppointmentService = new TestAppointmentService(_httpClient);
            _enTestType = testType;
            _testService = new TestService(_httpClient);
            _DrivingLicesnseApplicationsService = new LocalDrivingLicesnseApplicationsService(_httpClient);
            _applicationService = new clsApplicationService(_httpClient);
        }

        public async void frmListTestAppointments_Load(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                await ctrlDrivingLicenseApplicationInfo1.LoadData(_LocalAppID);
                await LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء تحميل الصفحة: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private async  void btnAddNewAppointment_Click(object sender, EventArgs e)
        {
            frmScheduleTest frm = new frmScheduleTest(_LocalAppID, _enTestType, -1);
            frm.ShowDialog();

            await LoadData();
        }
        private async Task LoadData()
        {
            try
            {
                var testAppointments = await _AppointmentService.GetTestAppointmentByLocalAppIdAndTestTypeIdAsync(_LocalAppID, (int)_enTestType);

                dgvLicenseTestAppointments.DataSource = null;

                if (testAppointments is null || testAppointments.Count == 0)
                {
                    return;
                }

                dataTable = DatatableExtention.ToDataTable(testAppointments);

                BindingSource bindingSource = new BindingSource();
                bindingSource.DataSource = dataTable;

                dgvLicenseTestAppointments.DataSource = bindingSource;

                if (dgvLicenseTestAppointments.Columns.Count > 0)
                {
                    dgvLicenseTestAppointments.Columns["LocalDrivingLicenseApplicationID"].Visible = false;
                    dgvLicenseTestAppointments.Columns["RetakeTestApplicationID"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ أثناء تحديث بيانات الجدول: {ex.Message}", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                List<TestAppointmentDto> emptyList = new List<TestAppointmentDto>();
                BindingSource bindingSource = new BindingSource();
                bindingSource.DataSource = emptyList;

            }
        }

        private async void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvLicenseTestAppointments.CurrentRow == null) return;

            _TestAppointmentID = (int)dgvLicenseTestAppointments.CurrentRow.Cells["TestAppointmentID"].Value;
            int LocalAppID = (int)dgvLicenseTestAppointments.CurrentRow.Cells["LocalDrivingLicenseApplicationID"].Value;

            frmScheduleTest frmSchedule = new frmScheduleTest(LocalAppID, _enTestType, _TestAppointmentID);

            if (frmSchedule.ShowDialog() == DialogResult.OK)
            {
                await LoadData();
            }
            else
            {
                await Task.Delay(200);
                await LoadData();
            }
        }

        private async void takeTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvLicenseTestAppointments.CurrentRow == null) return;

            _TestAppointmentID = (int)dgvLicenseTestAppointments.CurrentRow.Cells["TestAppointmentID"].Value;

            frmTakeTest frmTake = new frmTakeTest(_LocalAppID, _enTestType, _TestAppointmentID);

            if (frmTake.ShowDialog() == DialogResult.OK)
            {
                await LoadData();
            }
            else
            {
                await Task.Delay(200);
                await LoadData();
            }
        }
    }
}