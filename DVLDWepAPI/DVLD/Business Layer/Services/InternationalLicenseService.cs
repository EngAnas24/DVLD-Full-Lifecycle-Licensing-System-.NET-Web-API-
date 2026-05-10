using DAL.DataAccessLayer.Common;
using DataAccessLayer.Dtos;
using Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer.Services
{
    public class InternationalLicenseService
    {
        public List<DataAccessLayer.Dtos.InternationalLicenseDto> GetAllInternationalLicenses()
        {
            return SqlHelper.ExecuteQueryAll<DataAccessLayer.Dtos.InternationalLicenseDto>("SPGetAllInternationalLicenses");
        }
        public DataAccessLayer.Dtos.InternationalLicenseDto GetInternationalLicenseByID(int id)
        {
            int returnedId;

            var InternationalLicense = SqlHelper.ExecuteeQuerySingl<DataAccessLayer.Dtos.InternationalLicenseDto>(
                "SPGetInternationalLicenseByID",
                new { ID = id },
                out returnedId
            );

            return InternationalLicense;
        }
        public InternationalLicenseDto GetInternationalLicenseByDriverID(int DriverID)
        {
            int returnedId;
            var ActiveInternationalLicense = new  { ID = DriverID};
            var InternationalLicense = SqlHelper.ExecuteeQuerySingl<InternationalLicenseDto>(
                "SPGetInternationalLicenseByDriverID",
                 ActiveInternationalLicense,
                out returnedId
            );

            return InternationalLicense;
        }


        public List<InternationalLicenseDto> GetInternationalLicensesByDriverID(int DriverID)
        {

            var ActiveInternationalLicense = new { ID = 0 , DriverID = DriverID };
            var InternationalLicense = SqlHelper.ExecuteQueryAll<InternationalLicenseDto>(
                "SPGetInternationalLicensesByDriverID",
                 ActiveInternationalLicense
      
            );

            return InternationalLicense;
        }

        public DataAccessLayer.Dtos.GetInternationalImagePath GetImagePathByApplicationID(int id)
        {
            int returnedId;

            var InternationalLicense = SqlHelper.ExecuteeQuerySingl<DataAccessLayer.Dtos.GetInternationalImagePath>(
                "SPGetInternationalImagePath",
                new { ID = id },
                out returnedId
            );

            return InternationalLicense;
        }

        public int InsertInternationalLicense(InternationalLicense InternationalLicense)
        {
            return SqlHelper.ExecuteNonQuery("SPInsertUpdateInternationalLicense", InternationalLicense, true);
        }
        public int UpdateInternationalLicense(InternationalLicense InternationalLicense)
        {
            return SqlHelper.ExecuteNonQuery("SPInsertUpdateInternationalLicense", InternationalLicense, isInsert: false);
        }
        public int DeleteInternationalLicense(int id)
        {
            return SqlHelper.ExecuteNonQuery("SPDeleteInternationalLicense", new { InternationalLicensealID = id });
        }


    }

}
