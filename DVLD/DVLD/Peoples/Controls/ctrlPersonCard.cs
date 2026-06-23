using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DVLDServices.Services;
using DVLD.Properties;
using System.IO;
using static DVLDServices.Services.PeopleService;

namespace DVLD.Peoples.Controls
{
        public partial class ctrlPersonCard : UserControl
        {
            private PeopleDto _Person;
            private readonly PeopleService _peopleService;
            private static readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient
            {
                BaseAddress = new Uri("http://localhost:5067/") // تأكد من وجود الـ / في النهاية
            };
        private int _PersonID = -1;

            public int PersonID
            {
                get { return _PersonID; }
            }

            public PeopleDto SelectedPersonInfo
            {
                get { return _Person; }
            }

            public ctrlPersonCard()
            {
                InitializeComponent();
            _peopleService = new PeopleService(_httpClient);
            }

            public async void LoadPersonInfo(int PersonID)
            {
                _Person = await _peopleService.GetPersonByIdAsync(PersonID);
                if (_Person == null)
                {
                    ResetPersonInfo();
                    MessageBox.Show("No Person with PersonID = " + PersonID.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _FillPersonInfo();
            }

            private void _LoadPersonImage()
            {
                if (_Person.Gender == "M")
                    pbPersonImage.Image = Resources.Male_512;
                else
                    pbPersonImage.Image = Resources.Female_512;

               
            }

            private void _FillPersonInfo()
            {
                llEditPersonInfo.Enabled = true;
                _PersonID = _Person.PersonalID;
                lblPersonID.Text = _Person.PersonalID.ToString();
                lblNationalNo.Text = _Person.NationalNo.ToString();
                lblFullName.Text = _Person.FullName;
                lblGendor.Text = _Person.Gender == "M" ? "Male" : "Female";
                lblEmail.Text = _Person.Email;
                lblPhone.Text = _Person.Phone;
                lblDateOfBirth.Text = _Person.DateOfBirth.ToShortDateString();
                lblCountry.Text = _Person.CountryName;
                lblAddress.Text = _Person.Address;
                DisplayPersonImage(_Person.ImagePath);




            }

            public void ResetPersonInfo()
            {
                _PersonID = -1;
                lblPersonID.Text = "[????]";
                lblNationalNo.Text = "[????]";
                lblFullName.Text = "[????]";
                pbGendor.Image = Resources.Man_32;
                lblGendor.Text = "[????]";
                lblEmail.Text = "[????]";
                lblPhone.Text = "[????]";
                lblDateOfBirth.Text = "[????]";
                lblCountry.Text = "[????]";
                lblAddress.Text = "[????]";
                pbPersonImage.Image = Resources.Male_512;

            }

            private void llEditPersonInfo_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
            {
                frmAddUpdatePerson frm = new frmAddUpdatePerson(_PersonID);
                frm.ShowDialog();

                //refresh
                LoadPersonInfo(_PersonID);
            }
        private void DisplayPersonImage(string imageName)
        {
            

            string baseApiUrl = "http://localhost:5067/uploads/people/";
            string defaultImage = baseApiUrl + "default.png";

            if (string.IsNullOrEmpty(imageName))
            {

                _LoadPersonImage();
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

        private void llEditPersonInfo_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmAddUpdatePerson frmAddUpdate = new frmAddUpdatePerson(_PersonID);
            frmAddUpdate.ShowDialog();

            //refresh
            LoadPersonInfo(_PersonID);
        }
    }


}
