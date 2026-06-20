using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entites
{
    public class DetainedLicense
    {
        public int ID { get; set; }
        public int LicenseID { get; set; }
        public DateTime DetainDate { get; set; }
        public bool IsReleased { get; set; }
        public decimal FineFees { get; set; } 
        public DateTime? ReleaseDate { get; set; }
        public int? ReleaseApplicationID { get; set; } = 0;
        public int ?ReleasedByUserID { get; set; } = 0;
        public int? CreatedByUserID { get; set; }
    }

    public class ReleaseTransactionParams
    {
        // --- 1. خواص الـ Application الكاملة كما أرسلتها ---
        public int ApplicationID { get; set; }
        public int ApplicantPersonID { get; set; }
        public DateTime ApplicationDate { get; set; }
        public int ApplicationTypeID { get; set; }
        public int ApplicationStatus { get; set; }
        public DateTime LastStatusDate { get; set; }
        public decimal PaidFees { get; set; }
        public int CreatedByUserID { get; set; } // مستخدم إنشاء الطلب

        // --- 2. خواص الـ DetainedLicense الكاملة كما أرسلتها ---
        public int DetainID { get; set; }
        public int LicenseID { get; set; }
        public DateTime DetainDate { get; set; }
        public bool IsReleased { get; set; }
        public decimal FineFees { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public int ReleaseApplicationID { get; set; }
        public int ReleasedByUserID { get; set; } // مستخدم فك الاحتجاز

        // --- 3. الحقل الإجباري للـ SqlHelper (outId) ---
        [Key]
        public int ID { get; set; }
    }
}
