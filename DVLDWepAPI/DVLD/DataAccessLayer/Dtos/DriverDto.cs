using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dtos
{
    public class DriverDto
    {
        public int DriverID { get; set; }
        public int PersonID { get; set; }
        public string NationalNo { get; set; } // أضفه
        public string FullName { get; set; }    // أضفه
        public DateTime CreatedDate { get; set; }
        public int NumberOfActiveLicenses { get; set; }
    }

    public class DriverLicenseDTO
    {
        public int LicenseID { get; set; }
        public int ApplicationID { get; set; }
        public string ClassName { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsActive { get; set; }
    }
}
