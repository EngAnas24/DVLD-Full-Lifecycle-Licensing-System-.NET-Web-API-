using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DVLDServices.Services.TestAppointmentService;
using static DVLDServices.Services.LocalDrivingLicesnseApplicationsService;
using DVLDServices.Services;
using ClassLibrary1.GlobalClasses;

namespace DVLD.Tests.Controls
{
    public partial class ctrlSecheduledTest : UserControl
    {
        private enTestType _TestTypeID;
        private int _TestID = -1;
        private int _TestAppointmentID = -1;
        private int _LocalDrivingLicenseApplicationID = -1;
        private LocalDrivingLicenseApplicationsDto _LocalDrivingLicenseApplication;
        private TestAppointmentService _testAppointmentService;
        private TestService _testService;
        private TestAppointmentDto _TestAppointment;
        private LocalDrivingLicesnseApplicationsService _LicesnseApplicationsService;
        private static readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient
        {
            BaseAddress = new Uri("http://localhost:5067/")
        };
        public enTestType TestTypeID
        {
            get
            {
                return _TestTypeID;
            }
            set
            {
                _TestTypeID = value;

                switch (_TestTypeID)
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

        public int TestAppointmentID
        {
            get => _TestAppointmentID;
        }
        public int TestID
        {
            get => _TestID;
        }


        

        public ctrlSecheduledTest()
        {
            InitializeComponent();
            _testAppointmentService = new TestAppointmentService(_httpClient);
            _testService = new TestService(_httpClient);
            _LicesnseApplicationsService = new LocalDrivingLicesnseApplicationsService(_httpClient);
        }


        public async Task LoadData(int TestAppointmentID) 
        {
            _TestAppointmentID = TestAppointmentID;

            _TestAppointment = await _testAppointmentService.GetTestAppointmentByIdAsync(_TestAppointmentID);

            if (_TestAppointment == null)
            {
                MessageBox.Show("Error: No Appointment ID = " + _TestAppointmentID.ToString(),
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var test = await _testService.GetTestByTestAppointmentId(_TestAppointmentID);

            if (test != null)
            {
                _TestID = test.TestID;
                lblTestID.Text = test.TestID.ToString();
            }
            else
            {
                _TestID = -1;
                lblTestID.Text = "Not Taken Yet";
            }

            _LocalDrivingLicenseApplicationID = _TestAppointment.LocalDrivingLicenseApplicationID;
            _LocalDrivingLicenseApplication = await _LicesnseApplicationsService.GetLocalApplicationByIdAsync(_LocalDrivingLicenseApplicationID);

            if (_LocalDrivingLicenseApplication == null)
            {
                MessageBox.Show("Error: No Application with ID = " + _LocalDrivingLicenseApplicationID.ToString(),
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            lblLocalDrivingLicenseAppID.Text = _LocalDrivingLicenseApplication.LocalDrivingLicenseApplicationID.ToString();
            lblDrivingClass.Text = _LocalDrivingLicenseApplication.ClassName;
            lblFullName.Text = _LocalDrivingLicenseApplication.FullName;

            lblTrial.Text = (await _testAppointmentService.GetTotalTrialsPerTest(_LocalDrivingLicenseApplicationID, (int)_TestTypeID)).ToString();

            lblDate.Text = clsFormat.DateToShort(_TestAppointment.AppointmentDate);
            lblFees.Text = _TestAppointment.PaidFees.ToString();
        }
    }
}
