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
    /// <summary>
    /// Authentication controller.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // authService connection
        private readonly IAuthService _authService;

        /// <summary>
        /// Constructor that initializes the authService.
        /// </summary>
        /// <param name="ticketRepository">Repositorio de tickets.</param>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// This C# function handles a POST request for user login, validating the request and returning
        /// appropriate responses based on the outcome.
        /// </summary>
        /// <param name="LoginRequest">The `LoginRequest` parameter in the `Login` method represents the
        /// data that is expected to be sent in the body of the HTTP POST request when a user is trying
        /// to log in. This data typically includes the user's credentials such as username and
        /// password.</param>
        /// <returns>
        /// The Login method returns an ActionResult of type LoginResponse. Depending on the scenario,
        /// it can return a BadRequest response with the ModelState if the request is not valid, an Ok
        /// response with the login response if successful, an Unauthorized response with a message if
        /// an UnauthorizedAccessException is caught, or a StatusCode 503 response with a message if a
        /// ServiceUnavailableException is caught.
        /// </returns>
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


        /// <summary>
        /// This C# function uses an HTTP GET request to retrieve user information based on the provided
        /// ID, handling exceptions for unauthorized access and service unavailability.
        /// </summary>
        /// <param name="id">The `id` parameter in the `GetUser` method is used to identify the user
        /// whose information is being retrieved. It is passed as a query parameter in the HTTP request
        /// to fetch the user details.</param>
        /// <returns>
        /// The GetUser method returns an IActionResult based on the outcome of the
        /// _authService.getUser(id) method call. If the call is successful, it returns an Ok response
        /// with the retrieved user data. If an UnauthorizedAccessException is caught, it returns a 401
        /// Unauthorized response with a message from the exception. If a ServiceUnavailableException is
        /// caught, it returns a 503 Service Unavailable response with a message from
        /// </returns>
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

        /// <summary>
        /// This C# function uses an HTTP GET request to retrieve all data, handling
        /// UnauthorizedAccessException and ServiceUnavailableException with appropriate responses.
        /// </summary>
        /// <returns>
        /// The GetAll method is returning an IActionResult. If the operation is successful, it returns
        /// an Ok response with the data retrieved from the _authService.getAll() method. If an
        /// UnauthorizedAccessException is caught, it returns an Unauthorized response with a message.
        /// If a ServiceUnavailableException is caught, it returns a StatusCode 503 response with a
        /// message.
        /// </returns>
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

        /// <summary>
        /// This C# function handles updating a user with error handling for UnauthorizedAccessException
        /// and ServiceUnavailableException.
        /// </summary>
        /// <param name="UpdateUserDto">The `UpdateUserDto` is a data transfer object (DTO) that
        /// contains the information needed to update a user. It likely includes properties such as the
        /// user's ID, name, email, and any other fields that can be updated for a user. This DTO is
        /// used to pass the user update</param>
        /// <returns>
        /// The UpdateUser method returns an IActionResult. If the update is successful, it returns an
        /// Ok response with the updated user information. If an UnauthorizedAccessException is caught,
        /// it returns an Unauthorized response with the exception message. If a
        /// ServiceUnavailableException is caught, it returns a 503 status code response with the
        /// exception message.
        /// </returns>
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

        /// <summary>
        /// The function `EnableDisableUser` in a C# controller handles enabling or disabling a user and
        /// returns appropriate responses for different exceptions.
        /// </summary>
        /// <param name="Id">The `Id` parameter in the `EnableDisableUser` method is used to identify
        /// the user whose account will be enabled or disabled. This method is typically called with the
        /// user's unique identifier to perform the enable/disable operation on that specific user
        /// account.</param>
        /// <returns>
        /// The `EnableDisableUser` method from the `_authService` is being called with the `Id`
        /// parameter. If the operation is successful, an `Ok` response is returned. If an
        /// `UnauthorizedAccessException` is caught, a 401 Unauthorized response is returned with a
        /// message containing the exception's message. If a `ServiceUnavailableException` is caught, a
        /// 503 Service Unavailable response is
        /// </returns>
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

        /// <summary>
        /// This C# function handles a POST request to register a new user and returns the registration
        /// response or an error message.
        /// </summary>
        /// <param name="CreateUserDto">The `CreateUserDto` is likely a data transfer object (DTO) that
        /// contains the necessary information to register a new user. It may include properties such as
        /// username, email, password, and any other required user details. This object is received as
        /// the request body in the `Register` action of</param>
        /// <returns>
        /// The Register method is returning an IActionResult. If the registration is successful, it
        /// returns an Ok response with the data returned from the
        /// _authService.RegisterUser(createUserDto) method. If an exception occurs during the
        /// registration process, it returns a StatusCode 500 response with the exception message.
        /// </returns>
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

        /// <summary>
        /// This C# function handles a GET request to filter users based on a query object, returning
        /// the filtered users or appropriate error responses.
        /// </summary>
        /// <param name="QueryObject">The `QueryObject` is a parameter that is being passed in the
        /// `GetUsersFilter` method as a query string from the HTTP request. It likely contains
        /// properties that are used to filter and retrieve a specific set of users from the
        /// `_authService`.</param>
        /// <returns>
        /// The GetUsersFilter method returns an IActionResult, which can be one of the following:
        /// 1. If the operation is successful, it returns an Ok result with the users data.
        /// 2. If an UnauthorizedAccessException is caught, it returns an Unauthorized result with a
        /// message containing the exception's message.
        /// 3. If a ServiceUnavailableException is caught, it returns a StatusCode 503 result with a
        /// message containing the
        /// </returns>
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