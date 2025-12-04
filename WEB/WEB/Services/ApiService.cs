using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace WEB.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _apiBaseUrl;

        public ApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5287/api";

            var normalizedBase = _apiBaseUrl.TrimEnd('/') + '/';
            if (_httpClient.BaseAddress == null || !string.Equals(_httpClient.BaseAddress.ToString(), normalizedBase, StringComparison.OrdinalIgnoreCase))
            {
                _httpClient.BaseAddress = new Uri(normalizedBase);
            }
        }

        private void AddAuthorizationHeader()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JWTToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync(endpoint);
            
            if (!response.IsSuccessStatusCode)
                return default;

            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        public async Task<(bool Success, T? Data, string? Error)> PostAsync<T>(string endpoint, object data)
        {
            try
            {
                AddAuthorizationHeader();
                
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var fullUrl = new Uri(_httpClient.BaseAddress!, endpoint).ToString();
                System.Diagnostics.Debug.WriteLine($"POST Request to: {_httpClient.BaseAddress}");
                System.Diagnostics.Debug.WriteLine($"POST Request to: {fullUrl}");
                System.Diagnostics.Debug.WriteLine($"Request Body: {json}");
                
                var response = await _httpClient.PostAsync(endpoint, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    System.Diagnostics.Debug.WriteLine($"Attempting to deserialize to type: {typeof(T).Name}");
                    var result = JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    System.Diagnostics.Debug.WriteLine($"Deserialization result is null: {result == null}");
                    if (result != null)
                    {
                        System.Diagnostics.Debug.WriteLine($"Deserialized object: {JsonSerializer.Serialize(result)}");
                    }
                    return (true, result, null);
                }

                System.Diagnostics.Debug.WriteLine($"Request failed with status code: {response.StatusCode}");
                return (false, default, responseContent);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in PostAsync: {ex.Message}");
                return (false, default, $"Exception: {ex.Message}");
            }
        }

        public async Task<(bool Success, T? Data, string? Error)> PutAsync<T>(string endpoint, object data)
        {
            AddAuthorizationHeader();
            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync(endpoint, content);
            
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    return (true, default, null);

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<T>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return (true, result, null);
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            return (false, default, errorContent);
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(string endpoint)
        {
            AddAuthorizationHeader();
            var response = await _httpClient.DeleteAsync(endpoint);
            
            if (response.IsSuccessStatusCode)
                return (true, null);

            var errorContent = await response.Content.ReadAsStringAsync();
            return (false, errorContent);
        }
    }
}
