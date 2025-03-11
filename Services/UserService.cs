using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using WebAPI_Code_First.Data;
using WebAPI_Code_First.Entities;
using WebAPI_Code_First.Interface;
using WebAPI_Code_First.Model;

namespace WebAPI_Code_First.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }
         
        //-- Get All User
        public async Task<ResponseModel<List<UserListModel>>> GetAllUsers()
        {
            try
            {
                var users = await _context.Users.ToListAsync(); // Fetch data

                if (users == null || users.Count == 0)
                {
                    return new ResponseModel<List<UserListModel>>(404, "No users found", null);
                }

                var userList = users.Select(u => new UserListModel
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    ContactNo = u.ContactNo,
                    Gender = u.Gender,
                    DOB = u.DOB,
                    IsActive = u.IsActive,
                    CreatedBy = u.CreatedBy,
                    CreatedDateTime = u.CreatedDateTime,
                    UpdatedBy = u.UpdatedBy,
                    UpdatedDateTime = u.UpdatedDateTime,
                }).ToList();

                return new ResponseModel<List<UserListModel>>(200, "Users retrieved successfully", userList);
            }
            catch (Exception ex)
            {
                // Log the exception if logging is set up
                Console.WriteLine($"Error: {ex.Message}");

                return new ResponseModel<List<UserListModel>>(500, "An error occurred while retrieving users", null);
            }
        }

        //-- Update User 
        public async Task<int> UpdateUser(UserListModel updatedUser)
        {
            var user = await _context.Users.FindAsync(updatedUser.Id);
            if (user == null) return 0;

            user.Name = updatedUser.Name ?? user.Name;
            user.ContactNo = updatedUser.ContactNo ?? user.ContactNo;
            user.Email = updatedUser.Email ?? user.Email;
            user.Gender = updatedUser.Gender ?? user.Gender;
            user.IsActive = updatedUser.IsActive;
            user.UpdatedBy = updatedUser.UpdatedBy;
            user.UpdatedDateTime = DateTime.Now;

            await _context.SaveChangesAsync();
            return user.Id;
        }

        //-- Get All User Details

        //-- 3rd way 3.35 s
        public async Task<List<UserProfileModel>> GetAllUserDetails(string uploadFolder)
        {
            // Fetch users & file uploads separately (single DB calls)
            var users = await _context.Users.AsNoTracking().ToListAsync();
            var fileUploads = await _context.FileUploads.AsNoTracking().ToListAsync();

            // Use ToLookup for fast in-memory grouping
            var fileUploadsByUser = fileUploads.ToLookup(f => f.UserId);

            var userList = users.Select(u => new UserProfileModel
            {
                Id = u.Id,
                Name = u.Name,
                ContactNo = u.ContactNo,
                EmailId = u.Email,
                IsActive = u.IsActive,
                UserProfilePics = fileUploadsByUser[u.Id] // Fast lookup, avoids looping
                    .OrderByDescending(f => f.CreatedDateTime) // Get latest files first
                    .Select(f => new UserProfilePics
                    {
                        Id = f.Id,
                        UserId = f.UserId,
                        FilePath = Path.Combine(uploadFolder, f.FileName), // ✅ Construct full file path
                        FileName = f.FileName,
                        IsCurrentProfileImage = f.IsCurrentProfileImage,
                        IsActive = f.IsActive,
                        CreatedBy = f.CreatedBy,
                        CreatedDateTime = f.CreatedDateTime,
                        UpdatedBy = f.UpdatedBy,
                        UpdatedDateTime = f.UpdatedDateTime
                    }).ToList()
            }).ToList();

            return userList;
        }


        //-- 2nd way 3.46 s
        //public async Task<List<UserProfileModel>> GetAllUserDetails(string uploadFolder)
        //{
        //    var users = await _context.Users.ToListAsync(); // Fetch all users
        //    var fileUploads = await _context.FileUploads.ToListAsync(); // Fetch all file uploads

        //    var userList = users.Select(u => new UserProfileModel
        //    {
        //        Id = u.Id,
        //        Name = u.Name,
        //        ContactNo = u.ContactNo,
        //        EmailId = u.Email,
        //        IsActive = u.IsActive,
        //        UserProfilePics = fileUploads
        //            .Where(f => f.UserId == u.Id) // Get matching file uploads
        //            .OrderByDescending(f => f.CreatedDateTime)
        //            .Select(f => new UserProfilePics
        //            {
        //                Id = f.Id,
        //                UserId = f.UserId,
        //                FilePath = Path.Combine(uploadFolder, f.FileName), // ✅ Construct full file path
        //                FileName = f.FileName,
        //                IsCurrentProfileImage = f.IsCurrentProfileImage,
        //                IsActive = f.IsActive,
        //                CreatedBy = f.CreatedBy,
        //                CreatedDateTime = f.CreatedDateTime,
        //                UpdatedBy = f.UpdatedBy,
        //                UpdatedDateTime = f.UpdatedDateTime
        //            }).ToList()
        //    }).ToList();

        //    return userList;
        //}


        //-- 1st way 3sec
        //public async Task<List<UserProfileModel>> GetAllUserDetails(string uploadFolder)
        //{
        //    var result = await _context.Users.ToListAsync();

        //    var userList = result.Select(u => new UserProfileModel
        //    {
        //        Id = u.Id,
        //        Name = u.Name,
        //        ContactNo = u.ContactNo,
        //        EmailId = u.Email,
        //        IsActive = u.IsActive,
        //        UserProfilePics = _context.FileUploads
        //            .Where(f => f.UserId == u.Id)
        //            .OrderByDescending(o => o.CreatedDateTime)
        //            .Select(f => new UserProfilePics
        //            {
        //                Id = f.Id,
        //                UserId = f.UserId,
        //                FilePath = Path.Combine(uploadFolder, f.FileName),
        //                FileName = f.FileName,
        //                IsCurrentProfileImage = f.IsCurrentProfileImage,
        //                IsActive = f.IsActive,
        //                CreatedBy = f.CreatedBy,
        //                CreatedDateTime = f.CreatedDateTime,
        //                UpdatedBy = f.UpdatedBy,
        //                UpdatedDateTime = f.UpdatedDateTime
        //            }).ToList()
        //    }).ToList();

        //    return userList;
        //}

        public async Task<int> AddUpdateRole(RoleModel role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            if (role.Id == 0)
            {
                // Add new role
                var newRole = new Role
                {
                    Name = role.Name,
                    Description = role.Description,
                    IsActive = role.IsActive,
                    CreatedBy = role.CreatedBy,
                    CreatedDateTime = DateTime.UtcNow
                };

                _context.Roles.Add(newRole);
                await _context.SaveChangesAsync();
                return newRole.Id; // Return newly created Role ID
            }
            else
            {
                // Update existing role
                var existingRole = await _context.Roles.FindAsync(role.Id);
                if (existingRole == null)
                    return 0; // Role not found

                existingRole.Name = role.Name;
                existingRole.Description = role.Description;
                existingRole.IsActive = role.IsActive;
                existingRole.UpdatedBy = role.UpdatedBy;
                existingRole.UpdatedDateTime = DateTime.UtcNow;

                _context.Roles.Update(existingRole);
                await _context.SaveChangesAsync();
                return existingRole.Id; // Return updated Role ID
            }
        }
        public async Task<List<RoleModel>> GetRoles()
        {
            var roles = await _context.Roles
                .Select(role => new RoleModel
                {
                    Id = role.Id,
                    Name = role.Name,
                    Description = role.Description,
                    IsActive = role.IsActive,
                    CreatedBy = role.CreatedBy,
                    CreatedDateTime = role.CreatedDateTime,
                    UpdatedBy = role.UpdatedBy,
                    UpdatedDateTime = role.UpdatedDateTime
                })
                .ToListAsync();

            return roles;
        }

    }
}
