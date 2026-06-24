using DVLD.Licenses;
using DVLD.Licenses.Local_Licenses;
using DVLD.Tests;
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
using static DVLDServices.Services.clsApplicationService;
using static DVLDServices.Services.LocalDrivingLicesnseApplicationsService;
using static DVLDServices.Services.TestAppointmentService;
namespace DVLD.Applications.Local_Driving_License
{
    public partial class frmListLocalDrivingLicesnseApplications : Form
    {
        private readonly LocalDrivingLicesnseApplicationsService _LocalappService;
        private DataTable dataTable;
        private TestAppointmentService _testAppointmentService;
        private clsApplicationService _applicationService;
        private clsApplicationService.EnApplicationStatus EnApplicationStatus = EnApplicationStatus.New;
        private enTestType _enTestType= enTestType.VisionTest;
        private LicenseService _licenseService;
        private static readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient
        {
            BaseAddress = new Uri("http://localhost:5067/") 
        };
        public frmListLocalDrivingLicesnseApplications()
        {
            InitializeComponent();
            _LocalappService = new LocalDrivingLicesnseApplicationsService(_httpClient);
            _applicationService = new clsApplicationService(_httpClient);
            _testAppointmentService = new TestAppointmentService(_httpClient);
            cmsApplications.Opening += cmsApplications_Opening;
            _licenseService = new LicenseService(_httpClient);

        }

        private void frmListLocalDrivingLicesnseApplications_Load(object sender, EventArgs e)
        {
            if (cbFilterBy.Items.Count > 0)
                cbFilterBy.SelectedIndex = 0;

            if (cbFilterBy.Text == "None")
                txtFilterValue.Visible = false;

            _LoadData();

        }

