using DVLDServices.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static DVLDServices.Services.TestTypeService;

namespace DVLD.Tests
{
    public partial class frmEditTestType : Form
    {
        private int _TestTypeID;
        private TestTypeService _testTypeService;
        private HttpClient _httpClient = new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:5067/")
        }; 
        public frmEditTestType(int TestTypeID)
        {
            InitializeComponent();
            _TestTypeID = TestTypeID;
            _testTypeService = new TestTypeService(_httpClient);
        }

        public async void LoadData()
        {
            var TestType = await _testTypeService.GetTestTypeByIdAsync(_TestTypeID);
            if(TestType == null)
            {
                MessageBox.Show("مافي بيانات"," ",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
            txtTitle.Text = TestType.TestTypeTitle;
            txtDescription.Text = TestType.TestTypeDescription;
            txtFees.Text = TestType.TestTypeFees.ToString();
            lblTestTypeID.Text = TestType.TestTypeID.ToString();
            lblTitle.Text = TestType.TestTypeTitle;

        }

        private void frmEditTestType_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (!this.ValidateChildren())
            {
                MessageBox.Show("تحقق من أنك قمت بتعبئة كل الحقول الفارغة");
                return;
            }


            TestType testType = new TestType()
            {
                TestTypeTitle = txtTitle.Text,
                TestTypeID = _TestTypeID,
                TestTypeDescription = txtDescription.Text,
                TestTypeFees = decimal.Parse(txtFees.Text)
            };
            if(testType != null)
            {
                await _testTypeService.UpdateTestType(_TestTypeID, testType);
                MessageBox.Show("تم التعديل بنجاح", "",MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            MessageBox.Show("فشل في الحفظ", "", MessageBoxButtons.OK, MessageBoxIcon.Error);

            LoadData();
        }

        private void txtTitle_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtTitle.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtTitle, "قم بكتابة العنوان الخاص بهذا الإختبار");
            }
            else
            {
                errorProvider1.SetError(txtTitle,null);
            }
        }

        private void txtDescription_Validating(object sender, CancelEventArgs e)
        {

            if (string.IsNullOrEmpty(txtDescription.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtDescription,"قم بكتابة الوصف الخاص بهذا الإختبار");
            }
            else
            {
                errorProvider1.SetError(txtDescription,null);
            }
        }

        private void txtFees_Validating(object sender, CancelEventArgs e)
        {

            if (string.IsNullOrEmpty(txtFees.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtFees, "قم بكتابة التكلفة  الخاصة بهذا الإختبار");
            }
            else
            {
                errorProvider1.SetError(txtFees, null);
            }
        }


    }
}
