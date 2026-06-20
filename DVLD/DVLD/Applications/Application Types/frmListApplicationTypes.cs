using DVLDServices.Extentions;
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

namespace DVLD.Application_Types
{
    public partial class frmListApplicationTypes : Form
    {
        private readonly ApplicationTypeService _applicationTypeService;
        private DataTable dataTable;
        private static readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient
        {
            BaseAddress = new Uri("http://localhost:5067/") 
        };
        public frmListApplicationTypes()
        {
            InitializeComponent();
            _applicationTypeService = new ApplicationTypeService(_httpClient);
        }


        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void frmListApplicationTypes_Load(object sender, EventArgs e)
        {
            var applicationTypes = await _applicationTypeService.GetApplicationTypesAsync();
            dataTable = DatatableExtention.ToDataTable(applicationTypes);
            dgvApplicationTypes.DataSource = dataTable;
            lblRecordsCount.Text = dataTable.Rows.Count.ToString();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int ID = (int)dgvApplicationTypes.CurrentRow.Cells["ApplicationTypeID"].Value;
            frmEditApplicationType frmEditApplicationType = new frmEditApplicationType(ID);
            frmEditApplicationType.ShowDialog();
            frmListApplicationTypes_Load(null, null);
        }
    }
}
