using DAL.DataAccessLayer.Common;
using DataAccessLayer.Dtos;
using Entites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer.Services
{
    public class TestService
    {
        public List<TestDto> GetAllTest()
        {
            return SqlHelper.ExecuteQueryAll<TestDto>("SPGetAllTests");
        }
        public TestDto GetTestById(int id)
        {
            int outId = 0;
            return SqlHelper.ExecuteeQuerySingl<TestDto>(
                "SPGetTestById",
                new { ID = id },
                out outId
            );
        }
        public TestDto GetTestByTestAppointmentId(int id)
        {
            int outId = 0;
            return SqlHelper.ExecuteeQuerySingl<TestDto>(
                "SPGetTestByTestAppointmentId",
                new { ID = id },
                out outId
            );
        }
        public int InsertTest(Test Test)
        {
            return SqlHelper.ExecuteNonQuery("SPInsertUpdateTest", Test, true);
        }
        public int UpdateTest(Test Test)
        {
            return SqlHelper.ExecuteNonQuery("SPInsertUpdateTest", Test, isInsert: false);
        }
        public int DeleteTest(int id)
        {
            return SqlHelper.ExecuteNonQuery("SPDeleteTest", new { TestalID = id });
        }

        public LastTestResult GetLastTestByPersonAndTestTypeAndLicenseClass(int LicenseClassID, int TestTypeID, int ApplicantPersonID)
        {
            int outId = 0;
            var queryParams = new GetLastTestDTO
            {
                TestID = -1, 
                LicenseClassID = LicenseClassID,
                TestTypeID = TestTypeID,
                ApplicantPersonID = ApplicantPersonID
            };

            return SqlHelper.ExecuteeQuerySingl<LastTestResult>("SPGetLastTest", queryParams, out outId);
        }

        public ClassNameResponse GetClassID(string LicenseClassName)
        {
            int outId = 0;
            var queryParams = new ClassNameRequest
            {

                ClassName = LicenseClassName
            };

            return SqlHelper.ExecuteeQuerySingl<ClassNameResponse>("SPGetClassID", queryParams, out outId);
        }

        public class ClassNameRequest
        {
            public string ClassName { get; set; }            
        }
        public class ClassNameResponse
        {
            public int LicenseClassID { get; set; }
        }
        // 1. كلاس للنتيجة (يستخدم لاستقبال البيانات من SQL)
        public class LastTestResult
        {
            public int TestID { get; set; }
            public int TestAppointmentID { get; set; }
            public bool TestResult { get; set; }
            public string Notes { get; set; }
            public int CreatedByUserID { get; set; }
            public int ApplicantPersonID { get; set; }
        }

        public class GetLastTestDTO
        {
            // أضف هذه السمة لكي يعرف الـ SqlHelper أن هذا هو الـ ID المقصود فقط
            [Key]
            public int TestID { get; set; }

            public int LicenseClassID { get; set; }
            public int TestTypeID { get; set; }
            public int ApplicantPersonID { get; set; }
        }
    }
}



