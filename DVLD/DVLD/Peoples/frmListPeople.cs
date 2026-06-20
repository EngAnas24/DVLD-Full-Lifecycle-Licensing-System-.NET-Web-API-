using DVLDServices.Extentions;
using DVLDServices.Services;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using DVLDServices.GlobalClasses;

namespace DVLD.Peoples
{
    public partial class frmListPeople : Form
    {
        private readonly PeopleService _peopleService;
        private static readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient
        {
            BaseAddress = new Uri("http://localhost:5067/")
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

            if (cbFilterBy.Items.Count > 0)
                cbFilterBy.SelectedIndex = 0;

            txtFilterValue.Visible = (cbFilterBy.Text != "None");
        }

        private void _UpdateRecordsCount()
        {
            lbllRowCount.Text = "Rows Count : " + dgvPeople.Rows.Count.ToString();
        }

        private async Task _RefreshPeopleList()
        {
            try
            {
                var allPeople = await _peopleService.GetPeopleAsync();

                _dtAllPeople = DatatableExtention.ToDataTable(allPeople);
                dgvPeople.DataSource = _dtAllPeople;
            }
            catch (Exception ex)
            {
                dgvPeople.DataSource = null;
            }
            finally
            {
                _UpdateRecordsCount();
            }
        }

        private void txtFilterValue_TextChanged(object sender, EventArgs e)
        {
            if (_dtAllPeople == null) return;

            string filterColumn = _GetFilterColumnName(cbFilterBy.Text);
            string filterValue = txtFilterValue.Text.Trim();

            if (string.IsNullOrWhiteSpace(filterValue) || filterColumn == "None")
            {
                _dtAllPeople.DefaultView.RowFilter = "";
            }
            else
            {
                try
                {
                    if (filterColumn == "PersonalID")
                    {
                        if (int.TryParse(filterValue, out int id))
                            _dtAllPeople.DefaultView.RowFilter = string.Format("[{0}] = {1}", filterColumn, id);
                        else
                            _dtAllPeople.DefaultView.RowFilter = "1=0"; 
                    }
                    else
                    {
                        _dtAllPeople.DefaultView.RowFilter = string.Format("[{0}] LIKE '{1}%'", filterColumn, filterValue);
                    }
                }
                catch
                {
                    _dtAllPeople.DefaultView.RowFilter = "";
                }
            }

            _UpdateRecordsCount();
        }

        private string _GetFilterColumnName(string propertyName)
        {
            switch (propertyName)
            {
                case "PersonalID": return "PersonalID";
                case "National No": return "NationalNo";
                case "First Name": return "FirstName";
                case "Second Name": return "SecondName";
                case "Third Name": return "ThirdName";
                case "Last Name": return "LastName";
                case "Nationality": return "CountryName";
                case "Gendor": return "GenderName"; 
                case "Phone": return "Phone";
                case "Email": return "Email";
                default: return "None";
            }
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
                    _UpdateRecordsCount();
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
            if (dgvPeople.CurrentRow == null) return;

            if (MessageBox.Show("This person will be deleted!! Are you sure you want to delete?",
                                "Confirm Delete",
                                MessageBoxButtons.OKCancel,
                                MessageBoxIcon.Warning) == DialogResult.OK)
            {
                await _peopleService.DeletePersonAsync(personID);
                MessageBox.Show("Person Deleted Successfully.", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await _RefreshPeopleList();
            }
        }

        private async void btnAddPerson_Click(object sender, EventArgs e)
        {
            frmAddUpdatePerson addUpdatePerson = new frmAddUpdatePerson();
            addUpdatePerson.ShowDialog();
            await _RefreshPeopleList();
        }

        private async void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmAddUpdatePerson addUpdatePerson = new frmAddUpdatePerson();
            addUpdatePerson.ShowDialog();
            await _RefreshPeopleList();
        }

        private async void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddUpdatePerson addUpdatePerson = new frmAddUpdatePerson(personID);
            addUpdatePerson.ShowDialog();
            await _RefreshPeopleList();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}