using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ApiMain.src.Interfaces;
using ApiMain.src.models;
using ApiMain.src.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ApiMain.src.DTOs;
using ApiMain.src.Helper;

namespace ApiMain.src.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var response = await _authService.LoginAsync(request);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (ServiceUnavailableException ex)
            {
                return StatusCode(503, new { Message = ex.Message });
            }
        }


        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser(string id)
        {
            try
            {
                var response = await _authService.getUser(id);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (ServiceUnavailableException ex)
            {
                return StatusCode(503, new { Message = ex.Message });
            }
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var response = await _authService.getAll();
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (ServiceUnavailableException ex)
            {
                return StatusCode(503, new { Message = ex.Message });
            }
        }

        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                var response = await _authService.UpdateUser(updateUserDto);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (ServiceUnavailableException ex)
            {
                return StatusCode(503, new { Message = ex.Message });
            }
        }

        [HttpPut("enable-disable")]
        public async Task<IActionResult> EnableDisableUser(string Id)
        {
            try
            {
                await _authService.EnableDisableUser(Id);
                return Ok();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (ServiceUnavailableException ex)
            {
                return StatusCode(503, new { Message = ex.Message });
            }
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] CreateUserDto createUserDto)
        {
            try
            {
                var response = await _authService.RegisterUser(createUserDto);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("UserFilter")]
        public async Task<IActionResult> GetUsersFilter([FromQuery] QueryObject query)
        {
            try
            {
                var users = await _authService.GetUsersFilter(query);
                return Ok(users);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (ServiceUnavailableException ex)
            {
                return StatusCode(503, new { Message = ex.Message });
            }
        }
    }
}