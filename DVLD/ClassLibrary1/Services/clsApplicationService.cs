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
using DVLDServices.GlobalClasses;

namespace DVLDServices.Services
{
    public class clsApplicationService
    {

        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private clsApplication _Application;
        public clsApplicationService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<List<clsApplicationDto>> GetApplicationsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Application/all");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<clsApplicationDto>>(_jsonOptions) ?? new List<clsApplicationDto>();
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ {response.StatusCode}: {error}");
            }
            catch (HttpRequestException)
            {
                throw new Exception("تعذر الاتصال بالسيرفر. تأكد من تشغيل الـ API.");
            }
        }

        public async Task<clsApplicationDto> GetApplicationByIdAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Application/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<clsApplicationDto>(_jsonOptions);

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

        public async Task<int> ChangeStatusToCompleted(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Application/ChangeStatus/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    if (int.TryParse(content, out int result))
                    {
                        return result;
                    }

                    return 0;
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

        public async Task<clsApplication> AddApplicationAsync(clsApplication Application)
        {
            try
            {
        
                var response = await _httpClient.PostAsJsonAsync("api/Application/AddApplication", Application, _jsonOptions);
                int Id = clsGlobal.GetUser.ID;
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<clsApplication>(_jsonOptions);
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ من السيرفر: {error}");
            }
            catch (Exception ex)
            {
                throw new Exception($"فشلت عملية الإضافة: {ex.Message}");
            }
        }
        public async Task<clsApplication> UpdateApplication(int id, clsApplication clsApplication)
        {
            try
            {
     
                var response = await _httpClient.PutAsJsonAsync($"api/Application/UpdateApplication/{id}", clsApplication, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<clsApplication>(_jsonOptions);
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ من السيرفر: {error}");
            }
            catch (Exception ex)
            {
                throw new Exception($"فشلت عملية التحديث: {ex.Message}");
            }
        }
        public async Task<string> DeleteApplicationAsync(int id)
        {
            try
            {
                var response = await DeleteApplicationHttpResponseAsync(id);

                if (response.IsSuccessStatusCode)
                {
                    return "Deleted Successfully";
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return $"This clsApplication is not found : {id}";

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ {response.StatusCode}: {error}");
            }
            catch (HttpRequestException)
            {
                throw new Exception("تعذر الاتصال بالسيرفر. تأكد من تشغيل الـ API.");
            }
        }
        public async Task<HttpResponseMessage> DeleteApplicationHttpResponseAsync(int id)
        {
            return await _httpClient.DeleteAsync($"api/Application/DeleteApplication/{id}");
        }
        public enum EnApplicationStatus
        {
            New = 1,
            Cancelled = 2,
            Completed = 3
        }
        public class clsApplicationDto
        {
            public int ApplicationID { get; set; }
            public int ApplicantPersonID { get; set; }
            public string ApplicantName { get; set; }
            public DateTime ApplicationDate { get; set; }
            public int ApplicationTypeID { get; set; }
            public int ApplicationStatus { get; set; }
            public DateTime LastStatusDate { get; set; }
            public decimal PaidFees { get; set; }
            public int CreatedByUserID { get; set; }
        }


        public class clsApplication
        {
            public int ApplicationID { get; set; }
            public int ApplicantPersonID { get; set; }
            public DateTime ApplicationDate { get; set; }
            public int ApplicationTypeID { get; set; }
            public int ApplicationStatus { get; set; }
            public DateTime LastStatusDate { get; set; }
            public decimal PaidFees { get; set; }
            public int CreatedByUserID { get; set; }
        }


    }
}


