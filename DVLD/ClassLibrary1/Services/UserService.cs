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
    public class UserService
    {

        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        public User user;
        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<List<UserDto>> GetUserAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/User/GetAllUsers");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<UserDto>>(_jsonOptions) ?? new List<UserDto>();
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ {response.StatusCode}: {error}");
            }
            catch (HttpRequestException)
            {
                throw new Exception("تعذر الاتصال بالسيرفر. تأكد من تشغيل الـ API.");
            }
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            try
            {
     
                var response = await _httpClient.GetAsync($"api/User/GetUserById/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserDto>(_jsonOptions);
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

        public async Task<UserDto> GetUserByNameAndPasswordAsync(string name, string password)
        {
            try
            {
                var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(name), "UserName");
                formData.Add(new StringContent(password), "Password");

                var response = await _httpClient.PostAsync("api/User/Login", formData);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<UserDto>(_jsonOptions);
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
        public async Task<User> AddUserAsync(User User)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/User/AddUser", User, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<User>(_jsonOptions);
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ من السيرفر: {error}");
            }
            catch (Exception ex)
            {
                throw new Exception($"فشلت عملية الإضافة: {ex.Message}");
            }
        }
        public async Task<User> UpdateUser(int id, User User)
        {
            try
            {

                var response = await _httpClient.PutAsJsonAsync($"api/User/UpdateUser/{id}", User, _jsonOptions);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<User>(_jsonOptions);
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ من السيرفر: {error}");
            }
            catch (Exception ex)
            {
                throw new Exception($"فشلت عملية التحديث: {ex.Message}");
            }
        }
        public async Task<string> DeleteUserAsync(int id)
        {
            try
            {
                var response = await DeleteUserHttpResponseAsync(id);

                if (response.IsSuccessStatusCode)
                {
                    return "Deleted Successfully";
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return $"This User is not found : {id}";

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"خطأ {response.StatusCode}: {error}");
            }
            catch (HttpRequestException)
            {
                throw new Exception("تعذر الاتصال بالسيرفر. تأكد من تشغيل الـ API.");
            }
        }
        public async Task<HttpResponseMessage> DeleteUserHttpResponseAsync(int id)
        {
            return await _httpClient.DeleteAsync($"api/User/DeleteUser/{id}");
        }
        public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            try
            {
                var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(oldPassword), "OldPassword");
                formData.Add(new StringContent(newPassword), "NewPassword");

                var response = await _httpClient.PostAsync($"api/User/ChangePassword/{userId}", formData);

                if (response.IsSuccessStatusCode)
                {
                    return true; 
                }

                var error = await response.Content.ReadAsStringAsync();
                throw new Exception(error);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public class User
        {
            public int ID { get; set; }
            public int PersonalID { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public bool IsActive { set; get; }
        }

        public class UserDto
        {
            public int ID { get; set; }
            public int PersonalID { get; set; }
            public string Password { get; set; }
            public string UserName { get; set; }
            public bool IsActive { set; get; }
        }

    }
}
