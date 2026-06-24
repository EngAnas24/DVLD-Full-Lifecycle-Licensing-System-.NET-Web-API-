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
using static DVLDServices.Services.TestAppointmentService;
using static DVLDServices.Services.TestService;

namespace DVLD.Tests
{
    public partial class frmTakeTest : Form
    {
        private int _LocalDrivingLicenseApplicationID = -1;
        private enTestType _TestTypeID = enTestType.VisionTest;
        private int _AppointmentID = -1;
        private TestService _testService;
        private TestDto _Test;

        private static readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient
        {
            BaseAddress = new Uri("http://localhost:5067/")
        };

        public frmTakeTest(int LocalDrivingLicenseApplicationID, enTestType TestTypeID, int AppointmentID)
        {
            InitializeComponent();

            _LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplicationID;
            _TestTypeID = TestTypeID;
            _AppointmentID = AppointmentID;

            _testService = new TestService(_httpClient);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void frmTakeTest_Load(object sender, EventArgs e)
        {
            ctrlSecheduledTest1.TestTypeID = _TestTypeID;

            await ctrlSecheduledTest1.LoadData(_AppointmentID);

            if (ctrlSecheduledTest1.TestAppointmentID == -1)
                btnSave.Enabled = false;
            else
                btnSave.Enabled = true;

            int _TestID = ctrlSecheduledTest1.TestID;

            if (_TestID != -1)
            {
                _Test = await _testService.GetTestByIdAsync(_TestID);

                if (_Test.TestResult == 1)
                    rbPass.Checked = true;
                else
                    rbFail.Checked = true;
                txtNotes.Text = _Test.Notes;

                lblUserMessage.Visible = true;
                rbFail.Enabled = false;
                rbPass.Enabled = false;
                btnSave.Enabled = false;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to save? After that you cannot change the Pass/Fail results after you save?.",
                                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                return;
            }

            var Test = new Test()
            {
                TestAppointmentID = _AppointmentID,
                TestResult = Convert.ToByte(rbPass.Checked),
                Notes = txtNotes.Text.Trim(),
                CreatedByUserID = clsGlobal.GetUser.ID
            };

            var SaveResult = await _testService.AddTestAsync(Test);

            if (SaveResult != null)
            {
                MessageBox.Show("Data Saved Successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnSave.Enabled = false;

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Error: Data Is not Saved Successfully.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}