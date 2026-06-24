using DAL.DataAccessLayer.Common;
using DataAccessLayer.Dtos;
using Entites;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer.Services
{
    public class DetainedLicenseService
    {
        public List<DetainedLicenseDto> GetAllDetainedLicenses()
        {
            return SqlHelper.ExecuteQueryAll<DetainedLicenseDto>("SPDetainedLicenses");
        }
        public DetainedLicenseDto GetDetainedLicenseByID(int LicenseID)
        {
            int returnedId;

            var DetainedLicense = SqlHelper.ExecuteeQuerySingl<DetainedLicenseDto>(
                "SPGetDetainedLicenseByID",
                new { ID = LicenseID },
                out returnedId
            );

            return DetainedLicense;
        }
        public DetainedLicenseDto GetDetainedLicenseByPersonID(int id)
        {
            int returnedId;

            var DetainedLicense = SqlHelper.ExecuteeQuerySingl<DetainedLicenseDto>(
                "SPGetDetainedLicenseByPersonID",
                new { ID = id },
                out returnedId
            );

            return DetainedLicense;
        }
        public int InsertDetainedLicense(DetainedLicense DetainedLicense)
        {
            return SqlHelper.ExecuteNonQuery("[sp_InsertUpdateDetainedLicense]", DetainedLicense, true);
        }
        public int UpdateDetainedLicense(DetainedLicense DetainedLicense)
        {
            return SqlHelper.ExecuteNonQuery("[sp_InsertUpdateDetainedLicense]", DetainedLicense, isInsert: false);
        }
        public int DeleteDetainedLicense(int id)
        {
            return SqlHelper.ExecuteNonQuery("SPDeleteDetainedLicense", new { DetainedLicenseID = id });
        }

        public bool CheckIsLicenseDetained(int licenseID)
        {
            int outId = 0;

            var queryParams = new QueryParams
            {
                LicenseID = licenseID,
                ID = 0
            };

            SqlHelper.ExecuteeQuerySingl<bool>(
                "sp_CheckIsLicenseDetained",
                queryParams,
                out outId
            );

            return outId == 1;
        }
        public ReleaseTransactionParams ReleaseDetainedLicense(ReleaseTransactionParams releaseParams)
        {
            int outId = 0;

            SqlHelper.ExecuteeQuerySingl<bool>(
                "sp_ReleaseDetainedLicense",
                releaseParams,
                out outId
            );

            if (outId > 0)
            {
                releaseParams.DetainID = outId;

                return releaseParams;
            }

            return null;
        }
        public int FindPersonByLicenseId(int licenseId)
        {
            var parameters = new FindPersonByLicenseIdModel
            {
                ID = 0,
                LicenseID = licenseId
            };

            int outId = 0;
            SqlHelper.ExecuteeQuerySingl<bool>("SPFindPersonByLicenseId", parameters, out outId);

            return outId; 
        }
        public class FindPersonByLicenseIdModel
            {
                [Key]
                public int ID { get; set; }

                public int LicenseID { get; set; }
            }



}
}
