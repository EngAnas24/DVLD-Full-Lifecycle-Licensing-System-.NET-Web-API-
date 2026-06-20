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
    public class TestService
    {

        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        public TestService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<List<TestDto>> GetTestsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Test/all");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<TestDto>>(_jsonOptions) ?? new List<TestDto>();
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ {response.StatusCode}: {error}");
            }
            catch (HttpRequestException)
            {
                throw new Exception("تعذر الاتصال بالسيرفر. تأكد من تشغيل الـ API.");
            }
        }

        public async Task<TestDto> GetTestByIdAsync(int id)
        {
            try
            {
            
                var response = await _httpClient.GetAsync($"api/Test/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<TestDto>(_jsonOptions);
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
        public async Task<TestDto> GetTestByTestAppointmentId(int TestAppointmentId)
        {
            try
            {

                var response = await _httpClient.GetAsync($"api/Test/appointment/{TestAppointmentId}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<TestDto>(_jsonOptions);
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
        public async Task<Test> AddTestAsync(Test Test)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Test/AddTest", Test, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Test>(_jsonOptions);
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ من السيرفر: {error}");
            }
            catch (Exception ex)
            {
                throw new Exception($"فشلت عملية الإضافة: {ex.Message}");
            }
        }
        public async Task<Test> UpdateTest(int id, Test Test)
        {
            try
            {
             
                var response = await _httpClient.PutAsJsonAsync($"api/Test/UpdateTest/{id}", Test, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<Test>(_jsonOptions);
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ من السيرفر: {error}");
            }
            catch (Exception ex)
            {
                throw new Exception($"فشلت عملية التحديث: {ex.Message}");
            }
        }
        public async Task<string> DeleteTestAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Test/DeleteTest/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return "Deleted Successfully";
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return $"This Test is not found : {id}";

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ {response.StatusCode}: {error}");
            }
            catch (HttpRequestException)
            {
                throw new Exception("تعذر الاتصال بالسيرفر. تأكد من تشغيل الـ API.");
            }
        }
        public async Task<LastTestResult> GetLastTest(int LicenseClassID, int ApplicantPersonID, int TestTypeID)
        {
            try
            {

                var response = await _httpClient.GetAsync($"api/Test/GetLastTest/{LicenseClassID}/{ApplicantPersonID}/{TestTypeID}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<LastTestResult>(_jsonOptions);
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
        public async Task<ClassNameResponse> GetClassID(string ClassName)
        {
            try
            {

                var response = await _httpClient.GetAsync($"api/Test/GetClassID/{ClassName}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ClassNameResponse>(_jsonOptions);
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

        public class ClassNameRequest
        {
            public string ClassName { get; set; }
        }
        public class ClassNameResponse
        {
            public int LicenseClassID { get; set; }
        }
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
            public int LicenseClassID { get; set; }
            public int TestTypeID { get; set; }
            public int ApplicantPersonID { get; set; }
        }
        public class TestDto
        {
            public int TestID { get; set; }
            public int TestAppointmentID { get; set; }
            public byte TestResult { get; set; }
            public string Notes { get; set; }
            public int CreatedByUserID { get; set; }
        }


        public class Test
        {
            public int TestID { get; set; }
            public int TestAppointmentID { get; set; }
            public byte TestResult { get; set; }
            public string Notes { get; set; }
            public int CreatedByUserID { get; set; }
        }
    


    }
}


