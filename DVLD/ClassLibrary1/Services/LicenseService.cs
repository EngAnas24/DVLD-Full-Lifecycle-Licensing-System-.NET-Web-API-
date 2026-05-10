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
    public class LicenseService
    {

        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        public clsLicense _License;
        public LicenseService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<List<LicenseDto>> GetLicensesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/License/all");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<LicenseDto>>(_jsonOptions) ?? new List<LicenseDto>();
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ {response.StatusCode}: {error}");
            }
            catch (HttpRequestException)
            {
                throw new Exception("تعذر الاتصال بالسيرفر. تأكد من تشغيل الـ API.");
            }
        }

        public async Task<LicenseDto> GetLicenseByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/License/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<LicenseDto>(_jsonOptions);
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
        public async Task<List<LicenseDto>> GetLicensesByPersonIdAsync(int PersonID)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/License/LicensesBy/{PersonID}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<LicenseDto>>(_jsonOptions) ?? new List<LicenseDto>();
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
        public async Task<int> GetLicenseByPersonIdAsync(int PersonId, int LicenseClassId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/License/ActiveLicense/{PersonId}/{LicenseClassId}");

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

        public async Task<int> DeactivateLicenseAsync(int Id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/License/Deactivate/{Id}");

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

        public async Task<GetLicenseByPersonIdAndLicenseClass> GetLicenseByApplicationIdAsync(int Id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/License/GetPersonIdAndLicenseClass/{Id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<GetLicenseByPersonIdAndLicenseClass>(_jsonOptions);
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
        public async Task<GetImagePath> GetImagePathByApplicationID(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/License/ImagePath/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<GetImagePath>(_jsonOptions);
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

        public async Task<clsLicense> AddLicenseAsync(clsLicense License)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/License/AddLicense", License, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<clsLicense>(_jsonOptions);
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ من السيرفر: {error}");
            }
            catch (Exception ex)
            {
                throw new Exception($"فشلت عملية الإضافة: {ex.Message}");
            }
        }
        public async Task<clsLicense> UpdateLicense(int id, clsLicense License)
        {
            try
            {
        
                var response = await _httpClient.PutAsJsonAsync($"api/License/UpdateLicense/{id}", License, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<clsLicense>(_jsonOptions);
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ من السيرفر: {error}");
            }
            catch (Exception ex)
            {
        
                throw new Exception($"فشلت عملية التحديث: {ex.Message}");
            }
        }
        public async Task<string> DeleteLicenseAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/License/DeleteLicense/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return "Deleted Successfully";
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return $"This License is not found : {id}";

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ {response.StatusCode}: {error}");
            }
            catch (HttpRequestException)
            {
                throw new Exception("تعذر الاتصال بالسيرفر. تأكد من تشغيل الـ API.");
            }
        }

        public enum enIssueReason { FirstTime = 1, Renew = 2, DamagedReplacement = 3, LostReplacement = 4 };

        public class LicenseDto
        {
            public int LicenseID { get; set; }
            public int ApplicationID { get; set; }
            public int DriverID { get; set; }
            public int LicenseClass { get; set; }
            public DateTime IssueDate { get; set; }
            public DateTime ExpirationDate { get; set; }
            public string Notes { get; set; }
            public float PaidFees { get; set; }
            public byte IsActive { get; set; }
            public int IssueReason { get; set; }
            public int CreatedByUserID { get; set; }
        }

        public class clsLicense
        {
            public int LicenseID { get; set; }
            public int ApplicationID { get; set; }
            public int DriverID { get; set; }
            public int LicenseClass { get; set; }
            public DateTime IssueDate { get; set; }
            public DateTime ExpirationDate { get; set; }
            public string Notes { get; set; }
            public float PaidFees { get; set; }
            public byte IsActive { get; set; }
            public int IssueReason { get; set; }
            public int CreatedByUserID { get; set; }

        }
        public class GetImagePath
        {
            public string ImagePath { get; set; }
            public string Gender { get; set; }
        }
        public class GetLicenseByPersonIdAndLicenseClass
        {
            public int ID { get; set; }
            public int LicenseClassID { get; set; }
        }
    }
}


