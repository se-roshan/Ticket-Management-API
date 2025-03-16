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
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

         //-- Get All User List 
        [HttpGet("GetAllUsers")]
        [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            var response = await _userService.GetAllUsers();

            if (response.StatusCode == 200)
            {
                return Ok(new APIMessage(HttpStatusCode.OK, Constants.SUCCESSMSG, response.Data));
            }

            if (response.StatusCode == 404)
            {
                return NotFound(new APIMessage(HttpStatusCode.NotFound, Constants.NO_RECORDS_FOUND));
            }

            return StatusCode(500, new APIMessage(HttpStatusCode.InternalServerError, "An error occurred while processing your request."));
        }

        //-- Get User Details with Profile image 
        [HttpGet("GetUserDetails")]
        [Authorize]
        public async Task<IActionResult> GetUserDetails()
        {
            //// Define the upload folder path
            //string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");

            //// Ensure the directory exists
            //if (!Directory.Exists(uploadFolder))
            //{
            //    Directory.CreateDirectory(uploadFolder);
            //}

            //var response = await _userService.GetAllUserDetails(uploadFolder);

            // Construct Base URL (Example: http://localhost:5000)
            string baseUrl = $"{Request.Scheme}://{Request.Host.Value}";

            var response = await _userService.GetAllUserDetails(baseUrl);

            if (response != null && response.Any())
            {
                return Ok(new APIMessage(HttpStatusCode.OK, Constants.SUCCESSMSG, response));
            }

            return NotFound(new APIMessage(HttpStatusCode.NotFound, Constants.FAILUREMSG, response));
        }


        //-- Get User Details with Profile image on the basic of token
        [HttpGet("GetUserDetail")]
        [Authorize]
        public async Task<IActionResult> GetUserDetail()
        {
            var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == "NameIdentifier");
            if (userIdClaim == null)
            {
                return Unauthorized(new APIMessage(HttpStatusCode.Unauthorized, "Invalid token. User not identified."));
            }
            var userId = Convert.ToInt32(userIdClaim.Value); 

            // Construct Base URL (Example: http://localhost:5000)
            string baseUrl = $"{Request.Scheme}://{Request.Host.Value}";

            var response = await _userService.GetUserDetailById(baseUrl, userId);

            if (response != null)
            {
                return Ok(new APIMessage(HttpStatusCode.OK, Constants.SUCCESSMSG, response));
            }

            return NotFound(new APIMessage(HttpStatusCode.NotFound, Constants.FAILUREMSG, response));
        }



        [HttpPost("AddUpdateRole")]
        [Authorize]
        public async Task<IActionResult> AddUpdateRole([FromBody] RoleModel role)
        {
            if (role == null)
                return BadRequest(new ResponseModel<object>(400, "Invalid role data", null));

            int roleId = await _userService.AddUpdateRole(role);

            if (roleId == 0)
                return NotFound(new ResponseModel<object>(404, "Role not found", null));

            return Ok(new ResponseModel<object>(200, Constants.SUCCESSMSG, new { roleId = roleId }));
        }

        [HttpGet("GetRoles")]
        [Authorize]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _userService.GetRoles();

            if (roles == null || !roles.Any())
                return NotFound(new ResponseModel<object>(404, "No roles found", null));

            return Ok(new ResponseModel<List<RoleModel>>(200, Constants.SUCCESSMSG, roles));
        }

        [HttpPost("AddUpdateUser")]
        [Authorize]
        public async Task<IActionResult> AddUpdateUser([FromBody] UserListModel userList)
        {
            var userId = await _userService.AddUpdatedUser(userList);

            if (userId == 0)
                return BadRequest(new ResponseModel<object>(400, "Failed to add or update user.", null));

            string message = userList.Id > 0 ? "User updated successfully." : "User added successfully.";

            return Ok(new ResponseModel<int>(200, message, userId));
        }

    }
}
