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
    public class LicenseClassService
    {

        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        public LicenseClass licenseClass;
        public LicenseClassService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<List<LicenseClassDto>> GetLicenseClassesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/LicenseClass/all");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<LicenseClassDto>>(_jsonOptions) ?? new List<LicenseClassDto>();
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ {response.StatusCode}: {error}");
            }
            catch (HttpRequestException)
            {
                throw new Exception("تعذر الاتصال بالسيرفر. تأكد من تشغيل الـ API.");
            }
        }

        public async Task<LicenseClassDto> GetLicenseClassByIdAsync(int ClassID)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/LicenseClass/{ClassID}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<LicenseClassDto>(_jsonOptions);
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
        public async Task<int> GetLicenseClassIDByLocalAppID(int Id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/LicenseClass/GetLicenseClassIDByLocalAppID/{Id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<int>(_jsonOptions);
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return 0;

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ {response.StatusCode}: {error}");
            }
            catch (HttpRequestException)
            {
                throw new Exception("تعذر الاتصال بالسيرفر. تأكد من تشغيل الـ API.");
            }
        }
        public async Task<LicenseClass> AddLicenseClassAsync(LicenseClass LicenseClass)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/LicenseClass/AddLicenseClass", LicenseClass, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<LicenseClass>(_jsonOptions);
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ من السيرفر: {error}");
            }
            catch (Exception ex)
            {
                throw new Exception($"فشلت عملية الإضافة: {ex.Message}");
            }
        }
        public async Task<LicenseClass> UpdateLicenseClass(int id, LicenseClass LicenseClass)
        {
            try
            {

                var response = await _httpClient.PutAsJsonAsync($"api/LicenseClass/UpdateLicenseClass/{id}", LicenseClass, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<LicenseClass>(_jsonOptions);
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ من السيرفر: {error}");
            }
            catch (Exception ex)
            {
                throw new Exception($"فشلت عملية التحديث: {ex.Message}");
            }
        }
        public async Task<string> DeleteLicenseClassAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/LicenseClass/DeleteLicenseClass/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return "Deleted Successfully";
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return $"This LicenseClass is not found : {id}";

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ {response.StatusCode}: {error}");
            }
            catch (HttpRequestException)
            {
                throw new Exception("تعذر الاتصال بالسيرفر. تأكد من تشغيل الـ API.");
            }
        }


        public class LicenseClassDto
        {
            public int LicenseClassID { get; set; }
            public string ClassName { get; set; }
            public string ClassDescription { get; set; }
            public int MinimumAllowedAge { get; set; }
            public int DefaultValidityLength { get; set; }
            public float ClassFees { get; set; }
        }

        public class LicenseClass
        {
            public int LicenseClassID { get; set; }
            public string ClassName { get; set; }
            public string ClassDescription { get; set; }
            public int MinimumAllowedAge { get; set; }
            public int DefaultValidityLength { get; set; }
            public decimal ClassFees { get; set; }

        }


    }
}


