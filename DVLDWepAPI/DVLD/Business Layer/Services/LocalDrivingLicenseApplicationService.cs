using DAL.DataAccessLayer.Common;
using Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer.Services
{
    public class LocalDrivingLicenseApplicationService
    {
        public List<LocalDrivingLicenseApplicationsDto> GetLocalDrivingLicenseApplications()
        {

            return SqlHelper.ExecuteQueryAll<LocalDrivingLicenseApplicationsDto>(
                "SpGetLocalDrivingLicenseApplications");
        }
        public LocalDrivingLicenseApplicationsDto GetLocalApplicationByIdAsync(int id)
        {
            int returnedId;

            var LocalApplication = SqlHelper.ExecuteeQuerySingl<LocalDrivingLicenseApplicationsDto>(
                "SPGetLocalDrivingLicenseApplicationById",
                new { ID = id }, 
                out returnedId
            );

            return LocalApplication;
        }

        public LocalDrivingLicenseApplicationsDto GetLocalApplicationByApplicationId(int id)
        {
            int returnedId;

            var LocalApplication = SqlHelper.ExecuteeQuerySingl<LocalDrivingLicenseApplicationsDto>(
                "SPGetLocalApplicationByApplicationId",
                new { ID = id },
                out returnedId
            );

            return LocalApplication;
        }


        public int InsertLocalApplication(LocalDrivingLicenseApplications LocalApplication)
        {
            return SqlHelper.ExecuteNonQuery("[SPInsertUpdateLocalDrivingLicenseApplications]", LocalApplication, true);
        }
        public int UpdateLocalApplication(LocalDrivingLicenseApplications LocalApplication)
        {
            return SqlHelper.ExecuteNonQuery("[SPInsertUpdateLocalDrivingLicenseApplications]", LocalApplication, isInsert: false);
        }
        public int DeleteLocalApplication(int id)
        {
            return SqlHelper.ExecuteNonQuery("SPDeleteLocalApplication", new { LocalApplicationID = id });
        }
    }
}
