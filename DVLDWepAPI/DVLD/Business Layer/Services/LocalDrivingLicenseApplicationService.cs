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
        public int GetPeronIdbyApplicationId(int id)
        {
            int returnedId;

            var LocalApplication = SqlHelper.ExecuteeQuerySingl<int>(
                "SPGetPeronIdbyApplicationId",
                new { ID = id },
                out returnedId
            );

            return returnedId;
        }
        public int HasApplication(int personId, int licenseClassId)
        {
            int returnedId;

            var LocalApplication = SqlHelper.ExecuteeQuerySingl<int>(
                "SPHasActiveApplication",
                new { Id = 0, PersonId = personId, LicenseClassID = licenseClassId },
                out returnedId
            );

            return returnedId;
        }
        public int HasAlreadyLicense (int personId, int licenseClassId)
        {
            int returnedId;

            var LocalApplication = SqlHelper.ExecuteeQuerySingl<int>(
                "SPHasAlreadyLicense",
                new { Id = 0, PersonId = personId, LicenseClassID = licenseClassId },
                out returnedId
            );

            return returnedId;
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

        public int CreateLocalDrivingLicenseApplication(CreateLocalDrivingLicenseAppDto dto)
        {
            int newLocalAppId;
            try
            {
                var requestData = new CreateLocalDrivingLicenseRequest
                {
                    FakeId = 0, 
                    ApplicantPersonID    = dto.ApplicantPersonID,
                    ApplicationDate     = DateTime.Now,
                    ApplicationTypeID    = dto.ApplicationTypeID,
                    ApplicationStatus   = 1, // New
                    LastStatusDate       = DateTime.Now,
                    PaidFees            = dto.PaidFees,
                    CreatedByUserID     = dto.CreatedByUserID,
                    LicenseClassID      =     dto.LicenseClassID,
                    
                };

                SqlHelper.ExecuteeQuerySingl<dynamic>(
                    "SP_CreateLocalDrivingLicenseApplicationWithTransaction",
                    requestData,
                    out newLocalAppId
                );

                return newLocalAppId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Transaction Error: {ex.Message}", ex);
            }
        }

        public int UpdateLocalDrivingLicenseApplication(int id ,UpdateLocalDrivingLicenseAppDto dto)
        {
            int updatedLocalAppId;
            try
            {
                var requestData = new UpdateLocalDrivingLicenseAppRequest
                {
                    FakeId = 0, 
                    ApplicationStatus = dto.ApplicationStatus,
                    LocalApplicationID = id,
                    ApplicationID = dto.ApplicationID,
                    LastStatusDate = DateTime.Now,
                    LicenseClassID = dto.LicenseClassID,

                };

                SqlHelper.ExecuteeQuerySingl<dynamic>(
                    "SP_UpdateLocalDrivingLicenseApplicationWithTransaction",
                    requestData,
                    out updatedLocalAppId
                );

                return updatedLocalAppId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Database Transaction Error: {ex.Message}", ex);
            }
        }
    }
}
