using DVLDServices.GlobalClasses;
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
using static DVLDServices.Services.clsApplicationService;
using static DVLDServices.Services.ApplicationTypeService;
using static DVLDServices.Services.LocalDrivingLicesnseApplicationsService;

namespace DVLD.Applications.Local_Driving_License
{
    public partial class frmAddUpdateLocalDrivingLicesnseApplication : Form
    {
        private int _PersonID;
        private int peronId;
        private readonly LocalDrivingLicesnseApplicationsService _LocalapplicationService;
        private readonly LicenseClassService _licenseClassService;
        private readonly clsApplicationService applicationService;
        private readonly ApplicationTypeService _applicationTypeService;
        private  ApplicationTypeDto _applicationType;
        private LocalDrivingLicenseApplicationsDto LicenseApplicationsDto; 

        private DataTable dataTable;
        private int _LocalLicesnseApplicationID;
        private static readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient
        {
            BaseAddress = new Uri("http://localhost:5067/") // تأكد من وجود الـ / في النهاية
        };
        private enum eMod {Add,Update };
        private eMod _Mod;
        public frmAddUpdateLocalDrivingLicesnseApplication()
        {
            InitializeComponent();
            _LocalapplicationService = new LocalDrivingLicesnseApplicationsService(_httpClient);
            _Mod = eMod.Add;
            _licenseClassService = new LicenseClassService(_httpClient);
            applicationService = new clsApplicationService(_httpClient);
            _applicationTypeService = new ApplicationTypeService(_httpClient);
        }
        public frmAddUpdateLocalDrivingLicesnseApplication(int LocalLicesnseApplicationID)
        {
         
            InitializeComponent();
            _LocalapplicationService = new LocalDrivingLicesnseApplicationsService(_httpClient);
            _Mod = eMod.Update;
            _LocalLicesnseApplicationID = LocalLicesnseApplicationID;
            _licenseClassService = new LicenseClassService(_httpClient);
            applicationService = new clsApplicationService(_httpClient);
            _applicationTypeService = new ApplicationTypeService(_httpClient);

        }


        private async Task LicenseClassCombox()
        {
            var LicenseClasses = await _licenseClassService.GetLicenseClassesAsync();

            dataTable = DatatableExtention.ToDataTable(LicenseClasses);

            cbLicenseClass.DataSource = dataTable;
            cbLicenseClass.DisplayMember = "ClassName";
            cbLicenseClass.ValueMember = "LicenseClassID";

            if(_Mod == eMod.Add)
            {
            if (cbLicenseClass.Items.Count > 0)
                cbLicenseClass.SelectedIndex = 0;
            }
            if (_Mod == eMod.Update)
            {
                if (cbLicenseClass.Items.Count > 0)
                    cbLicenseClass.SelectedIndex =
                        await _licenseClassService.GetLicenseClassIDByLocalAppID(_LocalLicesnseApplicationID) - 1;
            }

        }
        private async Task _RestData()
        {
            if (_Mod == eMod.Add)
            {
                lblTitle.Text = "New Local Driving License Application";
                this.Text = "New Local Driving License Application";
                ctrlPersonCardWithFilter1.FilterFocus();
                lblApplicationDate.Text = DateTime.Now.ToShortDateString();

                if (cbLicenseClass.SelectedValue == null)
                {
                    MessageBox.Show("SelectedValue is NULL");
                    return;
                }

                int LicenseClassID = (int)cbLicenseClass.SelectedValue;

                if (_licenseClassService == null)
                {
                    MessageBox.Show("Service is NULL");
                    return;
                }
                int AppTypeID = (int)ApplicationTypeService.EnApplicationType.NewLocalDrivingLicenseService;
                 _applicationType = await _applicationTypeService.GetApplicationTypeByIdAsync(AppTypeID);

                if (_applicationType == null)
                {
                    MessageBox.Show("Application Type is NULL");
                    return;
                }

                lblFees.Text = _applicationType.ApplicationFees.ToString();

                if (clsGlobal.GetUser == null)
                {
                    MessageBox.Show("User is NULL");
                    return;
                }

                lblCreatedByUser.Text = clsGlobal.GetUser.ID.ToString();

                tpApplicationInfo.Enabled = false;
                btnSave.Enabled = false;
            }
            else
            {
                lblTitle.Text = "Update Local Driving License Application";
                this.Text = "Update Local Driving License Application";

                tpApplicationInfo.Enabled = true;
                btnSave.Enabled = true;
            }
        }

        public async void frmAddUpdateLocalDrivingLicesnseApplication_Load(object sender, EventArgs e)
        {
            ctrlPersonCardWithFilter1.OnPersonSelected += ctrlPersonCardWithFilter1_OnPersonSelected;
            await LicenseClassCombox();  
            await _RestData();

            if(_Mod == eMod.Update)
            {
               await _LoadData();
            }
            
        }

