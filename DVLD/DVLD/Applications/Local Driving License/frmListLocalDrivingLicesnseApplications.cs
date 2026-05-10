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
            int LocalAppID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells["LocalDrivingLicenseApplicationID"].Value;



            bool PassedVisionTest = await _testAppointmentService.CheckIfPassedTest(LocalAppID, (int)TestAppointmentService.enTestType.VisionTest);
            bool PassedWrittenTest = await _testAppointmentService.CheckIfPassedTest(LocalAppID, (int)TestAppointmentService.enTestType.WrittenTest);
            bool PassedStreetTest = await _testAppointmentService.CheckIfPassedTest(LocalAppID, (int)TestAppointmentService.enTestType.StreetTest);

            ScheduleTestsMenue.Enabled = (!PassedVisionTest || !PassedWrittenTest || !PassedStreetTest);

            var LocalApp = await _LocalappService.GetLocalApplicationByIdAsync(LocalAppID);
            var App = await _applicationService.GetApplicationByIdAsync(LocalApp.ApplicationID);

            if (App.ApplicationStatus == (int)EnApplicationStatus.Cancelled)
            {
                ScheduleTestsMenue.Enabled = false;
                issueDrivingLicenseFirstTimeToolStripMenuItem.Enabled = false;
                CancelApplicaitonToolStripMenuItem.Enabled = false;
                DeleteApplicationToolStripMenuItem.Enabled = false;
                editToolStripMenuItem.Enabled = false;
                showLicenseToolStripMenuItem.Enabled = false;

            }
            if (App.ApplicationStatus == (int)clsApplicationService.EnApplicationStatus.New)
            {
                CancelApplicaitonToolStripMenuItem.Enabled = true;
                DeleteApplicationToolStripMenuItem.Enabled = true;
                editToolStripMenuItem.Enabled = true;
            }


            if (!PassedStreetTest)
            {
                issueDrivingLicenseFirstTimeToolStripMenuItem.Enabled = false;
                showLicenseToolStripMenuItem.Enabled = false;
            }
            if (ScheduleTestsMenue.Enabled)
            {
                scheduleVisionTestToolStripMenuItem.Enabled = !PassedVisionTest;
                scheduleWrittenTestToolStripMenuItem.Enabled = PassedVisionTest && !PassedWrittenTest;
                scheduleStreetTestToolStripMenuItem.Enabled = PassedVisionTest && PassedWrittenTest && !PassedStreetTest;

            }

            int ApplicationID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells["ApplicationID"].Value;
            var licinfo = await _licenseService.GetLicenseByApplicationIdAsync(ApplicationID);
            int licID = await _licenseService.GetLicenseByPersonIdAsync(licinfo.ID, licinfo.LicenseClassID);

            if (licID > 0)
            {
                issueDrivingLicenseFirstTimeToolStripMenuItem.Enabled = false;
                CancelApplicaitonToolStripMenuItem.Enabled = false;
                DeleteApplicationToolStripMenuItem.Enabled = false;
                editToolStripMenuItem.Enabled = false;
            }
              

        }

        private async void _LoadData()
        {
            var LocalApps = await _LocalappService.GetLocalApplicationsAsync();
            dataTable = DatatableExtention.ToDataTable(LocalApps);
            dgvLocalDrivingLicenseApplications.DataSource = dataTable;
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
            frmAddUpdateLocal.frmAddUpdateLocalDrivingLicesnseApplication_Load(null, null);
        }

        private async void DeleteApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int LocalAppID = (int)dgvLocalDrivingLicenseApplications.CurrentRow.Cells["LocalDrivingLicenseApplicationID"].Value;
            var LocalApp = await _LocalappService.GetLocalApplicationByIdAsync(LocalAppID);
            DialogResult result = MessageBox.Show("هل انت متاكد من حذف التقديم؟", "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if (LocalApp.ApplicationID == 0)
                {
                    MessageBox.Show("ApplicationID غير صالح");
                    return;
                }

                await _LocalappService.DeleteLocalApplication(LocalAppID);
                await _applicationService.DeleteApplicationAsync(LocalApp.ApplicationID);

                MessageBox.Show("تم الحذف بنجاح", "", 0, MessageBoxIcon.Information);
                _LoadData();
            }
            else
            {
                MessageBox.Show("تم الغاء الحذف", "", 0, MessageBoxIcon.Information);
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
            listTestAppointments.frmListTestAppointments_Load(null, null);
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
            //refresh
            frmListLocalDrivingLicesnseApplications_Load(null, null);
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
            frmListLocalDrivingLicesnseApplications_Load(null, null);
        }

        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 1. Safety Check: If dataTable hasn't been initialized yet, stop here.
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
