using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Dtos
{
    public class LicenseDto
    {
        public int LicenseID { get; set; }
        public int ApplicationID { get; set; }
        public int DriverID { get; set; }
        public int LicenseClass { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Notes { get; set; }
        public float PaidFees { get; set; }
        public byte IsActive { get; set; }
        public int IssueReason { get; set; }
        public int CreatedByUserID { get; set; }
    }
    public class GetImagePath
    {
        public string ImagePath { get; set; }
        public string Gender { get; set; }

    }
    public class ActiveLicense
    {
        public int ID { get; set; }
        public int LicenseClassId { get; set; }
    }


}
