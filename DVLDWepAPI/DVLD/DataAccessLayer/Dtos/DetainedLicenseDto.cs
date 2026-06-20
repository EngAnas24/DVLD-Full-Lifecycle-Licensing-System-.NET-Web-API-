using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entites
{
    public class DetainedLicenseDto
    {
        public int DetainID { get; set; }
        public int? PersonalID { get; set; }
        public int LicenseID { get; set; }
        public DateTime DetainDate { get; set; }
        public bool IsReleased { get; set; }
        public decimal FineFees { get; set; } 
        public DateTime? ReleaseDate { get; set; } 
        public string NationalNo { get; set; } = string.Empty;
        public string? FullName { get; set; } = string.Empty;
        public int? ReleaseApplicationID { get; set; }
    }

    public class QueryParams
    {
        [Key]
        public int id { get; set; }

        public int ID { get; set; } 
        public int LicenseID { get; set; }
    }
}
