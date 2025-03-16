using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAPI_Code_First.Entities;
using WebAPI_Code_First.Interface;
using WebAPI_Code_First.Migrations;
using WebAPI_Code_First.Model;
using WebAPI_Code_First.Utilities;

namespace WebAPI_Code_First.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]  
    public class FileUploadController : ControllerBase
    {
        //private readonly string _uploadFolder;
        //private readonly string _uploadProfileFolder;
        private readonly string _uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "Assset/UploadedFiles");
        private readonly string _uploadProfileFolder = Path.Combine(Directory.GetCurrentDirectory(), "Assset/ProfilePicture");

        private readonly IFileUploadService _fileUploadService; 

        public FileUploadController(IFileUploadService fileUploadService)
        {
            _fileUploadService = fileUploadService;

            //if (!Directory.Exists(_uploadFolder))
            //{
            //    Directory.CreateDirectory(_uploadFolder); // ✅ Ensure upload folder exists
            //}


            //if (!Directory.Exists(_uploadProfileFolder))
            //{
            //    Directory.CreateDirectory(_uploadProfileFolder); // ✅ Ensure upload folder exists
            //}
        }

        /// <summary>
        /// Get Base URL dynamically
        /// </summary>
        private string GetBaseUrl()
        {
            var request = HttpContext.Request;
            return $"{request.Scheme}://{request.Host}";
        }


        //-- Upload File
        [HttpPost("UploadFile")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new APIMessage(HttpStatusCode.BadRequest, "No file uploaded."));
            }

            try
            { 
                //-- Get User ID and Name from JWT Token
                var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == "NameIdentifier");
                var userNameClaim = User.Claims.FirstOrDefault(x => x.Type == "Name");

                var userId = Convert.ToInt32(userIdClaim.Value);
                var userName = userNameClaim.Value;

                //-- Generate File Name: Username_UserId_Datetime.Extension
                string fileExtension = Path.GetExtension(file.FileName);
                string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss"); // Format: YYYYMMDDHHMMSS
                string newFileName = $"{userName}_{userId}_{timestamp}{fileExtension}";

                //-- Generate Full File Path
                string filePath = Path.Combine(_uploadFolder, newFileName);
                //-- Save file to server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                //-- Save File Record in Database
                var fileRecord = new FileUpload
                {
                    UserId = userId,
                    FileName = newFileName,
                    CreatedBy = userId,
                    CreatedDateTime = DateTime.Now,
                    IsActive = true
                };

                int fileId = await _fileUploadService.SaveFileRecord(fileRecord);

                return Ok(new APIMessage(HttpStatusCode.OK, "File uploaded successfully.",
                    new {
                        FileId = fileId,
                        FileName = newFileName,
                        FulPath = $"{GetBaseUrl()}/uploads/{newFileName}"
                    }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new APIMessage(HttpStatusCode.InternalServerError, "File upload failed.", ex.Message));
            }
        }

        //-- Upload Profile Picure
        [HttpPost("UploadProfilePicture")]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file, bool IsProfile)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new APIMessage(HttpStatusCode.BadRequest, "No file uploaded."));
            }

            try
            { 
                //-- Get User ID and Name from JWT Token
                var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == "NameIdentifier");
                var userNameClaim = User.Claims.FirstOrDefault(x => x.Type == "Name");

                var userId = Convert.ToInt32(userIdClaim.Value);
                var userName = userNameClaim.Value;

                //-- Generate File Name: Username_UserId_Datetime.Extension
                string fileExtension = Path.GetExtension(file.FileName);
                string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss"); // Format: YYYYMMDDHHMMSS
                string newFileName = $"{userName}_{userId}_{timestamp}{fileExtension}";

                //-- Generate Full File Path
                string filePath = Path.Combine(_uploadProfileFolder, newFileName);
                //-- Save file to server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                //-- Save File Record in Database
                var fileRecord = new FileUpload
                {
                    UserId = userId,
                    FileName = newFileName,
                    IsCurrentProfileImage = IsProfile,
                    CreatedBy = userId,
                    CreatedDateTime = DateTime.Now,
                    IsActive = true
                };

                int fileId = await _fileUploadService.SaveProfileFileRecord(fileRecord);

                return Ok(new APIMessage(HttpStatusCode.OK, "File uploaded successfully.",
                    new
                    {
                        FileId = fileId,
                        FileName = newFileName,
                        FullPath = $"{GetBaseUrl()}/profile/{newFileName}"
                    }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new APIMessage(HttpStatusCode.InternalServerError, "File upload failed.", ex.Message));
            }
        }

        //-- Get User Files
        [HttpGet("GetUserFiles")] 
        public async Task<IActionResult> GetUserFiles()
        { 

            //--Get User ID from JWT Token
            var userClaims = User.Claims.FirstOrDefault(x => x.Type == "NameIdentifier");
            if (userClaims == null || !int.TryParse(userClaims.Value, out int userId))
            {
                return Unauthorized(new APIMessage(HttpStatusCode.Unauthorized, "Invalid user ID in token"));
            }
            //string filePath = _uploadFolder;
            //var files = await _fileUploadService.GetFilesByUserId(userId, filePath);

            //-- Generate Base URL Dynamically
            var request = HttpContext.Request;
            string baseUrl = $"{request.Scheme}://{request.Host}";  // Auto-detects local/prod URL

            //-- Pass Base URL Instead of Just Folder Path
            var files = await _fileUploadService.GetFilesByUserId(userId, baseUrl);


            if (files == null || files.Count == 0)
            {
                return NotFound(new APIMessage(HttpStatusCode.NotFound, "No files found for this user."));
            }

            return Ok(new APIMessage(HttpStatusCode.OK, Constants.SUCCESSMSG, files));
        }

        //--  Remove Porfile Image
        [HttpGet("RemoveProfileImage")]
        public async Task<IActionResult> RemoveProfileImage(int userId)
        {
            bool isRemoved = await _fileUploadService.RemoveProfileImage(userId);

            if (!isRemoved)
            {
                return BadRequest(new APIMessage(HttpStatusCode.BadRequest, "No active profile image found to remove."));
            }

            return Ok(new ResponseModel<object>(200, "Profile image removed successfully.", new { userId }));
        }


    }
}
