using DVDL.Domain.Entities;
using DAL.DataAccessLayer.Common;
using DataAccessLayer.Dtos;
using Entites;
namespace BusinessLayer.Services
    {
        public class LicenseClassService
    {
            public List<DataAccessLayer.Dtos.LicenseClassDto> GetAllLicenseClasses()
            {
                return SqlHelper.ExecuteQueryAll<DataAccessLayer.Dtos.LicenseClassDto>("SPGetAllLicenseClasses");
            }
            public DataAccessLayer.Dtos.LicenseClassDto GetLicenseClassByClassID(int id)
            {
                int returnedId;

                var LicenseClass = SqlHelper.ExecuteeQuerySingl<DataAccessLayer.Dtos.LicenseClassDto>(
                    "SPGetLicenseClassByClassID",
                    new { ID = id }, 
                    out returnedId
                );

                return LicenseClass;
            }
        public DataAccessLayer.Dtos.LicenseClassDto GetLicenseClassByPersonID(int id)
        {
            int returnedId;

            var LicenseClass = SqlHelper.ExecuteeQuerySingl<DataAccessLayer.Dtos.LicenseClassDto>(
                "SPGetLicenseClassByClassName",
                new { ID = id },
                out returnedId
            );

            return LicenseClass;
        }

        public int GetLicenseClassIDByLocalAppID(int id)
        {
            int returnedId;

            var LicenseClassID = SqlHelper.ExecuteeQuerySingl<int>(
                "SPGetLicenseClassIDByLocalDrivingLicenseApplicationID",
                new { ID = id },
                out returnedId
            );

            return returnedId;
        }

        public int InsertLicenseClass(LicenseClass LicenseClass)
            {
                return SqlHelper.ExecuteNonQuery("[SPInsertUpdateLicenseClass]", LicenseClass, true);
            }
            public int UpdateLicenseClass(LicenseClass LicenseClass)
            {
                return SqlHelper.ExecuteNonQuery("[SPInsertUpdateLicenseClass]", LicenseClass, isInsert: false);
            }
            public int DeleteLicenseClass(int id)
            {
                return SqlHelper.ExecuteNonQuery("SPDeleteLicenseClass", new { LicenseClassalID = id });
            }
         
        }

    }



