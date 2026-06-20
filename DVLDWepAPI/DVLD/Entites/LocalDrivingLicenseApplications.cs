using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Entites
{
    public class LocalDrivingLicenseApplications
    {
        public int LocalDrivingLicenseApplicationID { get; set; }
        public int ApplicationID { get; set; }
        public int LicenseClassID { get; set; }

    }

    public class CreateLocalDrivingLicenseAppDto
    {
        public int LocalApplicantID { get; set; }
        public int NewApplicationID { get; set; }
        public int NewLocalDrivingLicenseAppID { get; set; }
        public int ApplicantPersonID { get; set; }
        public int ApplicationTypeID { get; set; }
        public decimal PaidFees { get; set; }
        public int CreatedByUserID { get; set; }
        public int LicenseClassID { get; set; }     }



    public class CreateLocalDrivingLicenseRequest
    {
                        [Key]
        public int FakeId { get; set; }

                public int ApplicantPersonID { get; set; }
        public DateTime ApplicationDate { get; set; }
        public byte ApplicationStatus { get; set; }
        public int ApplicationTypeID { get; set; }
        public DateTime LastStatusDate { get; set; }
        public decimal PaidFees { get; set; }
        public int CreatedByUserID { get; set; }
        public int LicenseClassID { get; set; }
    }



    public class UpdateLocalDrivingLicenseAppDto
    {
        public int ApplicationID { get; set; }
        public byte ApplicationStatus { get; set; }
        public int LicenseClassID { get; set; }

    }
    public class UpdateLocalDrivingLicenseAppRequest
    {

        [Key]
        public int FakeId { get; set; }
        public int ApplicationID { get; set; }
        public int LocalApplicationID { get; set; }
        public byte ApplicationStatus { get; set; }
        public DateTime LastStatusDate { get; set; }
        public int LicenseClassID { get; set; }

    }
}
