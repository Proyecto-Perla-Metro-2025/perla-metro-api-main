using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ApiMain.src.Dto;
using ApiMain.src.Interfaces;
using ApiMain.src.models;
using Microsoft.AspNetCore.DataProtection.KeyManagement.Internal;
using Microsoft.Extensions.Caching.Memory;
using ApiMain.src.DTOs;
using ApiMain.src.Helper;

namespace ApiMain.src.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private readonly string _userApiBaseUrl;

        public AuthService(HttpClient httpClient, ITokenService tokenService, IConfiguration configuration, IMemoryCache cache)
        {
            _httpClient = httpClient;
            _tokenService = tokenService;
            _configuration = configuration;
            _cache = cache;
            _userApiBaseUrl = _configuration["UserAPI:BaseUrl"];
        }

        public async Task<string> GetServiceTokenAsync()
        {
            const string cacheKey = "service_token";

            if (_cache.TryGetValue(cacheKey, out string cachedToken))
            {
                return cachedToken;
            }

            return "no token";
        }
        public async Task<string> GetCurrentUserId()
        {
            const string cacheKey = "UserId";

            if (_cache.TryGetValue(cacheKey, out string id))
            {
                return id;
            }

            return "no user";
        }
        public async Task<UserDto> UpdateUser(UpdateUserDto updateUserDto)
        {

            string token = await GetServiceTokenAsync();

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var httpContent = RequestBuilder.BuildUserUpdateRequest(updateUserDto);

            var response = await _httpClient.PutAsync($"{_userApiBaseUrl}api/User/update-user", httpContent);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Update failed: {error}");
            }

            return await response.Content.ReadFromJsonAsync<UserDto>();
        }
        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                const string cacheKey = "service_token";
                const string cacheU = "UserId";

                // Call User API to validate credentials
                var validationRequest = new UserValidationRequest
                {
                    Email = request.Email,
                    Password = request.Password
                };

                var response = await _httpClient.PostAsJsonAsync(
                    $"{_userApiBaseUrl}api/User/login",
                    validationRequest);

                if (!response.IsSuccessStatusCode)
                {
                    throw new UnauthorizedAccessException("Authentication failed");
                }

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                jsonOptions.Converters.Add(new ClaimJsonConverter());

                var validationResult = await response.Content.ReadFromJsonAsync<UserValidationResponse>(jsonOptions);

                if (validationResult == null || !validationResult.IsValid)
                {
                    throw new UnauthorizedAccessException("Invalid credentials");
                }
                // Convert DTO claims into real Claim objects
                var claims = validationResult.Claims
                    .Select(c => new Claim(c.Type, c.Value))
                    .ToList();

                // Generate JWT token using the claims
                var jwtToken = _tokenService.GenerateJwtToken(claims);

                // Cache for 50 minutes (tokens expire in 60 minutes)
                _cache.Set(cacheKey, jwtToken, TimeSpan.FromMinutes(50));
                _cache.Set(cacheU, validationResult.Id, TimeSpan.FromMinutes(50));
                return new LoginResponse

                {
                    Token = jwtToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60), // Should match JWT expiration
                    User = new UserInfo
                    {
                        Id = validationResult.Id,
                        Email = validationResult.Email,
                        Role = validationResult.Role,
                        FullName = validationResult.Claims
                            .FirstOrDefault(c => c.Type == "fullName")?.Value ?? ""
                    }
                };
            }
            catch (HttpRequestException)
            {
                throw new ServiceUnavailableException("User service is currently unavailable");
            }
        }

        public async Task<VisualizeUserDto> getUser(string id)
        {
            var response = await _httpClient.GetAsync(
                    $"{_userApiBaseUrl}api/User/GetUser?Id={id}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<VisualizeUserDto>();
            }

            return null;

        }
        public async Task<List<VisualizeUserDto>?> getAll()
        {

            string token = await GetServiceTokenAsync();

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync(
                    $"{_userApiBaseUrl}api/User/GetAll");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<VisualizeUserDto>>();
            }
            return null;
        }

        public Task<TokenValidationResponse> ValidateTokenAsync(string token)
        {
            try
            {
                var principal = _tokenService.ValidateToken(token);

                if (principal == null)
                {
                    return Task.FromResult(new TokenValidationResponse
                    {
                        IsValid = false,
                        ErrorMessage = "Invalid or expired token"
                    });
                }

                return Task.FromResult(new TokenValidationResponse
                {
                    IsValid = true,
                    Claims = principal.Claims.ToList()
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new TokenValidationResponse
                {
                    IsValid = false,
                    ErrorMessage = $"Token validation error: {ex.Message}"
                });
            }
        }

        public async Task EnableDisableUser(string id)
        {
            string token = await GetServiceTokenAsync();

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PutAsync($"{_userApiBaseUrl}/api/User/enable-disable/{id}", null);
    
            // Handle response properly
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException("Unauthorized to perform this action");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
                {
                    throw new ServiceUnavailableException("User service is currently unavailable");
                }
                else
                {
                    throw new HttpRequestException($"Failed to enable/disable user: {response.StatusCode} - {errorContent}");
                }
            };
        }

        public async Task<UserDto> RegisterUser(CreateUserDto createUserDto)
        {
            try
            {

                // Call User API register a new user

                var response = await _httpClient.PostAsJsonAsync($"{_userApiBaseUrl}api/User/Register", createUserDto);

                if (!response.IsSuccessStatusCode)
                {
                    throw new UnauthorizedAccessException("Register failed");
                }
                return await response.Content.ReadFromJsonAsync<UserDto>();
            }

            catch (HttpRequestException)
            {
                throw new ServiceUnavailableException("User service is currently unavailable");
            }
        }

        public async Task<List<VisualizeUserDto>> GetUsersFilter(QueryObject queryObject)
        {
            string token = await GetServiceTokenAsync();

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var queryParams = new List<string>();

            if (!string.IsNullOrEmpty(queryObject.Name))
                queryParams.Add($"Name={Uri.EscapeDataString(queryObject.Name)}");

            if (!string.IsNullOrEmpty(queryObject.Email))
                queryParams.Add($"Email={Uri.EscapeDataString(queryObject.Email)}");

            if (queryObject.isActive.HasValue)
                queryParams.Add($"isActive={queryObject.isActive.Value.ToString().ToLower()}");

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";

            var response = await _httpClient.GetAsync($"{_userApiBaseUrl}api/User/UserFilter{queryString}");

            return await response.Content.ReadFromJsonAsync<List<VisualizeUserDto>>();
        }


    }

    
    public class ServiceUnavailableException : Exception
    {
        public ServiceUnavailableException(string message) : base(message) { }
    }

    public static class RequestBuilder
    {
        public static StringContent BuildUserUpdateRequest(UpdateUserDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, // skip null fields
                WriteIndented = true // optional, makes JSON readable
            };

            // Convert empty strings to null, so they are ignored
            var dtoForJson = new
            {
                email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email,
                password = string.IsNullOrWhiteSpace(dto.Password) ? null : dto.Password,
                name = string.IsNullOrWhiteSpace(dto.Name) ? null : dto.Name,
                surename = string.IsNullOrWhiteSpace(dto.Surename) ? null : dto.Surename

            };

            string json = JsonSerializer.Serialize(dtoForJson, options);

            return new StringContent(json, Encoding.UTF8, "application/json");
        }
        public static StringContent BuildUserQueryRequest(QueryObject queryObject)
        {
            if (queryObject == null)
                throw new ArgumentNullException(nameof(queryObject));

            var options = new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, // skip null fields
                WriteIndented = true // optional, makes JSON readable
            };

            // Convert empty strings to null, so they are ignored
            var queryObjectForJson = new
            {
                email = string.IsNullOrWhiteSpace(queryObject.Email) ? null : queryObject.Email,
                name = string.IsNullOrWhiteSpace(queryObject.Name) ? null : queryObject.Name,
                isActive = queryObject.isActive.HasValue ? null : queryObject.isActive

            };



            string json = JsonSerializer.Serialize(queryObjectForJson, options);

            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}
