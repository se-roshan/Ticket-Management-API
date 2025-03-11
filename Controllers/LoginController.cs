using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using WebAPI_Code_First.Entities;
using WebAPI_Code_First.Interface;
using WebAPI_Code_First.Model;
using WebAPI_Code_First.Utilities;

namespace WebAPI_Code_First.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IAuthService _authService;

        public LoginController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest(new { StatusCode = 400, Message = "Invalid request." });
            }

            var result = await _authService.RegisterUser(user);

            if (result.Item1 == 0)
            {
                return Conflict(new { StatusCode = 409, Message = result.Item2 });
            }

            return Ok(new { StatusCode = 200, Message = "User registered successfully.", Data = new { UserId = result.Item1 } });
        }

        //-- Forgot Password
        [HttpGet("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new ResponseModel<object>(400, "Email is required", null));
            }

            var response = await _authService.ForgotPassword(email);

            if (response.StatusCode == 400)
            {
                return NotFound(response);
            }

            return Ok(response);
        }


        //-- Change Password
        [HttpPost("ChnagePassword")]
        [Authorize]
        public async Task<IActionResult> ChnagePassword([FromBody] PasswordModel password)
        {
            if (password == null)
            {
                return BadRequest(new { StatusCode = 400, Message = "Invalid request." });
            }

            //-- Get User Id from Bearer Token
            var userClaims = User.Claims.FirstOrDefault(x => x.Type == "NameIdentifier");

            if (userClaims == null || !int.TryParse(userClaims.Value, out int userId))
            {
                return Unauthorized(new { StatusCode = 401, Message = "Invalid token." });
            }

            password.UpdatedBy = userId;

            var result = await _authService.UpdatePassword(password);

            if (result.Item1 == 0)
            {
                return Conflict(new { StatusCode = 409, Message = result.Item2 });
            }

            return Ok(new { StatusCode = 200, Message = "Password changed successfully.", Data = new { UserId = result.Item1 } });
        }
         
        //-- Login User
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginUser(request.EmailOrContactNo, request.Password);
            return StatusCode(result.StatusCode, result);
        }
    }
}