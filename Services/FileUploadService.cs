using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI_Code_First.Data;
using WebAPI_Code_First.Entities;
using WebAPI_Code_First.Interface;
using WebAPI_Code_First.Model;

namespace WebAPI_Code_First.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly ApplicationDbContext _context;

        public FileUploadService(ApplicationDbContext context)
        {
            _context = context;
        }

        //-- Save File Record
        public async Task<int> SaveFileRecord(FileUpload fileUpload)
        {
            _context.FileUploads.Add(fileUpload);

            await _context.SaveChangesAsync();
            return fileUpload.Id;
        }

        //-- Save Profile File Record
        public async Task<int> SaveProfileFileRecord(FileUpload fileUpload)
        {
            //-- Get Last Profile Picture for the User
            var lastProfilePic = await _context.FileUploads
                .Where(f => f.UserId == fileUpload.UserId && f.IsCurrentProfileImage == true)
                .OrderByDescending(f => f.CreatedDateTime)
                .FirstOrDefaultAsync();

            //-- If user has an existing profile picture, update it to false
            if (lastProfilePic != null)
            {
                lastProfilePic.IsCurrentProfileImage = false;
                lastProfilePic.UpdatedBy = fileUpload.CreatedBy;
                lastProfilePic.UpdatedDateTime = DateTime.Now;
            }

            _context.FileUploads.Add(fileUpload);

            await _context.SaveChangesAsync();
            return fileUpload.Id;
        }

        //-- Get Files by User ID
        public async Task<List<UserProfilePics>> GetFilesByUserId(int userId, string baseUrl)
        {
            try
            {
                var files = await _context.FileUploads.Where(f => f.UserId == userId).OrderByDescending(f => f.CreatedDateTime).ToListAsync();

                if (files == null || files.Count == 0)
                {
                    return new List<UserProfilePics>();
                }

                var fileList = files.Select(f => new UserProfilePics
                {
                    Id = f.Id,
                    UserId = f.UserId,
                    //FilePath = FilePath,
                    // Use 'profile' for profile images, 'uploads' for other files
                    FilePath = f.IsCurrentProfileImage ? $"{baseUrl}/profile/{f.FileName}" : $"{baseUrl}/uploads/{f.FileName}",

                    FileName = f.FileName,
                    IsCurrentProfileImage = f.IsCurrentProfileImage,
                    IsActive = f.IsActive,
                    CreatedBy = f.CreatedBy,
                    CreatedDateTime = f.CreatedDateTime,
                    UpdatedBy = f.UpdatedBy,
                    UpdatedDateTime = f.UpdatedDateTime
                }).ToList();

                return fileList;
            }
            catch (Exception ex)
            {
                // Log the exception if logging is set up
                Console.WriteLine($"Error: {ex.Message}");
                return new List<UserProfilePics>();
            }
        }

        //--
        public async Task<bool> RemoveProfileImage(int userId)
        {
            var fileUpload = await _context.FileUploads
                .Where(f => f.IsCurrentProfileImage == true && f.UserId == userId)
                .FirstOrDefaultAsync(); // Corrected the query

            if (fileUpload == null)
            {
                return false; // No active profile image found
            }

            // Remove the image by marking it as inactive
            fileUpload.IsCurrentProfileImage = false;
            fileUpload.UpdatedBy = userId;
            fileUpload.UpdatedDateTime = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }


    }
}
