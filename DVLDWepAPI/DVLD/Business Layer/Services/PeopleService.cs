using DVDL.Domain.Entities;
using DAL.DataAccessLayer.Common;
using DataAccessLayer.Dtos;
using Entites;
namespace BusinessLayer.Services
{
    public class PeopleService
    {
        public List<PeopleDto> GetAllPeople()
        {
            return SqlHelper.ExecuteQueryAll<PeopleDto>("SPGetAllPeople");
        }
        public PeopleDto GetPersonById(int id)
        {
            int returnedId;

            var person = SqlHelper.ExecuteeQuerySingl<PeopleDto>(
                "SPGetPeopleById",
                new { ID = id },
                out returnedId
            );

            return person;
        }
    
        public int InsertPeople(People people)
        {
            return SqlHelper.ExecuteNonQuery("SPInsertUpdatePeople", people, true);
        }
        public int UpdatePeople(People people)
        {
            return SqlHelper.ExecuteNonQuery("SPInsertUpdatePeople", people, isInsert: false);
        }
        public int DeletePeople(int id)
        {
            return SqlHelper.ExecuteNonQuery("SPDeletePeople", new { PersonalID = id });
        }
        public List<Country> GetAllCountries()
        {
            return SqlHelper.ExecuteQueryAll<Country>("SPGetAllCountries");
        }

        public int UpdateImage(object updateData)
        {
            return SqlHelper.ExecuteNonQuery("SP_UpdatePersonImagePath", updateData, false);
        }
        public int DeleteImage(int id)
        {
            return SqlHelper.ExecuteNonQuery("SPDeletePersonImage", new { PersonalID = id });
        }
    }

}