        private async void cmsApplications_Opening(object sender, CancelEventArgs e)
        {
            try
            {
                if (dgvLocalDrivingLicenseApplications.CurrentRow == null)
                {
                    e.Cancel = true;
                    return;
                }

                int localAppID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells["LocalDrivingLicenseApplicationID"].Value;
                int applicationID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells["ApplicationID"].Value;

                ResetContextMenuItems(true);

                var visionTestTask = _testAppointmentService.CheckIfPassedTest(localAppID, (int)TestAppointmentService.enTestType.VisionTest);
                var writtenTestTask = _testAppointmentService.CheckIfPassedTest(localAppID, (int)TestAppointmentService.enTestType.WrittenTest);
                var streetTestTask = _testAppointmentService.CheckIfPassedTest(localAppID, (int)TestAppointmentService.enTestType.StreetTest);
                var localAppTask = _LocalappService.GetLocalApplicationByIdAsync(localAppID);
                var licenseTask = _licenseService.GetLicenseByApplicationIdAsync(applicationID);

                await Task.WhenAll(visionTestTask, writtenTestTask, streetTestTask, localAppTask, licenseTask);

                bool passedVisionTest = visionTestTask.Result;
                bool passedWrittenTest = writtenTestTask.Result;
                bool passedStreetTest = streetTestTask.Result;
                var localApp = localAppTask.Result;
                var licenseInfo = licenseTask.Result;

                var app = await _applicationService.GetApplicationByIdAsync(localApp.ApplicationID);

                int licenseID = -1;
                if (licenseInfo != null)
                {
                    licenseID = await _licenseService.GetLicenseByPersonIdAsync(licenseInfo.ID, licenseInfo.LicenseClassID);
                }


                if (app.ApplicationStatus == (int)EnApplicationStatus.Cancelled)
                {
                    ResetContextMenuItems(false); 
                    showDetailsToolStripMenuItem.Enabled = true; 
                    showPersonLicenseHistoryToolStripMenuItem.Enabled = true; 
                    return; 
                }

                else if (app.ApplicationStatus == (int)EnApplicationStatus.Completed || licenseID > 0)
                {
                    CancelApplicaitonToolStripMenuItem.Enabled = false;
                    DeleteApplicationToolStripMenuItem.Enabled = false;
                    editToolStripMenuItem.Enabled = false;
                    ScheduleTestsMenue.Enabled = false;
                    issueDrivingLicenseFirstTimeToolStripMenuItem.Enabled = false;

                    showLicenseToolStripMenuItem.Enabled = true;
                    showDetailsToolStripMenuItem.Enabled = true;
                    showPersonLicenseHistoryToolStripMenuItem.Enabled = true;
                }

                else if (app.ApplicationStatus == (int)EnApplicationStatus.New)
                {
                    CancelApplicaitonToolStripMenuItem.Enabled = true;
                    DeleteApplicationToolStripMenuItem.Enabled = true;
                    editToolStripMenuItem.Enabled = true;

                    showLicenseToolStripMenuItem.Enabled = false;

                    if (passedStreetTest)
                    {
                        issueDrivingLicenseFirstTimeToolStripMenuItem.Enabled = true;
                        ScheduleTestsMenue.Enabled = false;
                    }
                    else
                    {
                        issueDrivingLicenseFirstTimeToolStripMenuItem.Enabled = false;

                        ScheduleTestsMenue.Enabled = (!passedVisionTest || !passedWrittenTest || !passedStreetTest);
                        if (ScheduleTestsMenue.Enabled)
                        {
                            scheduleVisionTestToolStripMenuItem.Enabled = !passedVisionTest;
                            scheduleWrittenTestToolStripMenuItem.Enabled = passedVisionTest && !passedWrittenTest;
                            scheduleStreetTestToolStripMenuItem.Enabled = passedVisionTest && passedWrittenTest && !passedStreetTest;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ غير متوقع أثناء تحديث قائمة الخيارات: {ex.Message}", "خطأ في النظام", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void ResetContextMenuItems(bool enabled)
        {
            ScheduleTestsMenue.Enabled = enabled;
            issueDrivingLicenseFirstTimeToolStripMenuItem.Enabled = enabled;
            CancelApplicaitonToolStripMenuItem.Enabled = enabled;
            DeleteApplicationToolStripMenuItem.Enabled = enabled;
            editToolStripMenuItem.Enabled = enabled;
            showLicenseToolStripMenuItem.Enabled = enabled;
            showDetailsToolStripMenuItem.Enabled = enabled;
            showPersonLicenseHistoryToolStripMenuItem.Enabled = enabled;
        }
        
        private async void _LoadData()
        {
            try
            {
            var LocalApps = await _LocalappService.GetLocalApplicationsAsync();
            dataTable = DatatableExtention.ToDataTable(LocalApps);
            dgvLocalDrivingLicenseApplications.DataSource = dataTable;
            }
            catch
            {
                List<LocalDrivingLicenseApplicationsDto> emptyList
                    = new List<LocalDrivingLicenseApplicationsDto>();
                dgvLocalDrivingLicenseApplications.DataSource = emptyList;
            }
            finally
            {
                if (cbFilterBy.Items.Count > 0)
                    cbFilterBy.SelectedIndex = 0;

                if (cbFilterBy.Text == "None")
                    txtFilterValue.Visible = false;
            }
        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int LocalAppID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells["LocalDrivingLicenseApplicationID"].Value;
            frmLocalDrivingLicenseApplicationInfo frmLocalDrivingLicense = new frmLocalDrivingLicenseApplicationInfo(LocalAppID);
            frmLocalDrivingLicense.ShowDialog();
        }

        private void dgvLocalDrivingLicenseApplications_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int LocalAppID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells["LocalDrivingLicenseApplicationID"].Value;
            frmAddUpdateLocalDrivingLicesnseApplication frmAddUpdateLocal =
                new frmAddUpdateLocalDrivingLicesnseApplication(LocalAppID);
           
            frmAddUpdateLocal.ShowDialog();
            _LoadData();

        }

        private async void DeleteApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvLocalDrivingLicenseApplications.CurrentRow == null) return;

            int localAppId = Convert.ToInt32(dgvLocalDrivingLicenseApplications.CurrentRow.Cells["LocalDrivingLicenseApplicationID"].Value);

            DialogResult dialogResult = MessageBox.Show("هل أنت متأكد من حذف هذا التقديم والطلب المرتبط به نهائياً؟", "تأكيد الحذف", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult != DialogResult.Yes)
            {
                MessageBox.Show("تم إلغاء عملية الحذف.", "ملغى", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var localApp = await _LocalappService.GetLocalApplicationByIdAsync(localAppId);
            if (localApp == null || localApp.ApplicationID == 0)
            {
                MessageBox.Show("خطأ: لم يتم العثور على بيانات الطلب الأساسي المرتبط (ApplicationID غير صالح).", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool isLocalAppDeleted = await DVLDServices.Commons.clsFormHelper.ExecuteSafeDeleteAsync(
                async () => await _LocalappService.DeleteLocalApplicationHttpResponseAsync(localAppId),
                "طلب رخصة القيادة المحلية"
            );

            if (!isLocalAppDeleted) return;

            try
            {
                var response = await _applicationService.DeleteApplicationHttpResponseAsync(localApp.ApplicationID);

                if (response.IsSuccessStatusCode || response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _LoadData();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    MessageBox.Show("تم حذف الطلب المحلي، لكن تعذر حذف الطلب العام لارتباطه بسجلات أخرى في النظام.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _LoadData();
                }
            }
            catch
            {
                _LoadData();
            }
        }
        private async void CancelApplicaitonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int LocalAppID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells["LocalDrivingLicenseApplicationID"].Value;
            var LocalApp = await _LocalappService.GetLocalApplicationByIdAsync(LocalAppID);

            var App = await _applicationService.GetApplicationByIdAsync(LocalApp.ApplicationID);
            App.ApplicationStatus = (int)clsApplicationService.EnApplicationStatus.Cancelled;
            EnApplicationStatus = EnApplicationStatus.Cancelled;
            DialogResult result = MessageBox.Show("هل انت متاكد من الفاء التقديم؟", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
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
                MessageBox.Show("تم الإلغاء بنجاح", "", 0, MessageBoxIcon.Information);
                _LoadData();
            }
        }

        private void _ScheduleTest(enTestType TestType)
        {
            int LocalAppID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells["LocalDrivingLicenseApplicationID"].Value;
            frmListTestAppointments listTestAppointments = new frmListTestAppointments(LocalAppID, TestType);
            listTestAppointments.ShowDialog();
            _LoadData();
        }

        private void scheduleVisionTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ScheduleTest(enTestType.VisionTest);
        }

        private void scheduleWrittenTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ScheduleTest(enTestType.WrittenTest);

        }

        private void scheduleStreetTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _ScheduleTest(enTestType.StreetTest);

        }

        private void issueDrivingLicenseFirstTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int LocalDrivingLicenseApplicationID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells[0].Value;
            frmIssueDriverLicenseFirstTime frm = new frmIssueDriverLicenseFirstTime(LocalDrivingLicenseApplicationID);
            frm.ShowDialog();
            _LoadData();

            //refresh
        }

        private async void showLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int ApplicationID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells["ApplicationID"].Value;
            var licinfo = await _licenseService.GetLicenseByApplicationIdAsync(ApplicationID);     
            int licID = await _licenseService.GetLicenseByPersonIdAsync(licinfo.ID, licinfo.LicenseClassID);

            frmShowLicenseInfo showLicenseInfo = new frmShowLicenseInfo(licID);
            showLicenseInfo.ShowDialog();
        }

        private async void showPersonLicenseHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int LocalAppID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells["LocalDrivingLicenseApplicationID"].Value;
            var LocalApp = await _LocalappService.GetLocalApplicationByIdAsync(LocalAppID);
            var App = await _applicationService.GetApplicationByIdAsync(LocalApp.ApplicationID);
            frmShowPersonLicenseHistory personLicenseHistory = new frmShowPersonLicenseHistory(App.ApplicantPersonID);
            personLicenseHistory.ShowDialog();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAddNewApplication_Click(object sender, EventArgs e)
        {
            frmAddUpdateLocalDrivingLicesnseApplication FrmAdd = new frmAddUpdateLocalDrivingLicesnseApplication();
            FrmAdd.ShowDialog();

            _LoadData();

        }

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dataTable == null)
                return;

