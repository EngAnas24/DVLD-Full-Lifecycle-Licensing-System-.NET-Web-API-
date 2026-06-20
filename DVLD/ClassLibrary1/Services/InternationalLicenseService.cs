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
    public class InternationalLicenseService
    {

        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        public InternationalLicense _InternationalLicense;
        public InternationalLicenseService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<List<InternationalLicenseDto>> GetInternationalLicensesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/InternationalLicense/all");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<InternationalLicenseDto>>(_jsonOptions)
                           ?? new List<InternationalLicenseDto>();
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new List<InternationalLicenseDto>();
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ {response.StatusCode}: {error}");
            }
            catch (HttpRequestException)
            {
                throw new Exception("تعذر الاتصال بالسيرفر. تأكد من تشغيل الـ API.");
            }
        }
        public async Task<InternationalLicenseDto> GetInternationalLicenseByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/InternationalLicense/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<InternationalLicenseDto>(_jsonOptions);
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
        public async Task<InternationalLicenseDto> GetActiveInternationalLicenseByDriverID(int DriverID)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/InternationalLicense/ActiveInternationalLicense/{DriverID}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<InternationalLicenseDto>(_jsonOptions);
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
        public async Task<List<InternationalLicenseDto>> GetActiveInternationalLicensesByDriverID(int DriverID)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/InternationalLicense/ActiveInternationalLicenses/{DriverID}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<InternationalLicenseDto>>(_jsonOptions) ?? new List<InternationalLicenseDto>();
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
                var response = await _httpClient.GetAsync($"api/InternationalLicense/InternationalImagePath/{id}");

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

        public async Task<InternationalLicense> AddInternationalLicenseAsync(InternationalLicense InternationalLicense)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/InternationalLicense/AddInternationalLicense", InternationalLicense, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<InternationalLicense>(_jsonOptions);
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ من السيرفر: {error}");
            }
            catch (Exception ex)
            {
                throw new Exception($"فشلت عملية الإضافة: {ex.Message}");
            }
        }
        public async Task<InternationalLicense> UpdateInternationalLicense(int id,InternationalLicense InternationalLicense)
        {
            try
            {
        
                var response = await _httpClient.PutAsJsonAsync($"api/InternationalLicense/UpdateInternationalLicense/{id}", InternationalLicense, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<InternationalLicense>(_jsonOptions);
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ من السيرفر: {error}");
            }
            catch (Exception ex)
            {
        
                throw new Exception($"فشلت عملية التحديث: {ex.Message}");
            }
        }
        public async Task<string> DeleteInternationalLicenseAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/InternationalLicense/DeleteInternationalLicense/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return "Deleted Successfully";
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return $"This InternationalLicense is not found : {id}";

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ {response.StatusCode}: {error}");
            }
            catch (HttpRequestException)
            {
                throw new Exception("تعذر الاتصال بالسيرفر. تأكد من تشغيل الـ API.");
            }
        }


        public class InternationalLicenseDto
        {
            public int InternationalLicenseID { get; set; }
            public int ApplicationID { get; set; }
            public int DriverID { get; set; }
            public int IssuedUsingLocalLicenseID { get; set; }
            public DateTime IssueDate { get; set; }
            public DateTime ExpirationDate { get; set; }
            public bool IsActive { get; set; }
            public int CreatedByUserID { get; set; }
        }

        public class InternationalLicense
        {
            public int InternationalLicenseID { get; set; }
            public int ApplicationID { get; set; }
            public int DriverID { get; set; }
            public int IssuedUsingLocalLicenseID { get; set; }
            public DateTime IssueDate { get; set; }
            public DateTime ExpirationDate { get; set; }
            public bool IsActive { get; set; }
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


