using DVLDServices.Extentions;
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

namespace DVLD.Tests
{
    public partial class frmListTestTypes : Form
    {
        private TestTypeService _testTypeService;
        private DataTable dataTable;
        HttpClient client = new HttpClient() { BaseAddress = new Uri("http://localhost:5067/") };
        public frmListTestTypes()
        {
            InitializeComponent();
            _testTypeService = new TestTypeService(client);

        }

        private void frmListTestTypes_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        public async void LoadData()
        {
            var TestTypes = await _testTypeService.GetTestTypesAsync();
            dataTable = DatatableExtention.ToDataTable(TestTypes);
            dgvTestTypes.DataSource = dataTable;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int TestTypeID =(int) dgvTestTypes.CurrentRow.Cells["TestTypeID"].Value;
            frmEditTestType frmEditTestType = new frmEditTestType(TestTypeID);
            frmEditTestType.ShowDialog();
            frmListTestTypes_Load(null, null);

        }
    }
}
