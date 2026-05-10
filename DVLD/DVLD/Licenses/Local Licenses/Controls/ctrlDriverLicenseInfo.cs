using DVLDServices.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using static DVLDServices.Services.LicenseService;
using DVLD.Properties;
using System.Drawing;
using static DVLDServices.Services.TestAppointmentService;
using ClassLibrary1.GlobalClasses;

namespace DVLD.Licenses.Local_Licenses.Controls
{
    public partial class ctrlDriverLicenseInfo : UserControl
    {

        private int _LicenseID;
        private LicenseDto _License;
        private LicenseService _licenseService;
        private LocalDrivingLicesnseApplicationsService _LocalAppService;
        private clsApplicationService _applicationService;
        private PeopleService _peopleService;
        private string _imagePath;
        HttpClient _httpClient = new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:5067/")
        };

        public int LicenseID
        {
            get { return _LicenseID; }
        }
        public LicenseDto SelectedLicenseInfo
        { get { return _License; } }

        public ctrlDriverLicenseInfo()
        {
            InitializeComponent();
            _licenseService = new LicenseService(_httpClient);
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

                _License = await _licenseService.GetLicenseByIdAsync(_LicenseID);

                if (_License == null)
                {
                    MessageBox.Show("Could not find License ID = " + _LicenseID.ToString(),
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    _LicenseID = -1;
                    // هنا قد تحتاج لتنظيف الواجهة (Reset Labels)
                    return;
                }

                lblLicenseID.Text = _License.LicenseID.ToString();
                lblIsActive.Text = Convert.ToBoolean(_License.IsActive) ? "Yes" : "No";
                lblIsDetained.Text = "";
                var LocalAppService = await _LocalAppService.GetLocalApplicationByApplicationId(_License.ApplicationID);
                lblClass.Text = LocalAppService.ClassName;
                lblFullName.Text = LocalAppService.FullName;
                lblNationalNo.Text = LocalAppService.NationalNo;
                var LicenseInfo = await _licenseService.GetImagePathByApplicationID(_License.ApplicationID);

                lblGendor.Text = LicenseInfo.Gender == "M" ? "Male" : "Female";
                var app = await _applicationService.GetApplicationByIdAsync(LocalAppService.ApplicationID);
                var person = await _peopleService.GetPersonByIdAsync(app.ApplicantPersonID);
                lblDateOfBirth.Text = clsFormat.DateToShort(person.DateOfBirth);

                lblDriverID.Text = _License.DriverID.ToString();
                lblIssueDate.Text = clsFormat.DateToShort(_License.IssueDate);
                lblExpirationDate.Text = clsFormat.DateToShort(_License.ExpirationDate);
                lblIssueReason.Text = _License.IssueReason.ToString();
                lblNotes.Text = _License.Notes == "" ? "No Notes" : _License.Notes;
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
