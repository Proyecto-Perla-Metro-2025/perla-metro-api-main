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

        /// <summary>
        /// The function `GetServiceTokenAsync` retrieves a service token from cache if available,
        /// otherwise returns "no token".
        /// </summary>
        /// <returns>
        /// If the token is found in the cache, the cached token will be returned. Otherwise, "no token"
        /// will be returned.
        /// </returns>
        public async Task<string> GetServiceTokenAsync()
        {
            const string cacheKey = "service_token";

            if (_cache.TryGetValue(cacheKey, out string cachedToken))
            {
                return cachedToken;
            }

            return "no token";
        }
        /// <summary>
        /// The function GetCurrentUserId retrieves the current user's ID from cache or returns "no
        /// user" if not found.
        /// </summary>
        /// <returns>
        /// If the value is found in the cache, the user ID will be returned. Otherwise, the string "no
        /// user" will be returned.
        /// </returns>
        public async Task<string> GetCurrentUserId()
        {
            const string cacheKey = "UserId";

            if (_cache.TryGetValue(cacheKey, out string id))
            {
                return id;
            }

            return "no user";
        }
        /// <summary>
        /// This C# function updates a user by sending a PUT request to the user API with the provided
        /// user data and returns the updated user information.
        /// </summary>
        /// <param name="UpdateUserDto">The `UpdateUserDto` parameter in the `UpdateUser` method is a
        /// data transfer object (DTO) that contains the information needed to update a user. It likely
        /// includes properties such as the user's ID, name, email, or any other fields that can be
        /// updated for a user.</param>
        /// <returns>
        /// The method `UpdateUser` is returning a `Task<UserDto>`, which is an asynchronous operation
        /// that will eventually return a `UserDto` object.
        /// </returns>
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
        /// <summary>
        /// The `LoginAsync` function validates user credentials, generates a JWT token, caches it, and
        /// returns a `LoginResponse` object.
        /// </summary>
        /// <param name="LoginRequest">The `LoginAsync` method you provided is responsible for handling
        /// the login process. It takes a `LoginRequest` object as a parameter, which likely contains
        /// the user's email and password for authentication.</param>
        /// <returns>
        /// The `LoginAsync` method returns a `Task<LoginResponse>`. The `LoginResponse` object contains
        /// the following properties:
        /// - `Token`: A JWT token generated using the user's claims.
        /// - `ExpiresAt`: The expiration time of the JWT token (calculated as the current time plus 60
        /// minutes).
        /// - `User`: An object of type `UserInfo` containing the user's information
        /// </returns>
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

        /// <summary>
        /// This C# function asynchronously retrieves user data from an API based on the provided ID.
        /// </summary>
        /// <param name="id">The `id` parameter in the `getUser` method is used to specify the unique
        /// identifier of the user whose information you want to retrieve.</param>
        /// <returns>
        /// The `getUser` method returns a `Task` that will eventually contain a `VisualizeUserDto`
        /// object representing a user. If the HTTP response is successful (status code 200), the method
        /// will read the response content as a `VisualizeUserDto` object and return it. Otherwise, it
        /// will return `null`.
        /// </returns>
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
        /// <summary>
        /// This C# function asynchronously retrieves a list of VisualizeUserDto objects from a user API
        /// endpoint using a bearer token for authorization.
        /// </summary>
        /// <returns>
        /// A `Task` that will eventually contain a `List` of `VisualizeUserDto` objects, or `null` if
        /// the HTTP response is not successful.
        /// </returns>
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

        /// <summary>
        /// The function `ValidateTokenAsync` asynchronously validates a token and returns a
        /// `TokenValidationResponse` indicating whether the token is valid or not.
        /// </summary>
        /// <param name="token">The `ValidateTokenAsync` method takes a `token` as input parameter. This
        /// token is used to validate the user's authentication token. If the token is valid, the method
        /// returns a `TokenValidationResponse` indicating that the token is valid along with the user's
        /// claims. If the token is</param>
        /// <returns>
        /// The `ValidateTokenAsync` method returns a `Task` that will eventually contain a
        /// `TokenValidationResponse`. The `TokenValidationResponse` object contains information about
        /// whether the token is valid or not, along with any error messages if the validation process
        /// encounters an exception.
        /// </returns>
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

        /// <summary>
        /// The function `EnableDisableUser` asynchronously enables or disables a user by sending a PUT
        /// request to the user API with appropriate error handling.
        /// </summary>
        /// <param name="id">The `EnableDisableUser` method takes a `string id` parameter, which
        /// represents the unique identifier of the user whose enable/disable status needs to be
        /// updated. This method sends a PUT request to the user API endpoint with the user's ID to
        /// enable or disable the user based on the response from the</param>
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

        /// <summary>
        /// This C# async function registers a new user by calling a User API and handling potential
        /// exceptions.
        /// </summary>
        /// <param name="CreateUserDto">CreateUserDto is a data transfer object (DTO) that contains the
        /// information needed to create a new user. It typically includes properties such as username,
        /// email, password, and any other required user details.</param>
        /// <returns>
        /// The method `RegisterUser` returns a `Task<UserDto>`, which is an asynchronous operation that
        /// will eventually produce a `UserDto` object.
        /// </returns>
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

        /// <summary>
        /// This C# function asynchronously retrieves a list of users based on specified query
        /// parameters using an HTTP GET request.
        /// </summary>
        /// <param name="QueryObject">The `GetUsersFilter` method you provided is an asynchronous method
        /// that retrieves a list of users based on the provided `QueryObject` parameters. Here is a
        /// breakdown of the parameters used in the method:</param>
        /// <returns>
        /// The method `GetUsersFilter` returns a `Task` that will eventually contain a `List` of
        /// `VisualizeUserDto` objects.
        /// </returns>
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

    
    /* The class ServiceUnavailableException is a custom exception in C# that represents a service
    being unavailable. */
    public class ServiceUnavailableException : Exception
    {
        public ServiceUnavailableException(string message) : base(message) { }
    }

    public static class RequestBuilder
    {
        /// <summary>
        /// This C# function builds a JSON request for updating user information based on the provided
        /// data transfer object (DTO).
        /// </summary>
        /// <param name="UpdateUserDto">The `BuildUserUpdateRequest` method takes an `UpdateUserDto`
        /// object as a parameter. This object likely contains information about a user that needs to be
        /// updated, such as email, password, name, and surname.</param>
        /// <returns>
        /// The method `BuildUserUpdateRequest` returns a `StringContent` object containing the JSON
        /// representation of the `UpdateUserDto` object provided as input. The JSON is generated based
        /// on the properties of the `UpdateUserDto` object, with empty strings converted to `null`
        /// values to be ignored during serialization. The JSON is formatted with indentation for
        /// readability and the content type is set to "application
        /// </returns>
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
    }
}
