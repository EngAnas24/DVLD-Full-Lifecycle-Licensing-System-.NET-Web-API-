using DAL.DataAccessLayer.Common;
using DataAccessLayer.Dtos;
using DVDL.Domain.Entities;
using Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Dtos;

namespace Business_Layer.Services
{
    public class ApplicationTypeService
    {
            public List<ApplicationTypeDto> GetAllApplicationType()
            {
                return SqlHelper.ExecuteQueryAll<ApplicationTypeDto>("SPGetAllApplicationType");
            }
        public static ApplicationTypeDto GetApplicationTypeById(int id)
        {
            int returnedId;
            var appType = SqlHelper.ExecuteeQuerySingl<ApplicationTypeDto>(
                "SPGetApplicationTypeById",
                new { ID = 0, ApplicationTypeID = id },
                out returnedId
            );

            if (returnedId == -1)
            {
                return null;
            }

            return appType;
        }
        public int InsertApplicationType(ApplicationType ApplicationType)
            {
                return SqlHelper.ExecuteNonQuery("[SPInsertUpdateApplicationType]", ApplicationType, true);
            }
            public int UpdateApplicationType(ApplicationType ApplicationType)
            {
                return SqlHelper.ExecuteNonQuery("[SPInsertUpdateApplicationType]", ApplicationType, isInsert: false);
            }
            public int DeleteApplicationType(int id)
            {
                return SqlHelper.ExecuteNonQuery("SPDeleteApplicationType", new { ApplicationTypealID = id });
            }
        

    }
}
