using DVLD.Licenses.Local_Licenses;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD.Applications.Local_Driving_License
{
    public partial class frmLocalDrivingLicenseApplicationInfo : Form
    {
        private readonly int _LocalAppID;
        
        public frmLocalDrivingLicenseApplicationInfo(int localAppID)
        {
            InitializeComponent();
            _LocalAppID = localAppID;
        }

        private async void frmLocalDrivingLicenseApplicationInfo_Load(object sender, EventArgs e)
        {
            // تأكد الكنترول موجود
            if (ctrlDrivingLicenseApplicationInfo1 != null)
            {
                await ctrlDrivingLicenseApplicationInfo1.LoadData(_LocalAppID);
            }
            else
            {
                MessageBox.Show("ctrlDrivingLicenseApplicationInfo1 is NULL");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}