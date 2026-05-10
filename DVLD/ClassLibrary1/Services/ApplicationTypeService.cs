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
    public class ApplicationTypeService
    {

        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        public ApplicationType applicationType;
        public EnApplicationType enApplicationType;
        public ApplicationTypeService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<List<ApplicationTypeDto>> GetApplicationTypesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/ApplicationType/all");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<ApplicationTypeDto>>(_jsonOptions) ?? new List<ApplicationTypeDto>();
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ {response.StatusCode}: {error}");
            }
            catch (HttpRequestException)
            {
                throw new Exception("تعذر الاتصال بالسيرفر. تأكد من تشغيل الـ API.");
            }
        }

        public async Task<ApplicationTypeDto> GetApplicationTypeByIdAsync(int id)
        {
            try
            {

                var response = await _httpClient.GetAsync($"api/ApplicationType/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ApplicationTypeDto>(_jsonOptions);
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

        public async Task<ApplicationType> AddApplicationTypeAsync(ApplicationType ApplicationType)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/ApplicationType/AddApplicationType", ApplicationType, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ApplicationType>(_jsonOptions);
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ من السيرفر: {error}");
            }
            catch (Exception ex)
            {
                throw new Exception($"فشلت عملية الإضافة: {ex.Message}");
            }
        }
        public async Task<ApplicationType> UpdateApplicationType(int id, ApplicationType ApplicationType)
        {
            try
            {
       
                var response = await _httpClient.PutAsJsonAsync($"api/ApplicationType/UpdateApplicationType/{id}", ApplicationType, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ApplicationType>(_jsonOptions);
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ من السيرفر: {error}");
            }
            catch (Exception ex)
            {
 
                throw new Exception($"فشلت عملية التحديث: {ex.Message}");
            }
        }
        public async Task<string> DeleteApplicationTypeAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/ApplicationType/DeleteApplicationType/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return "Deleted Successfully";
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return $"This ApplicationType is not found : {id}";

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ {response.StatusCode}: {error}");
            }
            catch (HttpRequestException)
            {
                throw new Exception("تعذر الاتصال بالسيرفر. تأكد من تشغيل الـ API.");
            }
        }

        public enum EnApplicationType
        {
            NewLocalDrivingLicenseService = 1,
            RenewDrivingLicenseService = 2,
            ReplacementForALostDrivingLicense = 3,
            ReplacementForADamagedDrivingLicense = 4,
            ReleaseDetainedDrivingLicense = 5,
            NewInternationalLicense = 6,
            RetakeTest = 7
        }
        public class ApplicationTypeDto
        {
            public int ApplicationTypeID { get; set; }
            public string ApplicationTypeTitle { get; set; }
            public decimal ApplicationFees { get; set; }
        }


        public class ApplicationType
        {
            public int ApplicationTypeID { get; set; }
            public string ApplicationTypeTitle { get; set; }
            public decimal ApplicationFees { get; set; }
        }


    }
}


