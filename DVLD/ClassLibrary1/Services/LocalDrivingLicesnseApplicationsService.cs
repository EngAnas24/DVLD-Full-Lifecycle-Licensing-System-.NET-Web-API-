using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using static DVLDServices.Services.LocalDrivingLicesnseApplicationsService;
using System.Net.Http.Json;

namespace DVLDServices.Services
{
    public class LocalDrivingLicesnseApplicationsService
    {

        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private LocalDrivingLicenseApplications _LocalApp;
        public LocalDrivingLicesnseApplicationsService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<List<LocalDrivingLicenseApplicationsDto>> GetLocalApplicationsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/LocalDrivingLicenseApplication/GetLocalDrivingLicenseApplications");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<LocalDrivingLicenseApplicationsDto>>(_jsonOptions) ?? new List<LocalDrivingLicenseApplicationsDto>();
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ {response.StatusCode}: {error}");
            }
            catch (HttpRequestException)
            {
                throw new Exception("تعذر الاتصال بالسيرفر. تأكد من تشغيل الـ API.");
            }
        }

        public async Task<LocalDrivingLicenseApplicationsDto> GetLocalApplicationByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/LocalDrivingLicenseApplication/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<LocalDrivingLicenseApplicationsDto>(_jsonOptions);
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
        public async Task<LocalDrivingLicenseApplicationsDto> GetLocalApplicationByApplicationId(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/LocalDrivingLicenseApplication/LocalApplication/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<LocalDrivingLicenseApplicationsDto>(_jsonOptions);
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
        public async Task<int> HasApplication(int personId,int licenseClassId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/LocalDrivingLicenseApplication/has/{personId}/{licenseClassId}");

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
        public async Task<int> HasAlreadyLicense(int personId, int licenseClassId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/LocalDrivingLicenseApplication/HasAlreadyLicense/{personId}/{licenseClassId}");

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
        public async Task<LocalDrivingLicenseApplications> InsertLocalApplication(LocalDrivingLicenseApplications LocalApp)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/LocalDrivingLicenseApplication/InsertLocalApplication", LocalApp, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<LocalDrivingLicenseApplications>(_jsonOptions);
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ من السيرفر: {error}");
            }
            catch (Exception ex)
            {
                throw new Exception($"فشلت عملية الإضافة: {ex.Message}");
            }
        }
        public async Task<LocalDrivingLicenseApplications> UpdateLocalApplication(int id, LocalDrivingLicenseApplications LocalApp)
        {
            try
            {
    
                var response = await _httpClient.PutAsJsonAsync($"api/LocalDrivingLicenseApplication/UpdateLocalApplication/{id}", LocalApp, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<LocalDrivingLicenseApplications>(_jsonOptions);
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ من السيرفر: {error}");
            }
            catch (Exception ex)
            {
                throw new Exception($"فشلت عملية التحديث: {ex.Message}");
            }
        }
        public async Task<string> DeleteLocalApplication(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/LocalDrivingLicenseApplication/DeleteLocalApplication/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return "Deleted Successfully";
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return $"This LocalDrivingLicenseApplication is not found : {id}";

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ {response.StatusCode}: {error}");
            }
            catch (HttpRequestException)
            {
                throw new Exception("تعذر الاتصال بالسيرفر. تأكد من تشغيل الـ API.");
            }
        }



      
        public class LocalDrivingLicenseApplications
        {
            public int LocalDrivingLicenseApplicationID { get; set; }
            public int ApplicationID { get; set; }
            public int LicenseClassID { get; set; }
        }

        public class LocalDrivingLicenseApplicationsDto
        {
            public int LocalDrivingLicenseApplicationID { get; set; }
            public int ApplicationID { get; set; }
            public string ClassName { get; set; } = string.Empty;
            public DateTime ApplicationDate { get; set; }
            public string NationalNo { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public int PassedTestCount { get; set; }
            public string Status { get; set; } = string.Empty;

        }

    }

}