            txtFilterValue.Visible = (cbFilterBy.Text != "None");

            if (txtFilterValue.Visible)
            {
                txtFilterValue.Text = "";
                txtFilterValue.Focus();
            }

            dataTable.DefaultView.RowFilter = "";

            // 2. Safety Check: Make sure the GridView exists before counting rows
            if (dgvLocalDrivingLicenseApplications != null)
            {
                lblRecordsCount.Text = dgvLocalDrivingLicenseApplications.Rows.Count.ToString();
            }
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {
            string FilterColumn = "";
            //Map Selected Filter to real Column name 
            switch (cbFilterBy.Text)
            {
                case "L.D.L.AppID":
                    FilterColumn = "LocalDrivingLicenseApplicationID";
                    break;

                case "National No.":
                    FilterColumn = "NationalNo";
                    break;

                case "Full Name":
                    FilterColumn = "FullName";
                    break;

                case "Status":
                    FilterColumn = "Status";
                    break;

                default:
                    FilterColumn = "None";
                    break;
            }

            //Reset the filters in case nothing selected or filter value conains nothing.
            if (txtFilterValue.Text.Trim() == "" || FilterColumn == "None")
            {
                dataTable.DefaultView.RowFilter = "";
                lblRecordsCount.Text = dgvLocalDrivingLicenseApplications.Rows.Count.ToString();
                return;
            }

            if (FilterColumn == "LocalDrivingLicenseApplicationID")
            {
                // التحقق مما إذا كانت القيمة المدخلة رقماً فعلاً لمنع الخطأ
                if (int.TryParse(txtFilterValue.Text.Trim(), out int _))
                {
                    dataTable.DefaultView.RowFilter = string.Format("[{0}] = {1}", FilterColumn, txtFilterValue.Text.Trim());
                }
                else
                {
                    dataTable.DefaultView.RowFilter = "";
                }
            }
            else
                dataTable.DefaultView.RowFilter = string.Format("[{0}] LIKE '{1}%'", FilterColumn, txtFilterValue.Text.Trim());

            lblRecordsCount.Text = dgvLocalDrivingLicenseApplications.Rows.Count.ToString();
        }
    }
}
