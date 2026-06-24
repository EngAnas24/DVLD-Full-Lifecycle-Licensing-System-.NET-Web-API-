using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace DVLDServices.Services
{
    public class PeopleService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public PeopleService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<List<PeopleDto>> GetPeopleAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/People/all");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<PeopleDto>>(_jsonOptions) ?? new List<PeopleDto>();
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ {response.StatusCode}: {error}");
            }
            catch (HttpRequestException)
            {
                throw new Exception("تعذر الاتصال بالسيرفر. تأكد من تشغيل الـ API.");
            }
        }

        public async Task<PeopleDto> GetPersonByIdAsync(int id)
        {
            try
            {

                var response = await _httpClient.GetAsync($"api/People/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<PeopleDto>(_jsonOptions);
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
        public async Task<People> AddPersonAsync(People person)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/People/AddPerson", person, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<People>(_jsonOptions);
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ من السيرفر: {error}");
            }
            catch (Exception ex)
            {
                throw new Exception($"فشلت عملية الإضافة: {ex.Message}");
            }
        }
        public async Task<People> UpdatePerson(int id, People people)
        {
            try
            {

                var response = await _httpClient.PutAsJsonAsync($"api/People/UpdatePerson/{id}", people, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<People>(_jsonOptions);
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ من السيرفر: {error}");
            }
            catch (Exception ex)
            {

                throw new Exception($"فشلت عملية التحديث: {ex.Message}");
            }
        }
        public async Task<string> DeletePersonAsync(int id)
        {
            try
            {

                var response =await DeletePersonHttpResponseAsync(id);

                if (response.IsSuccessStatusCode)
                {
                    return "Deleted Successfully";
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return $"This person is not found : {id}";

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ {response.StatusCode}: {error}");
            }
            catch (HttpRequestException)
            {
                throw new Exception("تعذر الاتصال بالسيرفر. تأكد من تشغيل الـ API.");
            }
        }
        public async Task<HttpResponseMessage> DeletePersonHttpResponseAsync(int id)
        {
            return await _httpClient.DeleteAsync($"api/People/DeletePerson/{id}");
        }
        public async Task<bool> UploadPersonImage(int id, string localFilePath)
        {
            using (var content = new MultipartFormDataContent())
            {
                var fileStream = new FileStream(localFilePath, FileMode.Open);
                var fileContent = new StreamContent(fileStream);

                content.Add(fileContent, "file", Path.GetFileName(localFilePath));

                var response = await _httpClient.PostAsync($"api/People/upload-image/{id}", content);
                return response.IsSuccessStatusCode;
            }
        }
        public async Task<bool> DeleteImagePersonAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/people/delete-image/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return false;
                }

                var errorContent = await response.Content.ReadAsStringAsync();

                throw new Exception($"سيرفر الـ API أعاد خطأ: {errorContent}");
            }
            catch (HttpRequestException ex)
            {
                throw new Exception("فشل الاتصال بالشبكة أو السيرفر غير متاح. تفاصيل: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("حدث خطأ غير متوقع: " + ex.Message);
            }
        }

        public async Task<List<Country>> GetCountriesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/People/allCountries");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<Country>>(_jsonOptions) ?? new List<Country>();
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

        public class People
        {
            public int PersonalID { get; set; }
            public int NationalNo { get; set; }
            public string FirstName { get; set; }
            public string SecondName { get; set; }
            public string ThirdName { get; set; }
            public string LastName { get; set; }
            public DateTime DateOfBirth { get; set; }
            public string Gender { get; set; }
            public string Address { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public int NationalityCountryID { get; set; }
            public string ImagePath { get; set; }
        }

        public class PeopleDto
        {
            public int PersonalID { get; set; }
            public int NationalNo { get; set; }
            public string FullName { get; set; }
            public string FirstName { get; set; }
            public string SecondName { get; set; }
            public string ThirdName { get; set; }
            public string LastName { get; set; }
            public DateTime DateOfBirth { get; set; }
            public string GenderName { get; set; }   
            public string Gender { get; set; }  
            public string Address { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public int NationalityCountryID { get; set; }
            public string CountryName { get; set; }  
            public string ImagePath { get; set; }

        }

        public class Country
        {
            public int CountryID { get; set; }
            public string CountryName { get; set; }
        }
    }
}
