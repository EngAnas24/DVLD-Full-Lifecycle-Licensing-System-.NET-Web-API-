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
        public class TestAppointmentService
        {
            public List<TestAppointmentDto> GetAllTestAppointment()
            {
                return SqlHelper.ExecuteQueryAll<TestAppointmentDto>("SPGetAllTestAppointment");
            }
        public TestAppointmentDto GetTestAppointmentById(int id)
        {
            int outId = 0;
            return SqlHelper.ExecuteeQuerySingl<TestAppointmentDto>(
                "SPGetTestAppointmentById",
                new { ID = id },
                out outId
            );
        }
        public TestAppointmentDto GetTestAppointmentByTestTypeID(int id)
        {
            int outId = 0;
            return SqlHelper.ExecuteeQuerySingl<TestAppointmentDto>(
                "SPGetTestAppointmentByTestTypeID",
                new { ID = id },
                out outId
            );
        }

        public List<TestAppointmentDto> GetTestAppointmentByLocalAppIdAndTestTypeId(int localAppID, int testTypeID)
        {

            var queryParams = new TestQueryParams { AppID = localAppID, TypeID = testTypeID };
            return SqlHelper.ExecuteQueryAll<TestAppointmentDto>(
                "SPGetTestAppointmentByLocalAppIdAndTestTypeId",
               queryParams
            );
        }
        public int InsertTestAppointment(TestAppointment TestAppointment)
            {
                return SqlHelper.ExecuteNonQuery("SPInsertUpdateTestAppointment", TestAppointment, true);
            }
            public int UpdateTestAppointment(TestAppointment TestAppointment)
            {
                return SqlHelper.ExecuteNonQuery("SPInsertUpdateTestAppointment", TestAppointment, isInsert: false);
            }
            public int DeleteTestAppointment(int id)
            {
                return SqlHelper.ExecuteNonQuery("SPDeleteTestAppointment", new { TestAppointmentalID = id });
            }


        public bool CheckIfPassedTest(int localAppID, int testTypeID)
        {
            int outId;

            // نستخدم كلاس مخصص بدلاً من new { ... } لضمان وجود خاصية ID
            var queryParams = new TestQueryParams { AppID = localAppID, TypeID = testTypeID };

            var result = SqlHelper.ExecuteeQuerySingl<TestResultResult>(
                "SPDoesPassTestType",
                queryParams,
                out outId
            );

            return result != null && result.TestResult;
        }


        public int GetTotalTrialsPerTest(int localAppID, int testTypeID)
        {
            int outId;

            // نستخدم كلاس مخصص بدلاً من new { ... } لضمان وجود خاصية ID
            var queryParams = new TestQueryParams { AppID = localAppID, TypeID = testTypeID };

            var result = SqlHelper.ExecuteeQuerySingl<TestResultResult>(
                "SPGetTotalTrialsPerTest",
                queryParams,
                out outId
            );

            return result != null ? result.returnNum : 0;
        }

        public int IsThereAnActiveScheduledTest(int localAppID, int testTypeID)
        {
            int outId;

            // نستخدم كلاس مخصص بدلاً من new { ... } لضمان وجود خاصية ID
            var queryParams = new TestQueryParams { AppID = localAppID, TypeID = testTypeID };

            var result = SqlHelper.ExecuteeQuerySingl<TestResultResult>(
                "SPIsThereAnActiveScheduledTest",
                queryParams,
                out outId
            );

            return result != null ? result.returnNum : 0;
        }
        public int DoesAttendTestType(int LocalicenseID, int TestTypeID)
        {
            int outId;

            var queryParams = new TestQueryParams { AppID = LocalicenseID, TypeID = TestTypeID };

            var result = SqlHelper.ExecuteeQuerySingl<TestResultResult>(
                "SPDoesAttendTestType",
                queryParams,
                out outId
            );

            return result != null ? result.returnNum : 0;
        }

    }
    }



