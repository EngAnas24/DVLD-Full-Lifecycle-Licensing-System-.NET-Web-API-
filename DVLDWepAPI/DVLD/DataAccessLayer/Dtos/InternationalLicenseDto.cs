using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dtos
{
    public class InternationalLicenseDto
    {
        public int InternationalLicenseID { get; set; }
        public int ApplicationID { get; set; }
        public int DriverID { get; set; }
        public int IssuedUsingLocalLicenseID { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsActive { get; set; }
        public int CreatedByUserID { get; set; }
    }

    public class GetInternationalImagePath
    {
        public string ImagePath { get; set; }
        public string Gender { get; set; }

    }
    public class ActiventernationalLicense
    {
        public int ID { get; set; }
        public int LicenseClassId { get; set; }
    }
}
