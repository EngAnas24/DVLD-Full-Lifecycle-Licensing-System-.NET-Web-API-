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
    public class TestTypeService
    {

        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        public TestTypeService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<List<TestTypeDto>> GetTestTypesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/TestType/all");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<TestTypeDto>>(_jsonOptions) ?? new List<TestTypeDto>();
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ {response.StatusCode}: {error}");
            }
            catch (HttpRequestException)
            {
                throw new Exception("تعذر الاتصال بالسيرفر. تأكد من تشغيل الـ API.");
            }
        }

        public async Task<TestTypeDto> GetTestTypeByIdAsync(int id)
        {
            try
            {
            
                var response = await _httpClient.GetAsync($"api/TestType/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<TestTypeDto>(_jsonOptions);
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

        public async Task<TestType> AddTestTypeAsync(TestType TestType)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/TestType/AddTestType", TestType, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<TestType>(_jsonOptions);
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ من السيرفر: {error}");
            }
            catch (Exception ex)
            {
                throw new Exception($"فشلت عملية الإضافة: {ex.Message}");
            }
        }
        public async Task<TestType> UpdateTestType(int id, TestType TestType)
        {
            try
            {
             
                var response = await _httpClient.PutAsJsonAsync($"api/TestType/UpdateTestType/{id}", TestType, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<TestType>(_jsonOptions);
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ من السيرفر: {error}");
            }
            catch (Exception ex)
            {
                throw new Exception($"فشلت عملية التحديث: {ex.Message}");
            }
        }
        public async Task<string> DeleteTestTypeAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/TestType/DeleteTestType/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return "Deleted Successfully";
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return $"This TestType is not found : {id}";

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ {response.StatusCode}: {error}");
            }
            catch (HttpRequestException)
            {
                throw new Exception("تعذر الاتصال بالسيرفر. تأكد من تشغيل الـ API.");
            }
        }


        public class TestTypeDto
        {
            public int TestTypeID { get; set; }
            public string TestTypeTitle { get; set; }
            public string TestTypeDescription { get; set; }
            public decimal TestTypeFees { get; set; }
        }


        public class TestType
        {
            public int TestTypeID { get; set; }
            public string TestTypeTitle { get; set; }
            public string TestTypeDescription { get; set; }
            public decimal TestTypeFees { get; set; }
        }
    


    }
}