        private async Task _LoadData()
        {
            ctrlPersonCardWithFilter1.FilterEnabled = false;
            var localLicense =await _LocalapplicationService.GetLocalApplicationByIdAsync(_LocalLicesnseApplicationID);
            var App = await applicationService.GetApplicationByIdAsync(localLicense.ApplicationID);
            if (localLicense == null)
            {
                MessageBox.Show("No Application with ID = " + _LocalLicesnseApplicationID, "Application Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();

                return;
            }
            lblLocalDrivingLicebseApplicationID.Text = _LocalLicesnseApplicationID.ToString() ;
            lblApplicationDate.Text = DateTime.Now.ToString();
            cbLicenseClass.DisplayMember = localLicense.ClassName;

            int AppTypeID = (int)ApplicationTypeService.EnApplicationType.NewLocalDrivingLicenseService;
            _applicationType = await _applicationTypeService.GetApplicationTypeByIdAsync(AppTypeID);
            lblFees.Text = _applicationType.ApplicationFees.ToString();

            lblCreatedByUser.Text = clsGlobal.GetUser.ID.ToString();
            ctrlPersonCardWithFilter1.LoadPersonInfo(App.ApplicantPersonID);
        }

        private void ctrlPersonCardWithFilter1_OnPersonSelected(int obj)
        {
            if (obj == -1)
                return;

            _PersonID = obj;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            
            if (_Mod == eMod.Add)
            {
                var Application = new clsApplication()
                {
                    ApplicantPersonID = ctrlPersonCardWithFilter1.PersonID,
                    ApplicationDate = DateTime.Now,
                    ApplicationTypeID = _applicationType.ApplicationTypeID,
                    ApplicationStatus = (int)clsApplicationService.EnApplicationStatus.New,
                    CreatedByUserID = clsGlobal.GetUser.ID,
                    LastStatusDate = DateTime.Now,
                    PaidFees = _applicationType.ApplicationFees
                };

                int ActiveApplicationID = await _LocalapplicationService.HasApplication(ctrlPersonCardWithFilter1.PersonID, (int)cbLicenseClass.SelectedValue);
                if (ActiveApplicationID ==1)
                {
                    MessageBox.Show("Choose another License Class, the selected Person Already have an active application for the selected class with id=" + ActiveApplicationID, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    cbLicenseClass.Focus();
                    return;
                }

                if (Application != null)
                {
                  var app =  await applicationService.AddApplicationAsync(Application);
                    var localApplication = new LocalDrivingLicenseApplications()
                    {
                        ApplicationID = app.ApplicationID,
                        LicenseClassID =(int) cbLicenseClass.SelectedValue
                    };
                    await _LocalapplicationService.InsertLocalApplication(localApplication);
             
                MessageBox.Show("تم الحفظ بنجاح ", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    MessageBox.Show("فشل في  الحفظ  ", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if(_Mod == eMod.Update)
            {
                var localLicense = await _LocalapplicationService.GetLocalApplicationByIdAsync(_LocalLicesnseApplicationID);

                var Application = new clsApplication()
                {
                    ApplicationID = localLicense.ApplicationID,
                    ApplicantPersonID = ctrlPersonCardWithFilter1.PersonID,
                    ApplicationDate = DateTime.Now,
                    ApplicationTypeID = _applicationType.ApplicationTypeID,
                    ApplicationStatus = (int)clsApplicationService.EnApplicationStatus.New,
                    CreatedByUserID = clsGlobal.GetUser.ID,
                    LastStatusDate = DateTime.Now,
                    PaidFees = _applicationType.ApplicationFees
                };

                if (Application != null)
                {
                    var localApplication = new LocalDrivingLicenseApplications()
                    {
                        LocalDrivingLicenseApplicationID = _LocalLicesnseApplicationID,
                        ApplicationID = Application.ApplicationID,
                        LicenseClassID = (int)cbLicenseClass.SelectedValue
                    };
                    await _LocalapplicationService.UpdateLocalApplication(_LocalLicesnseApplicationID, localApplication);
                    MessageBox.Show("تم التعديل بنجاح ", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    MessageBox.Show("فشل في  التعديل  ", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



        }

        private void btnApplicationInfoNext_Click(object sender, EventArgs e)
        {

            if (_Mod == eMod.Update)
            {
                btnSave.Enabled = true;
                tpApplicationInfo.Enabled = true;
                tcApplicationInfo.SelectedTab = tcApplicationInfo.TabPages["tpApplicationInfo"];
                return;
            }


            //incase of add new mode.
            if (ctrlPersonCardWithFilter1.PersonID != -1)
            {

                btnSave.Enabled = true;
                tpApplicationInfo.Enabled = true;
                tcApplicationInfo.SelectedTab = tcApplicationInfo.TabPages["tpApplicationInfo"];

            }

            else

            {
                MessageBox.Show("Please Select a Person", "Select a Person", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ctrlPersonCardWithFilter1.FilterFocus();
            }
        }
    }
}
