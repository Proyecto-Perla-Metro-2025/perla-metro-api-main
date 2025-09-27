using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiMain.src.Dto;
using ApiMain.src.models;
using ApiMain.src.DTOs;
using ApiMain.src.Helper;

namespace ApiMain.src.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<string> GetServiceTokenAsync();
        Task<string> GetCurrentUserId();
        Task<VisualizeUserDto> getUser(string id);
        Task<List<VisualizeUserDto>?> getAll();
        Task<UserDto> UpdateUser(UpdateUserDto updateUserDto);
        Task<UserDto> RegisterUser(CreateUserDto createUserDto);
        Task EnableDisableUser(string id);
        Task<List<VisualizeUserDto>> GetUsersFilter(QueryObject queryObject);
        Task<TokenValidationResponse> ValidateTokenAsync(string token);
    }
}