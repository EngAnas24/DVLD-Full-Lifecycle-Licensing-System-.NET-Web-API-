using DVLD.Peoples.Controls;
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
    public partial class frmShowPersonInfo : Form
    {
        private int _PersonID;

        public frmShowPersonInfo(int PersonID)
        {
            InitializeComponent();
            _PersonID = PersonID;

            // استدعاء الدالة مباشرة بعد تهيئة المكونات
            ctrlPersonCard1.LoadPersonInfo(_PersonID);
        }
    }
}
