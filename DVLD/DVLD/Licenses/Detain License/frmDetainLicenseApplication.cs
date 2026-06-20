using ClassLibrary1.Services;
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

namespace DVLD.Licenses.Detain_License
{
    public partial class frmDetainLicenseApplication : Form
    {
        private int _SelectedLicenseID = -1;
        DetainedLicenseService _detainedLicenseService;
        ApplicationTypeService _applicationTypeService;
        LicenseService _licenseService;
        clsApplicationService _applicationService;
        private enum eMod { Add, Update };
        private static readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient
        {
            BaseAddress = new Uri("http://localhost:5067/")
        };
        public frmDetainLicenseApplication()
        {
            InitializeComponent();
            _detainedLicenseService = new DetainedLicenseService(_httpClient);
            _applicationTypeService = new ApplicationTypeService(_httpClient);
            _licenseService = new LicenseService(_httpClient);
            _applicationService = new clsApplicationService(_httpClient);

        }
        public frmDetainLicenseApplication(int LicenseID)
        {
            InitializeComponent();
            _SelectedLicenseID = LicenseID;
            ctrlDriverLicenseInfoWithFilter1.LoadLicenseInfo(_SelectedLicenseID);
            ctrlDriverLicenseInfoWithFilter1.FilterEnabled = false;
            _detainedLicenseService = new DetainedLicenseService(_httpClient);
            _applicationTypeService = new ApplicationTypeService(_httpClient);
            _licenseService = new LicenseService(_httpClient);
            _applicationService = new clsApplicationService(_httpClient);

        }
        private void frmDetainLicenseApplication_Load(object sender, EventArgs e)
        {
            lblLicenseID.Text = _SelectedLicenseID.ToString();
            lblCreatedByUser.Text = clsGlobal.GetUser.UserName;
            lblDetainDate.Text = DateTime.Now.ToString();
           
        }

        private async void ctrlDriverLicenseInfoWithFilter1_OnLicenseSelected(int obj)
        {
            _SelectedLicenseID = obj;

            lblLicenseID.Text = _SelectedLicenseID.ToString();

            llShowLicenseHistory.Enabled = (_SelectedLicenseID != -1);

            if (_SelectedLicenseID == -1)

            {
                return;
            }
            try
            {
                var IsDetainedLicense = await _detainedLicenseService.IsDetainedLicenseAsync(_SelectedLicenseID);

                if (IsDetainedLicense)
                {
                    MessageBox.Show("Selected License already detained, choose another one.", "Not allowed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnDetain.Enabled = false; 
                    return;
                }

                btnDetain.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء فحص حالة الرخصة: {ex.Message}", "خطأ في الشبكة", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnDetain.Enabled = false;
            }
        }

        private async void btnDetain_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtFineFees.Text, out decimal fineFees))
            {
                MessageBox.Show("الرجاء إدخال قيمة رسوم صحيحة", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnDetain.Enabled = false;

            try
            {
                var dtain = new DetainedLicense()
                {
                    CreatedByUserID = clsGlobal.GetUser.ID,
                    LicenseID = _SelectedLicenseID,
                    FineFees = fineFees,
                    DetainDate = DateTime.Now,
                    IsReleased = false,
                    ReleaseApplicationID = 0,
                    ReleaseDate = null,
                    ReleasedByUserID = 0
                };

                var result = await _detainedLicenseService.AddDetainedLicenseAsync(dtain);

                if (result != null)
                {
                    MessageBox.Show("تم حفظ البيانات بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnDetain.Enabled = false; 
                }
                else
                {
                    MessageBox.Show("فشلت عملية الحفظ في السيرفر", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnDetain.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء الاتصال: {ex.Message}", "خطأ فني", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnDetain.Enabled = true;
            }
        }
        private void gpDetain_Enter(object sender, EventArgs e)
        {

        }

        private void lblTitle_Click(object sender, EventArgs e)
        {

        }

        private void ctrlDriverLicenseInfoWithFilter1_Load(object sender, EventArgs e)
        {

        }

        private void llShowLicenseInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
        }

        private void llShowLicenseHistory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
           
        }
    }
}
