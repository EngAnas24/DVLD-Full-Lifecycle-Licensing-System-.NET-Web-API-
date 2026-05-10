using DVDL.Domain.Entities;
using DAL.DataAccessLayer.Common;
using DataAccessLayer.Dtos;
using Entites;
namespace BusinessLayer.Services
{
    public class ApplicationService
    {
        public List<DataAccessLayer.Dtos.ApplicationDto> GetAllApplication()
        {
            return SqlHelper.ExecuteQueryAll<DataAccessLayer.Dtos.ApplicationDto>("SPGetAllApplication");
        }
        public DataAccessLayer.Dtos.ApplicationDto GetApplicationById(int id)
        {
            int returnedId;

            var Application = SqlHelper.ExecuteeQuerySingl<DataAccessLayer.Dtos.ApplicationDto>(
                "SPGetApplicationById",
                new { ID = id }, 
                out returnedId
            );

            return Application;
        }
        public int ChangeStatusToCompleted(int id)
        {
            int returnedId;

            var Application = SqlHelper.ExecuteeQuerySingl<int>(
                "SPChangeStatusToCompleted",
                new { ID = id },
                out returnedId
            );

            return returnedId;
        }
        public int InsertApplication(Application Application)
        {
            return SqlHelper.ExecuteNonQuery("[SPInsertUpdateApplication]", Application, true);
        }
        public int UpdateApplication(Application Application)
        {
            return SqlHelper.ExecuteNonQuery("[SPInsertUpdateApplication]", Application, isInsert: false);
        }
        public int DeleteApplication(int id)
        {
            return SqlHelper.ExecuteNonQuery("SPDeleteApplication", new { ApplicationalID = id });
        }

    
    }
}


