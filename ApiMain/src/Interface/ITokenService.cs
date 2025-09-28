using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApiMain.src.models;
using ApiMain.src.DTOs;

namespace ApiMain.src.Interfaces
{
    public interface ITokenService
    {
        string GenerateJwtToken(List<Claim> claims);
        ClaimsPrincipal? ValidateToken(string token);
        bool IsTokenValid(string token);
    }
}