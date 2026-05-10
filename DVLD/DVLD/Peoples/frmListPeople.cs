using DVLDServices.Extentions;
using DVLDServices.Services;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq; // أضفنا هذا لاستخدام عمليات التحقق المتقدمة
using DVLDServices.GlobalClasses;

namespace DVLD.Peoples
{
    public partial class frmListPeople : Form
    {
        private readonly PeopleService _peopleService;
        private static readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient
        {
            BaseAddress = new Uri("http://localhost:5067/") // تأكد من وجود الـ / في النهاية
        };

        private DataTable _dtAllPeople;
        private int personID;

        public frmListPeople()
        {
            InitializeComponent();
            _peopleService = new PeopleService(_httpClient);
        }

        private async void frmListPeople_Load(object sender, EventArgs e)
        {
            await _RefreshPeopleList();

            // تأكد أن الكومبوبوكس يحتوي على العناصر قبل اختيار المندكس
            if (cbFilterBy.Items.Count > 0)
                cbFilterBy.SelectedIndex = 0;

            if(cbFilterBy.Text =="None")
            txtFilterValue.Visible = false;
        }

        private async Task _RefreshPeopleList()
        {
            try
            {
                var allPeople = await _peopleService.GetPeopleAsync();
                if (!allPeople.Any())
                {
                    lbllRowCount.Text = "0";
                }
                // تحويل القائمة إلى DataTable باستخدام الـ Extension الخاص بك
                _dtAllPeople = DatatableExtention.ToDataTable(allPeople);

                dgvPeople.DataSource = _dtAllPeople;
                lbllRowCount.Text = lbllRowCount.Text + dgvPeople.Rows.Count.ToString();
            }
            catch (Exception ex)
            {
                lbllRowCount.Text = lbllRowCount.Text + " 0";
            }
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {
            if (_dtAllPeople == null) return;

            string filterColumn = "";

            switch (cbFilterBy.Text)
            {
                case "PersonalID":
                    filterColumn = "PersonalID";
                    break;
                case "National No":
                    filterColumn = "National No";
                    break;
                case "First Name":
                    filterColumn = "FirstName";
                    break;
                case "Second Name":
                    filterColumn = "SecondName";
                    break;
                case "Third Name":
                    filterColumn = "ThirdName";
                    break;
                case "Last Name":
                    filterColumn = "LastName";
                    break;
                case "Nationality":
                    filterColumn = "CountryName";
                    break;
                case "Gendor":
                    filterColumn = "GenderName"; 
                    break;
                case "Phone":
                    filterColumn = "Phone";
                    break;
                case "Email":
                    filterColumn = "Email";
                    break;
                default:
                    filterColumn = "None";
                    break;
            }

            // تنظيف المسافات
            string filterValue = txtFilterValue.Text.Trim();

            if (string.IsNullOrWhiteSpace(filterValue) || filterColumn == "None")
            {
                _dtAllPeople.DefaultView.RowFilter = "";
                lblRecordsCount.Text = _dtAllPeople.Rows.Count.ToString();
                return;
            }

            try
            {
                if (filterColumn == "PersonalID" )
                {
                    if (int.TryParse(filterValue, out int id))
                    {
                        _dtAllPeople.DefaultView.RowFilter = string.Format("[{0}] = {1}", filterColumn, id);
                    }
                    else
                    {
                        // إذا كتب المستخدم نصاً في حقل الرقم، نعرض قائمة فارغة
                        _dtAllPeople.DefaultView.RowFilter = "1=0";
                    }
                }
                else
                {
                    // بقية الأعمدة نصية
                    _dtAllPeople.DefaultView.RowFilter = string.Format("[{0}] LIKE '{1}%'", filterColumn, filterValue);
                }
            }
            catch (Exception ex)
            {
                // لتجنب انهيار البرنامج في حال وجود رموز خاصة
                _dtAllPeople.DefaultView.RowFilter = "";
            }

            lblRecordsCount.Text = _dtAllPeople.DefaultView.Count.ToString();
        }
        private void cbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtFilterValue.Visible = (cbFilterBy.Text != "None");

            if (txtFilterValue.Visible)
            {
                txtFilterValue.Text = "";
                txtFilterValue.Focus();
            }
            else
            {
                if (_dtAllPeople != null)
                {
                    _dtAllPeople.DefaultView.RowFilter = "";
                    lblRecordsCount.Text = _dtAllPeople.Rows.Count.ToString();
                }
            }

        }

        private void dgvPeople_DoubleClick(object sender, EventArgs e)
        {
            if (dgvPeople.CurrentRow == null) return;

             personID = (int)dgvPeople.CurrentRow.Cells["PersonalID"].Value;
            frmShowPersonInfo frmShowPerson = new frmShowPersonInfo(personID);
            frmShowPerson.ShowDialog();
        }

        private void dgvPeople_SelectionChanged(object sender, EventArgs e)
        {

            if (dgvPeople.CurrentRow != null && dgvPeople.CurrentRow.Index >= 0)
            {
                try
                {
                    personID = Convert.ToInt32(dgvPeople.CurrentRow.Cells["PersonalID"].Value);
                }

                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvPeople.CurrentRow == null) return;

            personID = (int)dgvPeople.CurrentRow.Cells["PersonalID"].Value;
            frmShowPersonInfo frmShowPerson = new frmShowPersonInfo(personID);
            frmShowPerson.ShowDialog();
        }

        private async void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This person will be deleted!! Are you sure you want to delete?",
                                "Confirm Delete",
                                MessageBoxButtons.OKCancel,
                                MessageBoxIcon.Warning) == DialogResult.OK)
            {
                 await _peopleService.DeletePersonAsync(personID);
                 MessageBox.Show("Person Deleted Successfully.", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                 await _RefreshPeopleList();
            }

            else
                MessageBox.Show("Person was not deleted because it has data linked to it.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }

        private void btnAddPerson_Click(object sender, EventArgs e)
        {
            frmAddUpdatePerson addUpdatePerson = new frmAddUpdatePerson();
            addUpdatePerson.Show();
            this.Hide();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            
            FrmMain frmMain = new FrmMain(clsGlobal.GetUser.ID);
            this.Hide();
            frmMain.Show();
        }
    }
}