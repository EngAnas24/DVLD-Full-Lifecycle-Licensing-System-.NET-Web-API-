using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entites
{
    public class Test
    {
        public int TestID { get; set; }
        public int TestAppointmentID { get; set; }
        public byte TestResult { get; set; }
        public string Notes { get; set; }
        public int CreatedByUserID { get; set; }

    }
}
