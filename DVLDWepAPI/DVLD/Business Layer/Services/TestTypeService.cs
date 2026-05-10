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
    public class TestTypeService
    {
        public List<TestTypeDto> GetAllTestType()
        {
            return SqlHelper.ExecuteQueryAll<TestTypeDto>("SPGetAllTestType");
        }
        public static TestTypeDto GetTestTypeById(int id)
        {
            int returnedId;
            var appType = SqlHelper.ExecuteeQuerySingl<TestTypeDto>(
                "SPGetTestTypeById",
                new { ID = 0, TestTypeID = id },
                out returnedId
            );

            if (returnedId == -1)
            {
                return null;
            }

            return appType;
        }
        public int InsertTestType(TestType TestType)
        {
            return SqlHelper.ExecuteNonQuery("[SPInsertUpdateTestType]", TestType, true);
        }
        public int UpdateTestType(TestType TestType)
        {
            return SqlHelper.ExecuteNonQuery("[SPInsertUpdateTestType]", TestType, isInsert: false);
        }
        public int DeleteTestType(int id)
        {
            return SqlHelper.ExecuteNonQuery("SPDeleteTestType", new { TestTypeID = id });
        }


    }
}
