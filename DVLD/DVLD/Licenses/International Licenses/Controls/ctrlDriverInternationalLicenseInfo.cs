using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DVLDServices.Services.InternationalLicenseService;
using DVLDServices.Services;
using System.Net.Http;
using ClassLibrary1.GlobalClasses;
using DVLD.Properties;
using System.IO;

namespace DVLD.Licenses.International_Licenses.Controls
{
    public partial class ctrlDriverInternationalLicenseInfo : UserControl
    {

        private int _LicenseID;
        private InternationalLicenseDto _License;
        private InternationalLicenseService _licenseService;
        private LocalDrivingLicesnseApplicationsService _LocalAppService;
        private clsApplicationService _applicationService;
        private PeopleService _peopleService;
        private string _imagePath;
        HttpClient _httpClient = new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:5067/")
        };

        public ctrlDriverInternationalLicenseInfo()
        {
            InitializeComponent();
            _licenseService = new InternationalLicenseService(_httpClient);
            _LocalAppService = new LocalDrivingLicesnseApplicationsService(_httpClient);
            _applicationService = new clsApplicationService(_httpClient);
            _peopleService = new PeopleService(_httpClient);
        }


        public async Task LoadInfo(int LicenseID) // تغيير من void إلى Task
        {
            _LicenseID = LicenseID;

            if (_LicenseID <= 0)
                return;

            try
            {
                // تأكد أولاً أن الخدمة ليست null
                if (_licenseService == null)
                {
                    MessageBox.Show("Internal Error: License Service is not initialized.");
                    return;
                }

                _License = await _licenseService.GetInternationalLicenseByIdAsync(_LicenseID);

                if (_License == null)
                {
                    MessageBox.Show("Could not find International License ID = " + _LicenseID.ToString(),
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _LicenseID = -1;
                    // هنا قد تحتاج لتنظيف الواجهة (Reset Labels)
                    return;
                }

                var LocalAppService = await _LocalAppService.GetLocalApplicationByApplicationId(_License.ApplicationID);
                
                lblFullName.Text = LocalAppService.FullName;
                lblNationalNo.Text = LocalAppService.NationalNo;
                lblInternationalLicenseID.Text = _LicenseID.ToString();
                var LicenseInfo = await _licenseService.GetImagePathByApplicationID(_License.ApplicationID);
                lblGendor.Text = LicenseInfo.Gender == "M" ? "Male" : "Female";

                var app = await _applicationService.GetApplicationByIdAsync(LocalAppService.ApplicationID);
                var person = await _peopleService.GetPersonByIdAsync(app.ApplicantPersonID);
                lblDateOfBirth.Text = clsFormat.DateToShort(person.DateOfBirth);

                lblDriverID.Text = _License.DriverID.ToString();
                lblIssueDate.Text = clsFormat.DateToShort(_License.IssueDate);
                lblLocalLicenseID.Text = _License.IssuedUsingLocalLicenseID.ToString();
                lblApplicationID.Text = _License.ApplicationID.ToString();
                lblExpirationDate.Text = clsFormat.DateToShort(_License.ExpirationDate);
                lblIsActive.Text = _License.IsActive.ToString();
                _LoadPersonImage();

            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }


        private async void _LoadPersonImage()
        {
            var imagePath = await _licenseService.GetImagePathByApplicationID(_License.ApplicationID);
            if (imagePath.Gender == "M")
                pbPersonImage.Image = Resources.Male_512;
            else
                pbPersonImage.Image = Resources.Female_512;

            _imagePath = imagePath.ImagePath;

            if (_imagePath != "")
                DisplayPersonImage(_imagePath);
            else
                MessageBox.Show("Could not find this image: = " + _imagePath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        private void DisplayPersonImage(string imageName)
        {

            string baseApiUrl = "http://localhost:5067/uploads/people/";
            string defaultImage = baseApiUrl + "default.png";

            if (string.IsNullOrEmpty(imageName))
            {
                pbPersonImage.LoadAsync(defaultImage);
                return;
            }

            string fullUrl = baseApiUrl + imageName;

            try
            {
                pbPersonImage.LoadAsync(fullUrl);
            }
            catch
            {
                pbPersonImage.LoadAsync(defaultImage);
            }
        }
        private void ShowDefaultImage(string path)
        {
            if (File.Exists(path))
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    pbPersonImage.Image = Image.FromStream(stream);
                }
            }
        }

    }
}
