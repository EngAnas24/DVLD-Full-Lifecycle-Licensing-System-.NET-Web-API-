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

namespace DVLD.Tests
{
    public partial class frmScheduleTest : Form
    {

        private int _LocalDrivingLicenseApplicationID = -1;
        private enTestType _TestTypeID = enTestType.VisionTest;
        private int _AppointmentID = -1;


        public frmScheduleTest(int LocalDrivingLicenseApplicationID, enTestType TestTypeID, int AppointmentID = -1)
        {

            InitializeComponent();

            _LocalDrivingLicenseApplicationID = LocalDrivingLicenseApplicationID;
            _TestTypeID = TestTypeID;
            _AppointmentID = AppointmentID;


        }


        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmScheduleTest_Load(object sender, EventArgs e)
        {
            crlScheduleTest1.enTestType = _TestTypeID;
            crlScheduleTest1.LoadData(_LocalDrivingLicenseApplicationID, _AppointmentID);
        }
    }
}
