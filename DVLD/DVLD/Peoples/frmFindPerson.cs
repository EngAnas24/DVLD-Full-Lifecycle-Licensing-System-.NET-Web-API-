using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLD.Peoples
{
    public partial class frmFindPerson : Form
    {
        private int PersoID;
        public frmFindPerson()
        {
            InitializeComponent();
        }

        private void frmFindPerson_Load(object sender, EventArgs e)
        {
            ctrlPersonCardWithFilter1.LoadPersonInfo(PersoID);
        }

        private void ctrlPersonCardWithFilter1_OnPersonSelected(int obj)
        {
            PersoID = obj;
        }
    }
}
