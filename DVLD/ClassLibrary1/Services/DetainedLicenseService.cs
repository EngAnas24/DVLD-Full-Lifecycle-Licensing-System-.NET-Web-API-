using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace ClassLibrary1.Services
{
    public class DetainedLicenseService
    {

            private readonly HttpClient _httpClient;
            private readonly JsonSerializerOptions _jsonOptions;
            public DetainedLicenseService(HttpClient httpClient)
            {
                _httpClient = httpClient;

                _jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
            }

            public async Task<List<DetainedLicenseDto>> GetDetainedLicensesAsync()
            {
                try
                {
                    var response = await _httpClient.GetAsync("api/DetainedLicense/all");
                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadFromJsonAsync<List<DetainedLicenseDto>>(_jsonOptions) ?? new List<DetainedLicenseDto>();
                    }

                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"خطأ {response.StatusCode}: {error}");
                }
                catch (HttpRequestException)
                {
                    throw new Exception("تعذر الاتصال بالسيرفر. تأكد من تشغيل الـ API.");
                }
            }

            public async Task<DetainedLicenseDto> GetDetainedLicenseByLicenseIDAsync(int LicenseID)
            {
                try
                {

                    var response = await _httpClient.GetAsync($"api/DetainedLicense/{LicenseID}");

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadFromJsonAsync<DetainedLicenseDto>(_jsonOptions);
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
            public async Task<DetainedLicenseDto> GetDetainedLicenseByPersonIDAsync(int PersonID)
            {
                try
                {

                    var response = await _httpClient.GetAsync($"api/DetainedLicense/ByPersonID/{PersonID}");

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadFromJsonAsync<DetainedLicenseDto>(_jsonOptions);
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

            public async Task<DetainedLicense> AddDetainedLicenseAsync(DetainedLicense DetainedLicense)
            {
                try
                {
                    var response = await _httpClient.PostAsJsonAsync("api/DetainedLicense/AddDetainedLicense", DetainedLicense, _jsonOptions);

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadFromJsonAsync<DetainedLicense>(_jsonOptions);
                    }

                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"خطأ من السيرفر: {error}");
                }
                catch (Exception ex)
                {
                    throw new Exception($"فشلت عملية الإضافة: {ex.Message}");
                }
            }
            public async Task<DetainedLicense> UpdateDetainedLicense(int DetainedLicenseID, DetainedLicense DetainedLicense)
            {
                try
                {

                    var response = await _httpClient.PutAsJsonAsync($"api/DetainedLicense/Release/{DetainedLicenseID}", DetainedLicense, _jsonOptions);

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadFromJsonAsync<DetainedLicense>(_jsonOptions);
                    }

                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"خطأ من السيرفر: {error}");
                }
                catch (Exception ex)
                {
                    throw new Exception($"فشلت عملية التحديث: {ex.Message}");
                }
            }
            public async Task<string> DeleteDetainedLicenseAsync(int DetainedLicenseID)
            {
                try
                {
                    var response = await _httpClient.DeleteAsync($"api/DetainedLicense/DeleteDetainedLicense/{DetainedLicenseID}");

                    if (response.IsSuccessStatusCode)
                    {
                        return "Deleted Successfully";
                    }

                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        return $"This DetainedLicense is not found : {DetainedLicenseID}";

                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"خطأ {response.StatusCode}: {error}");
                }
                catch (HttpRequestException)
                {
                    throw new Exception("تعذر الاتصال بالسيرفر. تأكد من تشغيل الـ API.");
                }
            }
        public async Task<bool> IsDetainedLicenseAsync(int LicenseID)
        {
            try
            {
                // تم التغيير هنا من DeleteAsync إلى GetAsync لتتوافق مع الـ Backend
                var response = await _httpClient.GetAsync($"api/DetainedLicense/IsDetained/{LicenseID}");

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return false;

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ {response.StatusCode}: {error}");
            }
            catch (HttpRequestException)
            {
                throw new Exception("تعذر الاتصال بالسيرفر. تأكد من تشغيل الـ API.");
            }
        }

        public async Task<ReleaseTransactionParams> ReleaseLicense(ReleaseTransactionParams releaseLicense)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/DetainedLicense/release", releaseLicense, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ReleaseTransactionParams>(_jsonOptions);
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ من السيرفر: {error}");
            }
            catch (Exception ex)
            {
                throw new Exception($"فشلت عملية الإضافة: {ex.Message}");
            }
        }
        public async Task<int> FindPersonByLicenseId(int licenseId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/DetainedLicense/{licenseId}");

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<GetPerson>();
                    return result?.PersonalID ?? 0;
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
    }
    public class DetainedLicenseDto
        {
            public int DetainID { get; set; }
            public int LicenseID { get; set; }
            public DateTime DetainDate { get; set; }
            public bool IsReleased { get; set; }
            public decimal FineFees { get; set; }
            public DateTime? ReleaseDate { get; set; }
            public string NationalNo { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public int? ReleaseApplicationID { get; set; }
            public int? PersonalID { get; set; }
        }

        public class DetainedLicense
        {
            public int DetainID { get; set; }
            public int LicenseID { get; set; }
            public DateTime DetainDate { get; set; }
            public bool IsReleased { get; set; }
            public decimal FineFees { get; set; }
            public DateTime? ReleaseDate { get; set; }
            public int ReleaseApplicationID { get; set; } = 0;
            public int ReleasedByUserID { get; set; } = 0;
            public int? CreatedByUserID { get; set; }
        }
    public class ReleaseTransactionParams
    {
        public int ApplicationID { get; set; }
        public int ApplicantPersonID { get; set; }
        public DateTime ApplicationDate { get; set; }
        public int ApplicationTypeID { get; set; }
        public int ApplicationStatus { get; set; }
        public DateTime LastStatusDate { get; set; }
        public decimal PaidFees { get; set; }
        public int CreatedByUserID { get; set; } 

        public int DetainID { get; set; }
        public int LicenseID { get; set; }
        public DateTime DetainDate { get; set; }
        public bool IsReleased { get; set; }
        public decimal FineFees { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public int ReleaseApplicationID { get; set; }
        public int ReleasedByUserID { get; set; } 

 
    }
    class GetPerson
    {
        public int? PersonalID { get; set; }

    }

}
