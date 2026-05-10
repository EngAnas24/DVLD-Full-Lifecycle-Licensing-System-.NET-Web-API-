using DVLD.Properties;
using DVLDServices.Extentions;
using DVLDServices.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DVLDServices.Services.PeopleService;

namespace DVLD.Peoples
{
    public partial class frmAddUpdatePerson : Form
    {
        public enum enMode { AddNew = 0, Update = 1 };
        private readonly PeopleService _peopleService;
        private static readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient
        {
            BaseAddress = new Uri("http://localhost:5067/") // تأكد من وجود الـ / في النهاية
        };
        private enMode _Mode;
        private int _PersonID=-1;
        private People _person;
        private PeopleDto _Person ;
        private string _selectedImagePath;
        public delegate void DataBackEventHandler(object sender, int PersonID);
        private DataTable _dtAllContries;

        // Declare an event using the delegate
        public event DataBackEventHandler DataBack;
        public frmAddUpdatePerson(int PersonID)
        {
            InitializeComponent();
            _Mode = enMode.Update;
            _PersonID = PersonID;
            _person = new People();
            _peopleService = new PeopleService(_httpClient);
            GetAllCountries();


        }
        public frmAddUpdatePerson()
        {
            InitializeComponent();
            _Mode = enMode.AddNew;
            _person = new People();
            _peopleService = new PeopleService(_httpClient);
            GetAllCountries();

        }
        private void frmAddUpdatePerson_Load(object sender, EventArgs e)
        {
            ResetData();
            if (_Mode == enMode.Update)
                LoadData();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                //Here we dont continue becuase the form is not valid
                MessageBox.Show("Some fileds are not valide!, put the mouse over the red icon(s) to see the erro", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            int nationalityCountryID = (int)cbCountry.SelectedValue;

            _person.FirstName = txtFirstName.Text.Trim();
            _person.SecondName = txtSecondName.Text.Trim();
            _person.ThirdName = txtThirdName.Text.Trim();
            _person.LastName = txtLastName.Text.Trim();
            _person.NationalNo = int.Parse(txtNationalNo.Text.Trim());
            _person.Email = txtEmail.Text.Trim();
            _person.Phone = txtPhone.Text.Trim();
            _person.Address = txtAddress.Text.Trim();
            _person.DateOfBirth = dtpDateOfBirth.Value;

            if (rbMale.Checked)
                _person.Gender = "M";
            else
                _person.Gender = "F";

            _person.NationalityCountryID = nationalityCountryID;

            if (pbPersonImage.ImageLocation != null)
                _person.ImagePath = pbPersonImage.ImageLocation;
            else
                _person.ImagePath = "";


            if (_PersonID <= 0)
            {
                _person = await _peopleService.AddPersonAsync(_person);
                //change form mode to update.
                _Mode = enMode.Update;
                lblTitle.Text = "Update Person";

                MessageBox.Show("Data Saved Successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else if (_PersonID > 0)
            {
                _person.PersonalID = _PersonID;
                _person = await _peopleService.UpdatePerson(_PersonID, _person);
                lblPersonID.Text = _person.PersonalID.ToString();
                MessageBox.Show("Data Saved Successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //  DataBack.Invoke(this, _person.PersonalID);
            }
            else
                MessageBox.Show("Error: Data Is not Saved Successfully.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            if (!await _HandlePersonImage())
                return;
        }

    

        private async Task<bool> _HandlePersonImage()
        {
            if (!string.IsNullOrEmpty(_selectedImagePath) && File.Exists(_selectedImagePath))
            {
                // نستخدم result.PersonalID القادم من السيرفر
                bool isUploaded = await _peopleService.UploadPersonImage(_person.PersonalID, _selectedImagePath);

                if (!isUploaded)
                {
                    MessageBox.Show("تم إضافة الشخص ولكن فشل رفع الصورة.");
                }
            }
            return true;
        }

        private async void LoadData()
        {
            _Person = await _peopleService.GetPersonByIdAsync(_PersonID);

            if (_Person == null)
            {
                MessageBox.Show("No Person with ID = " + _PersonID, "Person Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
                return;
            }

            //the following code will not be executed if the person was not found
            lblPersonID.Text = _PersonID.ToString();
            txtFirstName.Text = _Person.FirstName;
            txtSecondName.Text = _Person.SecondName;
            txtThirdName.Text = _Person.ThirdName;
            txtLastName.Text = _Person.LastName;
            txtNationalNo.Text = _Person.NationalNo.ToString();
            dtpDateOfBirth.Value = _Person.DateOfBirth;
            txtPhone.Text = _Person.Phone;
            txtEmail.Text = _Person.Email;
            txtAddress.Text = _Person.Address;
            if (_Person.Gender == "M")
                rbMale.Checked = true;
            else
                rbFemale.Checked = true;

            cbCountry.SelectedIndex = cbCountry.FindString(_Person.CountryName);

            if (_Person.ImagePath != "")
                DisplayPersonImage (_Person.ImagePath);

            llRemoveImage.Visible = (_Person.ImagePath != "");

        }
        private void ResetData()
        {

            if (_Mode == enMode.AddNew)
            {
                lblTitle.Text = "Add New Person";
            }
            else
            {
                lblTitle.Text = "Update Person";
            }
            //set default image for the person.
            if (rbMale.Checked)
                pbPersonImage.Image = Resources.Male_512;
            else
                pbPersonImage.Image = Resources.Female_512;

            //hide/show the remove linke incase there is no image for the person.
            llRemoveImage.Visible = (pbPersonImage.ImageLocation != null);

            //we set the max date to 18 years from today, and set the default value the same.
            dtpDateOfBirth.MaxDate = DateTime.Now.AddYears(-18);
            dtpDateOfBirth.Value = dtpDateOfBirth.MaxDate;

            //should not allow adding age more than 100 years
            dtpDateOfBirth.MinDate = DateTime.Now.AddYears(-100);

            //this will set default country to jordan.
            cbCountry.SelectedIndex = cbCountry.FindString("Yemen");

            txtFirstName.Text = "";
            txtSecondName.Text = "";
            txtThirdName.Text = "";
            txtLastName.Text = "";
            txtNationalNo.Text = "";
            rbMale.Checked = true;
            txtPhone.Text = "";
            txtEmail.Text = "";
            txtAddress.Text = "";


        }

        private async void GetAllCountries()
        {
            var allCountries = await _peopleService.GetCountriesAsync();
            _dtAllContries = DatatableExtention.ToDataTable(allCountries);

            cbCountry.DataSource = _dtAllContries;
            cbCountry.DisplayMember = "CountryName"; 
            cbCountry.ValueMember = "CountryID";   
        }

        private void llSetImage_LinkClicked(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (openFileDialog.FileName == _selectedImagePath)
                    {
                        MessageBox.Show("لقد اخترت نفس الصورة فعلاً.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    _selectedImagePath = openFileDialog.FileName;

                    // استخدام ImageLocation يمنع قفل الملف ويسمح بتغييره لاحقاً
                    pbPersonImage.ImageLocation = _selectedImagePath;

                    // ملاحظة: هنا يجب استدعاء دالة الرفع للسيرفر (UploadToApi)
                }
            }
        }

        private void DisplayPersonImage(string imageName)
        {
            // تعريف المسار الافتراضي في البداية ليكون متاحاً للكل
            string defaultPath = Path.Combine("D:\\c#\\Asp.NetWebApi\\DVLD\\DVLD\\wwwroot\\uploads\\people\\", "default.png");

            if (string.IsNullOrEmpty(imageName))
            {
                ShowDefaultImage(defaultPath);
                return;
            }

            string baseUrl = "D:\\c#\\Asp.NetWebApi\\DVLD\\DVLD\\wwwroot\\uploads\\people\\";
            string fullUrl = baseUrl + imageName;

            try
            {
                // إضافة رقم عشوائي للرابط لمنع الكاش (Caching) في الـ WinForms
                // ليظهر التحديث فوراً إذا تغيرت الصورة بنفس الاسم
                pbPersonImage.LoadAsync(fullUrl);
            }
            catch
            {
                ShowDefaultImage(defaultPath);
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


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void llRemoveImage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // 1. التأكد من أن المستخدم يريد الحذف فعلاً
            if (MessageBox.Show("Are you sure you want to delete this image?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            try
            {
                // 2. استدعاء خدمة الحذف من الـ API
                bool isDeleted = await _peopleService.DeleteImagePersonAsync(_PersonID);

                if (isDeleted)
                {
                    // 3. تحديث واجهة المستخدم
                    // إرجاع الصورة الافتراضية (Default Image) بناءً على النوع
                    if (rbMale.Checked)
                        pbPersonImage.Image = Resources.Male_512; // استبدلها باسم الصورة لديك في الـ Resources
                    else
                        pbPersonImage.Image = Resources.Female_512;

                    // إخفاء رابط الحذف لأن الصورة حُذفت
                    llRemoveImage.Visible = false;

                    // تصفير الـ ImageLocation لضمان عدم حدوث تعارض عند الحفظ
                    pbPersonImage.ImageLocation = null;

                    MessageBox.Show("Image deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to delete the image from the server.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

    }
}
