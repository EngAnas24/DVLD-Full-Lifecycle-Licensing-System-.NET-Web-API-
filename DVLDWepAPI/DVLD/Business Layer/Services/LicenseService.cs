using DVDL.Domain.Entities;
using DAL.DataAccessLayer.Common;
using DataAccessLayer.Dtos;
using Entites;
namespace BusinessLayer.Services
{
    public class LicenseService
    {
        public List<DataAccessLayer.Dtos.LicenseDto> GetAllLicenses()
        {
            return SqlHelper.ExecuteQueryAll<DataAccessLayer.Dtos.LicenseDto>("SPGetAllLicenses");
        }
        public DataAccessLayer.Dtos.LicenseDto GetLicenseByID(int id)
        {
            int returnedId;

            var License = SqlHelper.ExecuteeQuerySingl<DataAccessLayer.Dtos.LicenseDto>(
                "SPGetLicenseByID",
                new { ID = id }, 
                out returnedId
            );

            return License;
        }

        public List<LicenseDto> GetLicensesByPersonId(int id)
        {

            var License = SqlHelper.ExecuteQueryAll<DataAccessLayer.Dtos.LicenseDto>(
                "SPGetLicensesByPersonId",
                new { ID = 0 , PersonID = id }
     
            );

            return License;
        }
        public int GetActiveLicenseByPersonID(int PersonId, int LicenseClassId)
        {
            int returnedId;
            var ActiveLicense = new ActiveLicense { ID = PersonId, LicenseClassId = LicenseClassId };
            var License = SqlHelper.ExecuteeQuerySingl<int>(
                "SpGetActiveLicenseIDByPersonID",
                 ActiveLicense,
                out returnedId
            );

            return returnedId;
        }
        public ActiveLicense GetPersonIdAndLicenseClass(int Id)
        {
            int returnedId;
            var ActiveLicense = new { ID = Id };
            var GetLicense = SqlHelper.ExecuteeQuerySingl<ActiveLicense>(
                "SPGetLicenseByPersonIdAndLicenseClass",
                 ActiveLicense,
                out returnedId
            );

            return GetLicense;
        }

        public int DeactivateLicense(int Id)
        {
            int returnedId;
            var DeactivateLicense = new { ID = Id };
            var GetLicense = SqlHelper.ExecuteeQuerySingl<int>(
                "SPDeactivateLicense",
                 DeactivateLicense,
                out returnedId
            );

            return returnedId;
        }

        public DataAccessLayer.Dtos.GetImagePath GetImagePathByApplicationID(int id)
        {
            int returnedId;

            var License = SqlHelper.ExecuteeQuerySingl<DataAccessLayer.Dtos.GetImagePath>(
                "SPGetImagePath",
                new { ID = id },
                out returnedId
            );

            return License;
        }

        public int InsertLicense(License License)
        {
            return SqlHelper.ExecuteNonQuery("SPInsertUpdateLicense", License, true);
        }
        public int UpdateLicense(License License)
        {
            return SqlHelper.ExecuteNonQuery("SPInsertUpdateLicense", License, isInsert: false);
        }


        public int DeleteLicense(int id)
        {
            return SqlHelper.ExecuteNonQuery("SPDeleteLicense", new { LicensealID = id });
        }

     
    }
}



