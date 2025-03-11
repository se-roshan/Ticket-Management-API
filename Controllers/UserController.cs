using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
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
        //[Authorize]
        public async Task<IActionResult> GetUserDetails()
        {
            // Define the upload folder path
            string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");

            // Ensure the directory exists
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            var response = await _userService.GetAllUserDetails(uploadFolder);

            if (response != null && response.Any())
            {
                return Ok(new APIMessage(HttpStatusCode.OK, Constants.SUCCESSMSG, response));
            }

            return NotFound(new APIMessage(HttpStatusCode.NotFound, Constants.FAILUREMSG, response));
        }


        [HttpPost("AddUpdateRole")]
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
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _userService.GetRoles();

            if (roles == null || !roles.Any())
                return NotFound(new ResponseModel<object>(404, "No roles found", null));

            return Ok(new ResponseModel<List<RoleModel>>(200, Constants.SUCCESSMSG, roles));
        }


    }
}
