using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DVLDServices.Services
{
    public class TestAppointmentService
    {

        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        public TestAppointmentService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<List<TestAppointmentDto>> GetTestAppointmentsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/TestAppointment/all");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<TestAppointmentDto>>(_jsonOptions) ?? new List<TestAppointmentDto>();
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ {response.StatusCode}: {error}");
            }
            catch (HttpRequestException)
            {
                throw new Exception("تعذر الاتصال بالسيرفر. تأكد من تشغيل الـ API.");
            }
        }
        public async Task<List<TestAppointmentDto>> GetTestAppointmentByLocalAppIdAndTestTypeIdAsync(int localAppID, int testTypeID)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/TestAppointment/GetTestAppointmentBy/{localAppID}/{testTypeID}");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<TestAppointmentDto>>(_jsonOptions) ?? new List<TestAppointmentDto>();
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ {response.StatusCode}: {error}");
            }
            catch (HttpRequestException)
            {
                throw new Exception("تعذر الاتصال بالسيرفر. تأكد من تشغيل الـ API.");
            }
        }
        public async Task<TestAppointmentDto> GetTestAppointmentByIdAsync(int id)
        {
            try
            {
            
                var response = await _httpClient.GetAsync($"api/TestAppointment/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<TestAppointmentDto>(_jsonOptions);
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ {response.StatusCode}: {error}");
            }
            catch (HttpRequestException)
            {
                throw new Exception("تعذر الاتصال بالسيرفر. تأكد من تشغيل الـ API.");
            }
        }

        public async Task<TestAppointment> AddTestAppointmentAsync(TestAppointment TestAppointment)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/TestAppointment/AddTestAppointment", TestAppointment, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<TestAppointment>(_jsonOptions);
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ من السيرفر: {error}");
            }
            catch (Exception ex)
            {
                throw new Exception($"فشلت عملية الإضافة: {ex.Message}");
            }
        }
        public async Task<TestAppointment> UpdateTestAppointment(int id, TestAppointment TestAppointment)
        {
            try
            {
             
                var response = await _httpClient.PutAsJsonAsync($"api/TestAppointment/UpdateTestAppointment/{id}", TestAppointment, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<TestAppointment>(_jsonOptions);
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ من السيرفر: {error}");
            }
            catch (Exception ex)
            {
                throw new Exception($"فشلت عملية التحديث: {ex.Message}");
            }
        }
        public async Task<string> DeleteTestAppointmentAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/TestAppointment/DeleteTestAppointment/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return "Deleted Successfully";
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return $"This TestAppointment is not found : {id}";

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ {response.StatusCode}: {error}");
            }
            catch (HttpRequestException)
            {
                throw new Exception("تعذر الاتصال بالسيرفر. تأكد من تشغيل الـ API.");
            }
        }

        public async Task<bool> CheckIfPassedTest(int localAppID, int testTypeID)
        {
            var response = await _httpClient.GetAsync($"api/TestAppointment/DoesPassTestType/{localAppID}/{testTypeID}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<TestResultResult>(_jsonOptions);
                return result != null && result.TestResult;
            }
            return false;
        }


        public async Task<int> GetTotalTrialsPerTest(int localAppID, int testTypeID)
        {
            var response = await _httpClient.GetAsync($"api/TestAppointment/GetTotalTrialsPerTest/{localAppID}/{testTypeID}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<TestResultResult>(_jsonOptions);
                return result != null ? result.returnNum : 0;
            }
            return 0;
        }
        public async Task<int> IsThereAnActiveScheduledTest(int localAppID, int testTypeID)
        {
            var response = await _httpClient.GetAsync($"api/TestAppointment/IsThereAnActiveScheduledTest/{localAppID}/{testTypeID}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<TestResultResult>(_jsonOptions);
                return result != null ? result.returnNum : 0;
            }
            return 0;
        }

        public async Task<int> DoesAttendTestType(int localAppID, int testTypeID)
        {
            var response = await _httpClient.GetAsync($"api/TestAppointment/DoesAttendTestType/{localAppID}/{testTypeID}");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<TestResultResult>(_jsonOptions);
                return result != null ? result.returnNum : 0;
            }
            return 0;
        }


        public enum enTestType
        {
            VisionTest = 1,
            WrittenTest = 2,
            StreetTest = 3
        };

        public class TestResultResult
        {
            public bool TestResult { get; set; }
            public int returnNum { get; set; }
        }
        public class TestAppointmentDto
        {
            public int TestAppointmentID { get; set; }
            public int TestTypeID { get; set; }
            public float PaidFees { get; set; }
            public int LocalDrivingLicenseApplicationID { get; set; }
            public DateTime AppointmentDate { get; set; }
            public int CreatedByUserID { get; set; }
            public byte IsLocked { get; set; }
            public int RetakeTestApplicationID { get; set; }
        }


        public class TestAppointment
        {
            public int TestAppointmentID { get; set; }
            public int TestTypeID { get; set; }
            public float PaidFees { get; set; }
            public int LocalDrivingLicenseApplicationID { get; set; }
            public DateTime AppointmentDate { get; set; }
            public int CreatedByUserID { get; set; }
            public byte IsLocked { get; set; }
            public int RetakeTestApplicationID { get; set; }
        }
    


    }
}


