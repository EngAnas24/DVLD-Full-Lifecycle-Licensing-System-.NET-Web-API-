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
using static DVLDServices.Services.ApplicationTypeService;

namespace DVLD.Application_Types
{
    public partial class frmEditApplicationType : Form
    {
        private int _ApplicationTypeID;
        private readonly ApplicationTypeService _applicationTypeService;
        private static readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient
        {
            BaseAddress = new Uri("http://localhost:5067/") // تأكد من وجود الـ / في النهاية
        };
        public frmEditApplicationType(int ApplicationTypeID)
        {
            InitializeComponent();
            _ApplicationTypeID = ApplicationTypeID;
            _applicationTypeService = new ApplicationTypeService(_httpClient);
        }


       public async void LoadData()
        {
            var ApplicationType = await _applicationTypeService.GetApplicationTypeByIdAsync(_ApplicationTypeID);
            txtTitle.Text = ApplicationType.ApplicationTypeTitle;
            txtFees.Text = ApplicationType.ApplicationFees.ToString();
        }

        private void frmEditApplicationType_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                ApplicationType type = new ApplicationType()
                {

                    ApplicationTypeID = _ApplicationTypeID,
                    ApplicationFees = decimal.Parse(txtFees.Text),
                    ApplicationTypeTitle = txtTitle.Text
                };

                await _applicationTypeService.UpdateApplicationType(_ApplicationTypeID, type);

                MessageBox.Show("تم التحديث بنجاح");
            }

            catch(Exception ex)
            {
                MessageBox.Show("فشل في النحديث");
            }
        }
    }
}
