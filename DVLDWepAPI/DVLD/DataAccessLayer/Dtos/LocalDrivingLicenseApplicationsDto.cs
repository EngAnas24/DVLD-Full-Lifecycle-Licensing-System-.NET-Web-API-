using DVDL.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entites
{
    public class LocalDrivingLicenseApplicationsDto 
{
    public int LocalDrivingLicenseApplicationID { get; set; }
    public int ApplicationID { get; set; }

    public string ClassName { get; set; } = string.Empty;

    public DateTime ApplicationDate { get; set; }

    public string NationalNo { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;

    public int PassedTestCount { get; set; }

    public string Status { get; set; } = string.Empty;
}
}
