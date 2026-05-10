using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DVLD.Application_Types;
using DVLD.Applications.International_License;
using DVLD.Applications.Local_Driving_License;
using DVLD.Applications.Renew_Local_License;
using DVLD.Applications.ReplaceLostOrDamagedLicense;
using DVLD.Drivers;
using DVLD.Login;
using DVLD.Peoples;
using DVLD.Tests;
using DVLD.Users;
using DVLDServices.Services;

namespace DVLD
{
    public partial class FrmMain : Form
    {
        private UserService _userService;
        private int _UserID;
        private static readonly System.Net.Http.HttpClient _httpClient = new System.Net.Http.HttpClient
        {
            BaseAddress = new Uri("http://localhost:5067/") // تأكد من وجود الـ / في النهاية
        };
        public FrmMain(int UserID)
        {
            InitializeComponent();
            _userService = new UserService(_httpClient);
            _UserID = UserID;
        }

        private void peopleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmListPeople frmPeople = new frmListPeople();
            frmPeople.Show();
            this.Hide();
        }

        private void employeesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmListUsers frmUser = new frmListUsers();
            frmUser.Show();
        }

        private void msMainMenue_MouseHover(object sender, EventArgs e)
        {
            lblClose.BackColor = Color.Red;
            lblClose.ForeColor = Color.White;
        }

        private void lblClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void currentUserInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmUserInfo userInfo = new frmUserInfo(_UserID);
            userInfo.ShowDialog();


        }

        private void changePasswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmChangePassword changePassword = new frmChangePassword(_UserID);
            changePassword.ShowDialog();
        }

        private void signOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmLogin frmLogin = new FrmLogin();
            this.Hide();
            frmLogin.Show();
        }

        private void manageApplicationTypesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmListApplicationTypes frmList = new frmListApplicationTypes();
            frmList.ShowDialog();
        }

        private void localLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddUpdateLocalDrivingLicesnseApplication frmAddUpdateLocalDriving =
                 new frmAddUpdateLocalDrivingLicesnseApplication();
            frmAddUpdateLocalDriving.ShowDialog();
        }

        private void manageTestTypesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmListTestTypes frmListTestTypes = new frmListTestTypes();
            frmListTestTypes.ShowDialog();
            
        }

        private void driversToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmListDrivers frmListDrivers = new frmListDrivers();
            frmListDrivers.ShowDialog();

        }

        private void manageLocalDrivingLicenseApplicationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmListLocalDrivingLicesnseApplications frmListLocalDriving = new frmListLocalDrivingLicesnseApplications();
            frmListLocalDriving.Show();
        }

        private void internationalLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmNewInternationalLicenseApplication frmNewInternational = new frmNewInternationalLicenseApplication();
            frmNewInternational.ShowDialog();
        }

        private void ManageInternationaDrivingLicenseToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmListInternationalLicesnseApplications frmListInternational = new frmListInternationalLicesnseApplications();
            frmListInternational.ShowDialog();
        }

        private void renewDrivingLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmRenewLocalDrivingLicenseApplication frmRenewLocalDriving = new frmRenewLocalDrivingLicenseApplication();
            frmRenewLocalDriving.ShowDialog();
        }

        private void ReplacementLostOrDamagedDrivingLicenseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmReplaceLostOrDamagedLicenseApplication frm =
                new frmReplaceLostOrDamagedLicenseApplication();

            frm.ShowDialog();
        }
    }
}
